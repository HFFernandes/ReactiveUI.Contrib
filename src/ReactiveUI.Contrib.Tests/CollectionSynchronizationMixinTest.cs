using System.Collections.Generic;
using System.Collections.ObjectModel;
using FluentAssertions;
using NUnit.Framework;

namespace ReactiveUI.Contrib {
    using System.Linq;

    [TestFixture]
    public class CollectionSynchronizationMixinTest {
        [Test]
        public void Items_From_Source_Are_Added() {
            var target = new List<int>();
            var source = new List<int>() {1, 2, 3, 4, 5};

            target.Mirror(source);
            target.Should().Equal(source);
        }

        [Test]
        public void Items_Added_To_Source_After_Mirror_Call_Are_Added_To_Target() {
            var target = new List<int>();
            var source = new ObservableCollection<int> { 1, 2, 3, 4, 5 };

            target.Mirror(source);
            source.Add(6);
            source.Add(7);

            target.Should().Equal(source);
        }

        [Test]
        public void Items_Removed_From_Source_After_Mirror_Call_Are_Removed_From_Target() {
            var target = new List<int>();
            var source = new ObservableCollection<int> { 1, 2, 3, 4, 5 };

            target.Mirror(source);
            source.Remove(1);
            source.Remove(3);
            source.Remove(5);

            target.Should().Equal(source);
        }

        [Test]
        public void Clearing_Source_Collection_Clears_Target_After_Mirror_Call() {
            var target = new List<int>();
            var source = new ObservableCollection<int> { 1, 2, 3, 4, 5 };

            target.Mirror(source);
            source.Clear();

            target.Should().Equal(source);
        }

        [Test]
        public void Filtered_Items_Are_Not_Added_Initially()
        {
            var target = new List<int>();
            var source = new ObservableCollection<int> { 1, 2, 3, 4, 5 };

            target.Mirror(source, x => x % 2 == 0);

            target.Should().Equal(new[] { 2, 4 });
        }

        [Test]
        public void Filtered_Items_Added_To_Source_After_Mirror_Call_Are_Not_Added_To_Target()
        {
            var target = new List<int>();
            var source = new ObservableCollection<int> { 1, 2, 3, 4, 5 };

            target.Mirror(source, x => x % 2 == 0);
            source.Add(6);
            source.Add(7);
            source.Add(8);
            source.Add(9);

            target.Should().Equal(new[] { 2, 4, 6, 8 });
        }

        [Test]
        public void Items_Are_Ordered_According_To_Orderer()
        {
            var target = new List<int>();
            var source = new ObservableCollection<int> { 1, 2, 3, 4, 5 };

            // This will sort the items descending
            target.Mirror(source, x => x, orderer: (x, y) => Comparer<int>.Default.Compare(x, y) * -1);
            target.Should().Equal(source.OrderByDescending(x => x));
        }

        [Test]
        public void Items_Are_Ordered_According_To_Orderer_When_Added_After_Mirror_Call()
        {
            var target = new List<int>();
            var source = new ObservableCollection<int> { 1, 2, 3, 4, 5 };

            // This will sort the items descending
            target.Mirror(source, x => x, orderer: (x, y) => Comparer<int>.Default.Compare(x, y) * -1);

            source.Add(0);
            source.Add(-1);

            target.Should().Equal(source.OrderByDescending(x => x));
        }
    }
}