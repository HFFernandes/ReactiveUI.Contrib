using System.Collections.Generic;
using System.Collections.ObjectModel;
using FluentAssertions;
using NUnit.Framework;

namespace ReactiveUI.Contrib {
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
    }
}