using Runerback.Utils.Wpf;
using System.Windows;

namespace GetLargestES
{
    sealed class PointData : ViewModelBase
    {
        public PointData(Point point, Point uiPoint, int index)
        {
            this.point = point;
            this.uiPoint = uiPoint;
            this.index = index;
        }

        private readonly Point point;
        public Point Point => point;

        private readonly Point uiPoint;
        public Point UIPoint => uiPoint;

        private int index;
        public int Index
        {
            get => index;
            set
            {
                if (value != index)
                {
                    index = value;
                    NotifyPropertyChanged(nameof(index));
                }
            }
        }

        private bool isChecked = false;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                if (value != isChecked)
                {
                    isChecked = value;
                    NotifyPropertyChanged(nameof(IsChecked));
                }
            }
        }

        public void ToggleCheckedState()
        {
            IsChecked = !isChecked;
        }

        public override string ToString()
        {
            return $"p{index} ({point})";
        }
    }
}
