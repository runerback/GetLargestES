using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GetLargestES
{
    sealed class PositionExtension : DependencyObject
    {
        public static Point GetPointPosition(UIElement d)
        {
            return (Point)d.GetValue(PointPositionProperty);
        }

        public static void SetPointPosition(UIElement d, Point value)
        {
            d.SetValue(PointPositionProperty, value);
        }

        public static readonly DependencyProperty PointPositionProperty =
            DependencyProperty.RegisterAttached(
                "PointPosition",
                typeof(Point),
                typeof(PositionExtension),
                new PropertyMetadata(OnPointPositionPropertyChanged));

        private static void OnPointPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Task.Delay(1000).ContinueWith(t =>
            {
                var target = (UIElement)d;
                var pp = GetPointPosition(target);

                Canvas.SetLeft(target, pp.X);
                Canvas.SetTop(target, pp.Y);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public static Point GetSegmentPosition(UIElement d)
        {
            return (Point)d.GetValue(SegmentPositionProperty);
        }

        public static void SetSegmentPosition(UIElement d, Point value)
        {
            d.SetValue(SegmentPositionProperty, value);
        }

        public static readonly DependencyProperty SegmentPositionProperty =
            DependencyProperty.RegisterAttached(
                "SegmentPosition",
                typeof(Point),
                typeof(PositionExtension));
    }

    sealed class PointPositionValueConverter : IMultiValueConverter
    {
        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 3)
                return default(Point);
            if (!(values[0] is Point point))
                return default(Point);
            if (!(values[1] is double scale))
                return default(Point);
            if (!(values[2] is double height))
                return default(Point);

            return new Point(point.X * scale - Constants.AXIS_SIZE, (point.Y - height / 2) * scale);
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("one-way");
        }
    }

    sealed class SegmentPositionValueConverter : IMultiValueConverter
    {
        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return default(Point);
            if (!(values[0] is Point point))
                return default(Point);
            if (!(values[1] is double scale))
                return default(Point);

            return new Point((point.X + Constants.AXIS_SIZE / 2) * scale, point.Y * scale);
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("one-way");
        }
    }
}
