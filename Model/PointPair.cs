using Runerback.Utils.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GetLargestES
{
    sealed class PointPair : ViewModelBase
    {
        public PointPair()
        {
        }

        private PointData p0 = null;
        public PointData P0
        {
            get => p0;
            set
            {
                if (value != p0)
                {
                    p0 = value;
                    NotifyPropertyChanged(nameof(P0));
                    NotifyPropertyChanged(nameof(Ready));

                    if (Ready)
                    {
                        distance = (p0.Point - p1.Point).Length;
                        NotifyPropertyChanged(nameof(Distance));
                    }
                }
            }
        }

        private PointData p1 = null;
        public PointData P1
        {
            get => p1;
            set
            {
                if (value != p1)
                {
                    p1 = value;
                    NotifyPropertyChanged(nameof(P1));
                    NotifyPropertyChanged(nameof(Ready));

                    if (Ready)
                    {
                        distance = (p0.Point - p1.Point).Length;
                        NotifyPropertyChanged(nameof(Distance));
                    }
                }
            }
        }

        public bool Ready => p0 != null && p1 != null && p0 != p1;

        private double distance = -1;
        public double Distance => distance;

        public override string ToString()
        {
            var left = p0?.ToString() ?? "?";
            var right = p1?.ToString() ?? "?";
            var dist = Ready ? distance.ToString() : "?";

            return $"{left} <-> {right} ({dist})";
        }
    }
}
