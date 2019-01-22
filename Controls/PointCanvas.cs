using Runerback.Utils.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GetLargestES
{
    sealed class PointCanvas : ItemsControl, IPointManager
    {
        public PointCanvas()
        {
            pointManager = new PointManager(this, nextPointSubject);
            ItemsSource = pointManager.Points;
        }

        protected override DependencyObject GetContainerForItemOverride() =>
            new PointPresenter();

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            ItemsChanged?.Invoke(this, e);
        }

        public event EventHandler ItemsChanged;

        public UIElement FindContainer(PointData data) =>
            (UIElement)ItemContainerGenerator.ContainerFromItem(data);

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            RaisePointReceived(e.GetPosition(this));
        }

        private void RaisePointReceived(Point point)
        {
            var hitItem = VisualTreeHelper.HitTest(this, point).VisualHit;
            if (!(hitItem is Canvas))
                return;

            nextPointSubject.OnNext(point);
        }

        private readonly Subject<Point> nextPointSubject = new Subject<Point>();
        private readonly PointManager pointManager;

        #region IPointManager

        public IEnumerable<PointData> Points => pointManager.Points;

        void IShortcutCommandSource.RemoveSelection()
        {
            Dispatcher.Invoke(pointManager.RemoveSelectedPoints);
        }

        void IShortcutCommandSource.SelectAll()
        {
            Dispatcher.Invoke(pointManager.SelectAllPoints);
        }

        void IShortcutCommandSource.Reset()
        {
            Dispatcher.Invoke(pointManager.Reset);
        }

        #endregion IPointManager
        
        sealed class PointManager
        {
            public PointManager(FrameworkElement host, IObservable<Point> pointSource)
            {
                this.host = host ?? throw new ArgumentNullException(nameof(host));
                (pointSource ?? throw new ArgumentNullException(nameof(pointSource)))
                    .Subscribe(OnNewPointReceived);
            }

            private readonly AutoInvokeObservableCollection<PointData> points = 
                new AutoInvokeObservableCollection<PointData>();
            public IEnumerable<PointData> Points => points;

            private readonly FrameworkElement host;

            private void OnNewPointReceived(Point point)
            {
                CreatePoint(point);
            }

            private void CreatePoint(Point point)
            {
                var pointData = new PointData(
                    new Point(point.X, host.ActualHeight - point.Y),
                    new Point(point.X + Constants.AXIS_SIZE, point.Y),
                    points.Count);
                points.Add(pointData);
            }

            public void RemoveSelectedPoints()
            {
                var selectedPoints = points
                    .Where(item => item.IsChecked)
                    .OrderByDescending(item => item.Index)
                    .ToArray();

                if (selectedPoints.Length == 0)
                    return;

                foreach (var item in selectedPoints)
                    points.RemoveAt(item.Index);

                for (int i = 0, j = points.Count; i < j; i++)
                    points[i].Index = i;
            }

            public void SelectAllPoints()
            {
                foreach (var item in points)
                    item.IsChecked = true;
            }

            public void Reset()
            {
                points.Reset(Enumerable.Empty<PointData>());
            }
        }
    }
}
