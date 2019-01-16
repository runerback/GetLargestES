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

        int[] CalculateMinimalConvex(PointData[] points)
        {
            var count = points?.Length ?? 0;
            if (count < 3)
                return null;

            //calculate


            return points.Select(item => item.Index).ToArray();
        }

        PathFigure GenerateRootFigure(PointData[] points, Func<PointData, PointPresenter> itemContainerSelector)
        {
            var indexes = CalculateMinimalConvex(points);
            var count = indexes?.Length ?? 0;
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
                    Source = itemContainerSelector(points[indexes[0]])
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
                        Source = itemContainerSelector(points[indexes[i]])
                    });
                segments.Add(segment);
            }

            return figure;
        }

        #endregion Core
    }
}
