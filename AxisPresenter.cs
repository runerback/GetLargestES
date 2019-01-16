using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GetLargestES
{
    sealed class AxisPresenter : Control
    {
        public AxisPresenter()
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
                typeof(AxisPresenter),
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

        #region Direction

        public Orientation Direction
        {
            get { return (Orientation)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register(
                "Direction",
                typeof(Orientation),
                typeof(AxisPresenter),
                new PropertyMetadata(Orientation.Horizontal));

        #endregion Direction

    }
}
