using Runerback.Utils.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GetLargestES
{
    [TemplatePart(Name = PointBoardName, Type = typeof(PointCanvas))]
    [TemplatePart(Name = ConvexPathName, Type = typeof(Path))]
    sealed class PointSetPresenter : Control
    {
        public PointSetPresenter()
        {
            SetValue(PointsPropertyKey, points);
        }
        
        #region Scale

        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register(
                "Scale",
                typeof(double),
                typeof(PointSetPresenter),
                new PropertyMetadata(Constants.DEFAULT_SCALE, OnScalePropertyChanged, CoereScahlePropertyCallBack));

        private static void OnScalePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static object CoereScahlePropertyCallBack(DependencyObject d, object baseValue)
        {
            if (baseValue is double scale)
            {
                if (scale <= 0)
                    return Constants.MIN_SCALE;
                return scale;
            }
            return Constants.DEFAULT_SCALE;
        }

        #endregion Scale

        #region PointStroke

        public Brush PointStroke
        {
            get { return (Brush)GetValue(PointStrokeProperty); }
            set { SetValue(PointStrokeProperty, value); }
        }

        public static readonly DependencyProperty PointStrokeProperty =
            DependencyProperty.Register(
                "PointStroke",
                typeof(Brush),
                typeof(PointSetPresenter));

        #endregion PointStroke

        #region SelectionPointStroke

        public Brush SelectionPointStroke
        {
            get { return (Brush)GetValue(SelectionPointStrokeProperty); }
            set { SetValue(SelectionPointStrokeProperty, value); }
        }

        public static readonly DependencyProperty SelectionPointStrokeProperty =
            DependencyProperty.Register(
                "SelectionPointStroke",
                typeof(Brush),
                typeof(PointSetPresenter));

        #endregion SelectionPointStroke

        #region ConvexStroke

        public Brush ConvexStroke
        {
            get { return (Brush)GetValue(ConvexStrokeProperty); }
            set { SetValue(ConvexStrokeProperty, value); }
        }

        public static readonly DependencyProperty ConvexStrokeProperty =
            DependencyProperty.Register(
                "ConvexStroke",
                typeof(Brush),
                typeof(PointSetPresenter));
        
        #endregion ConvexStroke
        
        #region Add/Remove Points

        private readonly ObservableCollection<PointData> points = new ObservableCollection<PointData>();

        public IEnumerable<PointData> Points
        {
            get { return (IEnumerable<PointData>)GetValue(PointsProperty); }
        }

        static readonly DependencyPropertyKey PointsPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "Points",
                typeof(IEnumerable<PointData>),
                typeof(PointSetPresenter),
                new PropertyMetadata());

        public static readonly DependencyProperty PointsProperty =
            PointsPropertyKey.DependencyProperty;
        
        private void CreatePoint(Point point)
        {
            var hitItem = VisualTreeHelper.HitTest(pointBoard, point).VisualHit;
            if (!(hitItem is Canvas))
                return;
            
            var pointData = new PointData(
                new Point(point.X, pointBoard.ActualHeight - point.Y),
                new Point(point.X + Constants.AXIS_SIZE, point.Y),
                points.Count);
            points.Add(pointData);
            //pointBoard.Children.Add(new PointPresenter
            //{
            //    Value = pointData
            //});

            Dispatcher.BeginInvoke((Action)UpdateConvex, DispatcherPriority.Render);
        }

        private void RemovePoints()
        {
            var selectedPoints = points//Board.Children
                //.OfType<PointPresenter>()
                .Where(item => item.IsChecked)
                .ToArray();

            if (selectedPoints.Length == 0)
                return;

            foreach (var item in selectedPoints
                .OrderByDescending(item => item.Index))
            {
                //pointBoard.Children.Remove(item);
                points.RemoveAt(item.Index);
            }

            int index = 0;
            foreach (var point in points)
                point.Index = index++;

            Dispatcher.BeginInvoke((Action)UpdateConvex, DispatcherPriority.Render);
        }

        #endregion Add/Remove Points

        const string PointBoardName = "PART_PointBoard";
        const string ConvexPathName = "PART_ConvexPath";

        private PointCanvas pointBoard;
        private Path convexPath;

        //private MinimalConvexService convexService;

        public override void OnApplyTemplate()
        {
            Dispatcher.BeginInvoke((Action)Initialize);
        }

        private void Initialize()
        {
            pointBoard = (PointCanvas)GetTemplateChild(PointBoardName);
            pointBoard.PreviewMouseLeftButtonDown += OnBoardPreviewMouseLeftButtonDown;

            convexPath = (Path)GetTemplateChild(ConvexPathName);

            //convexService = new MinimalConvexService(pointBoard.Children.OfType<PointPresenter>());

            if(!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                Window.GetWindow(this).PreviewKeyUp += OnWindowPreviewKeyUp;
        }

        private void OnWindowPreviewKeyUp(object sender, KeyEventArgs e)
        {
            RemovePoints();
        }

        private void OnBoardPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CreatePoint(e.GetPosition((UIElement)sender));
        }
        
        private void UpdateConvex()
        {
            //Geometry geometry = null;
            //var rootFigure = convexService?.BuildRootFigure();
            //if (rootFigure != null)
            //{
            //    geometry = new PathGeometry(
            //        Enumerable.Repeat(rootFigure, 1));
            //}

            //convexPath.Data = geometry;
        }

        sealed class MinimalConvexService2
        {
            public MinimalConvexService2(IEnumerable<PointPresenter> elements)
            {
                this.elements = elements;
            }

            private readonly IEnumerable<PointPresenter> elements;

            private int[] CalculateMinimalConvex(IEnumerable<PointData> points)
            {
                //calculate


                return points.Select(item => item.Index).ToArray();
            }

            public PathFigure BuildRootFigure()
            {
                var items = elements.ToArray();
                if (items.Length < 3)
                    return null;

                var indexes = CalculateMinimalConvex(items.Select(item => item.Value));
                var len = indexes?.Length ?? 0;
                if (len < 3)
                    return null;

                var figure = new PathFigure { IsClosed = true };

                BindingOperations.SetBinding(
                    figure,
                    PathFigure.StartPointProperty,
                    new Binding
                    {
                        Path = new PropertyPath(PositionExtension.SegmentPositionProperty),
                        Mode = BindingMode.OneWay,
                        Source = items[indexes[0]]
                    });

                var segments = figure.Segments;
                for (int i = 1, j = len; i < j; i++)
                {
                    var segment = new LineSegment();
                    BindingOperations.SetBinding(
                        segment,
                        LineSegment.PointProperty,
                        new Binding
                        {
                            Path = new PropertyPath(PositionExtension.SegmentPositionProperty),
                            Mode = BindingMode.OneWay,
                            Source = items[indexes[i]]
                        });
                    segments.Add(segment);
                }

                return figure;
            }
        }

        sealed class CanvasPositionValueConverter : IValueConverter
        {
            object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }

            object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}
