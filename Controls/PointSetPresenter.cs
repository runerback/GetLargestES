using Runerback.Utils.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
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
    sealed class PointSetPresenter : Control
    {
        public PointSetPresenter()
        {
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
        
    }
}
