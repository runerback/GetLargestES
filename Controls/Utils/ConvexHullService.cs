using Runerback.Utils.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    sealed class ConvexHullService : Behavior<PointCanvas>
    {
        public ConvexHullService()
        {
            builder = new ConvexHullBuilder(this);
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
                typeof(ConvexHullService),
                new PropertyMetadata());

        public static readonly DependencyProperty ConvexPathDataProperty =
            ConvexPathDataPropertyKey.DependencyProperty;

        #endregion ConvexPathData

        #region ConvexHull

        public static IEnumerable<PointData> GetConvexHull(PointCanvas d)
        {
            return (IEnumerable<PointData>)d.GetValue(ConvexHullProperty);
        }

        static void SetConvexHull(PointCanvas d, IEnumerable<PointData> value)
        {
            d.SetValue(ConvexHullPropertyKey, value);
        }

        static readonly DependencyPropertyKey ConvexHullPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly(
                "ConvexHull",
                typeof(IEnumerable<PointData>),
                typeof(ConvexHullService),
                new PropertyMetadata());

        public static readonly DependencyProperty ConvexHullProperty =
            ConvexHullPropertyKey.DependencyProperty;

        #endregion ConvexHull

        protected override void OnAttached()
        {
            var target = AssociatedObject;

            SetConvexPathData(target, new PathGeometry());
            SetConvexHull(target, builder.ConvexHull);

            target.ItemsChanged += OnItemsChanged;
        }

        protected override void OnDetaching()
        {
            var target = AssociatedObject;

            target.ItemsChanged -= OnItemsChanged;
            SetConvexPathData(target, null);
            SetConvexHull(target, null);
        }

        private readonly ConvexHullBuilder builder;

        private void OnItemsChanged(object sender, EventArgs e)
        {
            builder.Build();
        }
        
        sealed class ConvexHullBuilder
        {
            public ConvexHullBuilder(ConvexHullService host)
            {
                this.host = host ?? throw new ArgumentNullException(nameof(host));
            }

            private readonly ConvexHullService host;

            private readonly AutoInvokeObservableCollection<PointData> convexHull =
                new AutoInvokeObservableCollection<PointData>();
            public IEnumerable<PointData> ConvexHull => convexHull;
            
            public void Build()
            {
                var hull = convexHull;
                 var source = host.AssociatedObject;

                var points = source.Points.ToArray();
                var orderedPoints = CalculateMinimalConvex(points);
                var count = orderedPoints?.Length ?? 0;
                if (count < 3)
                {
                    SetConvexPathData(source, null);
                    hull.Reset(Enumerable.Empty<PointData>());
                    return;
                }

                if (hull.Select(item => item.Index).SequenceEqual(
                    orderedPoints.Select(item => item.Index)))
                    return;
                
                hull.Reset(orderedPoints);

                var geometry = new PathGeometry();
                var figure = GenerateRootFigure(orderedPoints, source.FindContainer);
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

                SortByAngle(points);

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

            void SortByAngle(PointData[] points)
            {
                var basePoint = points[0];
                var baseIndex = basePoint.Index;
                var p0 = basePoint.Point;

                var indexMap = new Dictionary<PointData, int>();
                for (int i = 1, j = points.Length; i < j; i++)
                {
                    var point = points[i];
                    if (point.Index < baseIndex)
                        indexMap.Add(point, point.Index + 1);
                    else
                        indexMap.Add(point, point.Index);
                }

                var sortingIndex = 0;
                foreach (var group in points
                    .Skip(1)
                    .GroupBy(item => item.Point.X.CompareTo(p0.X))
                    .OrderByDescending(item => item.Key))
                {
                    IEnumerable<PointData> sorted;
                    if (group.Key == 0)
                        sorted = group.OrderBy(item => item.Point.Y);
                    else
                        sorted = group.OrderBy(item =>
                        {
                            var p = item.Point;
                            return (p.Y - p0.Y) / (p.X - p0.X);
                        });

                    foreach (var sortedItem in sorted)
                    {
                        sortingIndex++;

                        var originItem = points[sortingIndex];
                        if (sortedItem == originItem)
                            continue;

                        var sortedItemIndex = indexMap[sortedItem];

                        points[sortingIndex] = sortedItem;
                        points[sortedItemIndex] = originItem;

                        indexMap[sortedItem] = sortingIndex;
                        indexMap[originItem] = sortedItemIndex;
                    }
                }

                indexMap = null;
            }

            PathFigure GenerateRootFigure(PointData[] orderedPoints, Func<PointData, UIElement> itemContainerSelector)
            {
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
                for (int i = 1, j = orderedPoints.Length; i < j; i++)
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

        }
    }
}
