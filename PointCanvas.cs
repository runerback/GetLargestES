using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GetLargestES
{
    sealed class PointCanvas : ItemsControl
    {
        protected override DependencyObject GetContainerForItemOverride() => 
            new PointPresenter();

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            ItemsChanged?.Invoke(this, e);
        }

        public event EventHandler ItemsChanged;

        public PointPresenter FindContainer(PointData data) =>
            (PointPresenter)ItemContainerGenerator.ContainerFromItem(data);
    }
}
