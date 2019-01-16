using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GetLargestES
{
    sealed class MinimalConvexService : Behavior<PointCanvas>
    {
        public MinimalConvexService()
        {
        }

        #region ConvexPathData

        public static Geometry GetConvexPathData(PointCanvas d)
        {
            return (Geometry)d.GetValue(ConvexPathDataProperty);
        }

        static void SetConvexPathData(PointCanvas d, Geometry value)
        {
            d.SetValue(ConvexPathDataPropertyKey, value);
        }

        static readonly DependencyPropertyKey ConvexPathDataPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly(
                "ConvexPathData",
                typeof(Geometry),
                typeof(MinimalConvexService),
                new PropertyMetadata());

        public static readonly DependencyProperty ConvexPathDataProperty =
            ConvexPathDataPropertyKey.DependencyProperty;

        #endregion ConvexPathData

        protected override void OnAttached()
        {
            var target = AssociatedObject;

            SetConvexPathData(target, new PathGeometry());
            target.ItemsChanged += OnItemsChanged;
        }

        protected override void OnDetaching()
        {
            var target = AssociatedObject;

            target.ItemsChanged -= OnItemsChanged;
            SetConvexPathData(target, null);
        }

        private void OnItemsChanged(object sender, EventArgs e)
        {
            UpdateConvexPathData((PointCanvas)sender);
        }

        #region Core

        void UpdateConvexPathData(PointCanvas source)
        {
            var figure = GenerateRootFigure(
                source.ItemsSource.OfType<PointData>().ToArray(),
                source.FindContainer);
            if (figure == null)
            {
                SetConvexPathData(source, null);
                return;
            }

            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);
            SetConvexPathData(source, geometry);
        }

        IEnumerable<PointData> CalculateMinimalConvex(PointData[] points)
        {
            var count = points?.Length ?? 0;
            if (count < 3)
                yield break;
            
            //calculate

            //step one: find the point with lowest y and lowest x
            var P = points
                .OrderBy(item => item.Point.Y)
                .OrderBy(item => item.Point.X)
                .First();

            var right = new Vector(1, 0);
            var others = OrderByAngle(
                points.Where(item => item.Index != P.Index), P.Point)
                .ToArray();
            
            var stack = new Stack<PointData>();
            stack.Push(P);
            stack.Push(others[0]);
            stack.Push(others[1]);

            for (int i = 2, j = count - 1; i < j;)
            {
                var item3 = others[i];
                var item2 = stack.Pop();
                var item1 = stack.Peek();

                var p3 = item3.Point;
                var p2 = item2.Point;
                var p1 = item1.Point;

                var dir = (p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X);
                if (dir > 0)
                    stack.Push(item2);
                else
                    item2.ToggleCheckedState();

                stack.Push(item3);
                i++;
            }

            while (stack.Count > 0)
                yield return stack.Pop();
        }

        IEnumerable<PointData> OrderByAngle(IEnumerable<PointData> source, Point basePoint)
        {
            foreach (var group in source
                .GroupBy(item => item.Point.X.CompareTo(basePoint.X))
                .OrderByDescending(item => item.Key))
            {
                switch (group.Key)
                {
                    case 0:
                        foreach (var item in group.OrderBy(item => item.Point.Y))
                            yield return item;
                        break;
                    default:
                        foreach (var item in group.OrderBy(item =>
                        {
                            var p = item.Point;
                            return (p.Y - basePoint.Y) / Math.Abs(p.X - basePoint.X);
                        }))
                            yield return item;
                        break;
                }
            }
        }

        PathFigure GenerateRootFigure(PointData[] points, Func<PointData, PointPresenter> itemContainerSelector)
        {
            var orderedPoints = CalculateMinimalConvex(points).ToArray();
            var count = orderedPoints?.Length ?? 0;
            if (count < 3)
                return null;

            var figure = new PathFigure { IsClosed = true };

            BindingOperations.SetBinding(
                figure,
                PathFigure.StartPointProperty,
                new Binding
                {
                    Path = new PropertyPath(PositionExtension.SegmentPositionProperty),
                    Mode = BindingMode.OneWay,
                    Source = itemContainerSelector(orderedPoints[0])
                });

            var segments = figure.Segments;
            for (int i = 1, j = count; i < j; i++)
            {
                var segment = new LineSegment();
                BindingOperations.SetBinding(
                    segment,
                    LineSegment.PointProperty,
                    new Binding
                    {
                        Path = new PropertyPath(PositionExtension.SegmentPositionProperty),
                        Mode = BindingMode.OneWay,
                        Source = itemContainerSelector(orderedPoints[i])
                    });
                segments.Add(segment);
            }

            return figure;
        }

        #endregion Core
    }
}
