using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ReactiveUI.Contrib {
    public static class CollectionSynchronizationMixin {
        public static void Mirror<T>(this IList<T> target, IEnumerable<T> source) {
            target.Mirror(source, x => x);
        }

        public static void Mirror<T>(this IList<T> target, IEnumerable<T> source, Func<T, bool> filter) {            
            target.Mirror(source, x => x, filter);
        }

        public static void Mirror<TTarget, TSource>(
            this IList<TTarget> target, 
            IEnumerable<TSource> source, 
            Func<TSource, TTarget> selector,            
            Func<TSource, bool> filter = null,
            Func<TTarget, TTarget, int> orderer = null)
        {
            if (target == null) throw new ArgumentNullException("target");

            var sourceCollectionChanged = new Subject<NotifyCollectionChangedEventArgs>();

            if (selector == null) {
                selector = (x => (TTarget)Convert.ChangeType(x, typeof(TTarget), CultureInfo.CurrentCulture));
            }

            var originalSource = source;
            originalSource = (filter != null ? originalSource.Where(filter) : originalSource);
            var modifiedSource = originalSource.Select(selector);
            modifiedSource = (orderer != null ? modifiedSource.OrderBy(x => x, new FuncComparator<TTarget>(orderer)) : modifiedSource);

            foreach (var item in modifiedSource) {
                target.Add(item);
            }

            var incc = source as INotifyCollectionChanged;
            if (incc != null) {
                ((INotifyCollectionChanged)source).CollectionChanged += (o, e) => sourceCollectionChanged.OnNext(e);
            }

            sourceCollectionChanged.Subscribe(args => {

                if (args.Action == NotifyCollectionChangedAction.Reset) {
                    target.Clear();
                    modifiedSource.ToObservable().Subscribe(target.Add);
                }

                int oldIndex = (args.Action == NotifyCollectionChangedAction.Replace ?
                    args.NewStartingIndex : args.OldStartingIndex);


                if (args.OldItems != null) {    
                    foreach (TSource item in args.OldItems) {
                        if (filter != null && !filter(item))
                        {
                            continue;
                        }

                        if (orderer == null)
                        {
                            target.RemoveAt(oldIndex);
                            continue;
                        }

                        for (int i = 0; i < target.Count; i++)
                        {
                            if (orderer(target[i], selector(item)) == 0)
                            {
                                target.RemoveAt(i);
                            }
                        }
                    }
                }

                if (args.NewItems != null) {
                    foreach (TSource item in args.NewItems) {
                        if (filter != null && !filter(item))
                        {
                            continue;
                        }

                        if (orderer == null) {
                            var newIndex = Math.Min(args.NewStartingIndex, target.Count);
                            target.Insert(newIndex, selector(item));
                            continue;
                        }

                        var toAdd = selector(item);
                        target.Insert(PositionForNewItem(target, toAdd, orderer), toAdd);
                    }
                }                
            });
        }

        private static int PositionForNewItem<T>(IList<T> list, T item, Func<T, T, int> orderer)
        {
            if (list.Count == 0)
            {
                return 0;
            }
            if (list.Count == 1)
            {
                return orderer(list[0], item) >= 0 ? 1 : 0;
            }

            // NB: This is the most tart way to do this possible
            int? prevCmp = null;
            int cmp;
            for (int i = 0; i < list.Count; i++)
            {
                cmp = orderer(list[i], item);
                if (prevCmp.HasValue && cmp != prevCmp)
                {
                    return i;
                }
                prevCmp = cmp;
            }

            return list.Count;
        }

        private class FuncComparator<T> : IComparer<T>
        {
            Func<T, T, int> _inner;

            public FuncComparator(Func<T, T, int> comparer)
            {
                _inner = comparer;
            }

            public int Compare(T x, T y)
            {
                return _inner(x, y);
            }
        }
    }
}