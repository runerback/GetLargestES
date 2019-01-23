using Runerback.Utils.Wpf;
using System;
using System.Windows;

namespace GetLargestES
{
    sealed class PointData : ViewModelBase
    {
        private PointData() : this(new Point(-1, -1), new Point(-1, -1), -1)
        {
            isNaN = true;
        }

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
                if (isNaN)
                    throw new InvalidOperationException("Point Value Unset");

                if (value != index)
                {
                    index = value;
                    NotifyPropertyChanged(nameof(index));
                }
            }
        }

        private readonly bool isNaN = false;
        public bool IsNaN => isNaN;

        public static readonly PointData Unset = new PointData();

        private bool isChecked = false;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                if (isNaN)
                    throw new InvalidOperationException("Point Value Unset");

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
            if (isNaN)
                return "Unset";
            return $"p{index} ({point})";
        }
    }
}
