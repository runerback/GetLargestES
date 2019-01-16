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

        PointData[] CalculateMinimalConvex(PointData[] points)
        {
            var count = points?.Length ?? 0;
            if (count < 3)
                return null;
            
            var p0 = points
                .OrderBy(item => item.Point.Y)
                .ThenBy(item => item.Point.X)
                .First();
            if (p0.Index > 0)
            {
                for (int i = p0.Index - 1; i >= 0; i--)
                    points[i + 1] = points[i];
                points[0] = p0;
            }

            points = SortByAngle(points);

            var stack = new Stack<PointData>(points.Take(2));
            for (int i = 2, j = count; i < j && stack.Count >= 2;)
            {
                var item3 = points[i];
                var item2 = stack.Pop();
                var item1 = stack.Peek();

                var p3 = item3.Point;
                var p2 = item2.Point;
                var p1 = item1.Point;

                var dir = (p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X);
                if (dir > 0)
                {
                    stack.Push(item2);
                    stack.Push(item3);
                    i++;
                }
            }

            if (stack.Count < 3)
                return null;
            return stack.ToArray();
        }

        PointData[] SortByAngle(PointData[] points)
        {
            var basePoint = points[0].Point;
            List<PointData> result = new List<PointData>(points.Length);
            result.Add(points[0]);

            foreach (var group in points
                .Skip(1)
                .GroupBy(item => item.Point.X.CompareTo(basePoint.X))
                .OrderByDescending(item => item.Key))
            {
                IEnumerable<PointData> sorted;
                if (group.Key == 0)
                    sorted = group.OrderBy(item => item.Point.Y);
                else
                    sorted = group.OrderBy(item =>
                    {
                        var p = item.Point;
                        return (p.Y - basePoint.Y) / (p.X - basePoint.X);
                    });

                result.AddRange(sorted);
            }

            return result.ToArray();
        }

        PathFigure GenerateRootFigure(PointData[] points, Func<PointData, PointPresenter> itemContainerSelector)
        {
            var orderedPoints = CalculateMinimalConvex(points);
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
