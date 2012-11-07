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

        public static void Mirror<TTarget, TSource>(
            this IList<TTarget> target, 
            IEnumerable<TSource> source, 
            Func<TSource,TTarget> selector) {
            if (target == null) throw new ArgumentNullException("target");

            var sourceCollectionChanged = new Subject<NotifyCollectionChangedEventArgs>();

            if (selector == null) {
                selector = (x => (TTarget)Convert.ChangeType(x, typeof(TTarget), CultureInfo.CurrentCulture));
            }

            var originalSource = source;
            var modifiedSource = originalSource.Select(selector);

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
                }

                int oldIndex = (args.Action == NotifyCollectionChangedAction.Replace ?
                    args.NewStartingIndex : args.OldStartingIndex);


                if (args.OldItems != null) {
                    foreach (TSource item in args.OldItems) {
                        target.RemoveAt(oldIndex);
                    }
                }

                if (args.NewItems != null) {
                    foreach (TSource item in args.NewItems) {
                        var toAdd = selector(item);
                        target.Add(toAdd);
                    }
                }                
            });
        }
    }
}