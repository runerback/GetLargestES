using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
                int len = source.Count();
                if (len < 4)
                    return;

                var root = Link.Create(source);

                var p_i = root.Previous;
                var p_i1 = root;
                var q_j = root;
                var q_j1 = root.Next;

                int count = len;
                while (count-- > 0)
                {
                    if (CompareToPI(
                        p_i.Data.Point,
                        p_i1.Data.Point,
                        q_j.Data.Point,
                        q_j1.Data.Point) > 0)
                    {
                        var s0 = q_j;

                        var pair = new PointPair
                        {
                            P0 = p_i.Data,
                            P1 = q_j.Data
                        };
                        Console.WriteLine(pair);

                        p_i = p_i1;
                        p_i1 = p_i1.Next;

                        count = len;
                        while (count-- > 0 && p_i != s0)
                        {
                            switch (CompareToPI(
                                p_i.Data.Point,
                                p_i1.Data.Point,
                                q_j.Data.Point,
                                q_j1.Data.Point))
                            {
                                case -1:
                                    q_j = q_j1;
                                    q_j1 = q_j1.Next;
                                    pair = new PointPair
                                    {
                                        P0 = p_i.Data,
                                        P1 = q_j.Data
                                    };
                                    Console.WriteLine(pair);
                                    break;
                                case 0:
                                    p_i = p_i1;
                                    p_i1 = p_i1.Next;
                                    break;
                                case 1:
                                    p_i = p_i1;
                                    p_i1 = p_i1.Next;
                                    pair = new PointPair
                                    {
                                        P0 = p_i.Data,
                                        P1 = q_j.Data
                                    };
                                    Console.WriteLine(pair);
                                    break;
                            }
                        }

                        break;
                    }
                    else
                    {
                        q_j = q_j1;
                        q_j1 = q_j1.Next;

                        if (q_j1 == root)
                            break;
                    }
                }
            }

            int CompareToPI(Point p0, Point p1, Point q0, Point q1)
            {
                return ((q1.X - q0.X) * (p1.Y - p0.Y)).CompareTo((p1.X - p0.X) * (q1.Y - q0.Y));
            }

            static class Link
            {
                public static Link<T> Create<T>(IEnumerable<T> source)
                {
                    return Link<T>.Create(source);
                }
            }

            sealed class Link<T>
            {
                private Link(T data)
                {
                    this.data = data;
                }
                
                private Link<T> previous;
                public Link<T> Previous => previous;

                private Link<T> next;
                public Link<T> Next => next;

                private readonly T data;
                public T Data => data;

                public static Link<T> Create(IEnumerable<T> source)
                {
                    using (var iterator = source.GetEnumerator())
                    {
                        if (!iterator.MoveNext())
                            return null;

                        var root = new Link<T>(iterator.Current);
                        var next = root;
                        while (iterator.MoveNext())
                        {
                            var _next = new Link<T>(iterator.Current);
                            next.next = _next;
                            _next.previous = next;
                            next = _next;
                        }
                        root.previous = next;
                        next.next = root;

                        return root;
                    }
                }
            }
        }
    }
}
