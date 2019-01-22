using System.Windows;
using System.Windows.Controls;

namespace GetLargestES
{
    sealed class PointDataPresenter : Control
    {
        public PointDataPresenter()
        {

        }

        public PointData Data
        {
            get { return (PointData)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(
                "Data",
                typeof(PointData),
                typeof(PointDataPresenter));
    }
}
