using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interactivity;

namespace GetLargestES
{
    sealed class FarthestPairService : Behavior<PointCanvas>
    {
        private FarthestPairSearcher searcher;

        protected override void OnAttached()
        {
            var source = AssociatedObject;
            var convexHull = ConvexHullService.GetConvexHull(source);
            if (convexHull is INotifyCollectionChanged notifySource)
                notifySource.CollectionChanged += OnConvexHullChanged;
            searcher = new FarthestPairSearcher(convexHull);
        }

        protected override void OnDetaching()
        {
            var source = AssociatedObject;
            var convexHull = ConvexHullService.GetConvexHull(source);
            if (convexHull is INotifyCollectionChanged notifySource)
                notifySource.CollectionChanged -= OnConvexHullChanged;
            searcher = null;
        }

        private void OnConvexHullChanged(object sender, EventArgs e)
        {
            searcher.Search();
        }

        sealed class FarthestPairSearcher
        {
            public FarthestPairSearcher(IEnumerable<PointData> source)
            {
                this.source = source ?? throw new ArgumentNullException(nameof(source));
            }

            private readonly IEnumerable<PointData> source;

            public void Search()
            {
                Console.WriteLine("NotImplemented");
            }
        }
    }
}
