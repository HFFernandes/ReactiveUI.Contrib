using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using FluentAssertions;
using NUnit.Framework;

namespace ReactiveUI.Contrib {
    [TestFixture]
    public class ObservableHashSetCollectionViewSourceTest {
        [Test]
        public void DefaultView_Should_Support_Grouping() {
            var observableHashSet = new ObservableHashSet<int>();
            var cv = CollectionViewSource.GetDefaultView(observableHashSet);
            cv.CanGroup.Should().BeTrue();
        }

        [Test]
        public void CollectionView_Grouping_Should_Group_Existing_Items() {
            var set = new ObservableHashSet<Foo>() {
                new Foo{Category = "Dog", Name="Spot"},
                new Foo{Category = "Dog", Name = "Rover"},
                new Foo{Category = "Cat", Name = "Frisky"},
                new Foo{Category = "Cat", Name = "Leo"},
                new Foo{Category = "Dog", Name = "Leo"}
            };

            var cv = CollectionViewSource.GetDefaultView(set);
            cv.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
            ////cv.Groups.Should()
        }

        public class Foo: ReactiveObject, IEquatable<Foo> {
            public string Category { get; set; }
            public string Name { get; set; }

            private sealed class NameEqualityComparer : IEqualityComparer<Foo> {
                public bool Equals(Foo x, Foo y) {
                    if (ReferenceEquals(x, y)) return true;
                    if (ReferenceEquals(x, null)) return false;
                    if (ReferenceEquals(y, null)) return false;
                    if (x.GetType() != y.GetType()) return false;
                    return string.Equals(x.Name, y.Name);
                }

                public int GetHashCode(Foo obj) {
                    return (obj.Name != null ? obj.Name.GetHashCode() : 0);
                }
            }

            private static readonly IEqualityComparer<Foo> NameComparerInstance = new NameEqualityComparer();

            public static IEqualityComparer<Foo> NameComparer {
                get { return NameComparerInstance; }
            }

            public bool Equals(Foo other) {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return string.Equals(Name, other.Name);
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Foo) obj);
            }

            public override int GetHashCode() {
                return (Name != null ? Name.GetHashCode() : 0);
            }

            public static bool operator ==(Foo left, Foo right) {
                return Equals(left, right);
            }

            public static bool operator !=(Foo left, Foo right) {
                return !Equals(left, right);
            }
        }
    }
}
