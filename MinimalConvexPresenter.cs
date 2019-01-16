using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GetLargestES
{
    sealed class MinimalConvexPresenter : Control
    {
        public MinimalConvexPresenter()
        {
            var a = ConvexData;
        }

        #region Points

        public IEnumerable<PointData> Points
        {
            get { return (IEnumerable<PointData>)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register(
                "Points",
                typeof(IEnumerable<PointData>),
                typeof(MinimalConvexPresenter),
                new PropertyMetadata(null, OnPointsPropertyChanged));

        private static void OnPointsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MinimalConvexPresenter)d).UpdatePointsBinding(
                e.OldValue as INotifyCollectionChanged,
                e.NewValue as INotifyCollectionChanged);
        }

        private void UpdatePointsBinding(INotifyCollectionChanged oldValue, INotifyCollectionChanged newValue)
        {
            if (oldValue != null)
                oldValue.CollectionChanged -= OnPointsCollectionChanged;
            if (newValue != null)
                newValue.CollectionChanged += OnPointsCollectionChanged;
        }

        #endregion Points
        
        #region PointItems

        public IEnumerable PointItems
        {
            get { return (IEnumerable)GetValue(PointItemsProperty); }
            set { SetValue(PointItemsProperty, value); }
        }

        public static readonly DependencyProperty PointItemsProperty =
            DependencyProperty.Register(
                "PointItems",
                typeof(IEnumerable),
                typeof(MinimalConvexPresenter));

        #endregion PointItems

        #region ConvexData

        public Path ConvexData
        {
            get { return (Path)GetValue(ConvexDataProperty); }
        }

        static readonly DependencyPropertyKey ConvexDataPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "ConvexData",
                typeof(Path),
                typeof(MinimalConvexPresenter),
                new PropertyMetadata(new Path()));

        public static readonly DependencyProperty ConvexDataProperty =
            ConvexDataPropertyKey.DependencyProperty;

        #endregion ConvexData

        private void OnPointsCollectionChanged(object sender, EventArgs e)
        {


            var items = CalculateMinimalConvex();
            if ((items?.Length ?? 0) < 3)
                return;


        }

        private PointPresenter[] CalculateMinimalConvex()
        {
            return PointItems?.OfType<PointPresenter>().ToArray();
        }
    }
}
