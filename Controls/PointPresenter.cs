using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace GetLargestES
{
    sealed class PointPresenter : Control
    {
        public PointPresenter()
        {

        }

        #region Value

        public PointData Value
        {
            get { return (PointData)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(PointData),
                typeof(PointPresenter),
                new PropertyMetadata(OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        #endregion Value

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
                typeof(PointPresenter),
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
        
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            Value?.ToggleCheckedState();
        }
    }
}
