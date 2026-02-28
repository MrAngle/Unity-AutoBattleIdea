using System.Collections.Generic;
using System.Linq;
using MageFactory.Shared.Contract;
using MageFactory.Shared.ItemSearch;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using NUnit.Framework;
using UnityEngine;

namespace MageFactory.Tests.Unit.Shared.ItemSearch {
    public class GridAdjacencySearchTests {
        [Test]
        public void shouldReturnSingleNeighborOnRight() {
            // given
            var source = new DummyGridItem(1, new[] { Vector2Int.zero });

            var neighbor = new DummyGridItem(2, new[] { GridDirection.Right.toVector2Int() });

            var cellIndex = new Dictionary<Vector2Int, DummyGridItem> {
                { Vector2Int.zero, source },
                { GridDirection.Right.toVector2Int(), neighbor }
            };

            var directions = new[] { GridDirection.Right };

            // when
            var result = GridAdjacencySearch
                .getNeighborItems(source, cellIndex, directions)
                .ToArray();

            // then
            Assert.That(result.Length, Is.EqualTo(1));
            Assert.That(result[0].getId(), Is.EqualTo(2));
        }

        [Test]
        public void shouldReturnEmptyWhenNoNeighborsExist() {
            // given
            var source = new DummyGridItem(1, new[] { Vector2Int.zero });

            var cellIndex = new Dictionary<Vector2Int, DummyGridItem> {
                { Vector2Int.zero, source }
            };

            var directions = GridDirectionSets.AllExcludeNone;

            // when
            var result = GridAdjacencySearch
                .getNeighborItems(source, cellIndex, directions)
                .ToArray();

            // then
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void shouldRespectDirectionFilter() {
            // given
            var source = new DummyGridItem(1, new[] { Vector2Int.zero });

            var neighborUp = new DummyGridItem(2, new[] { GridDirection.Up.toVector2Int() });

            var neighborLeft = new DummyGridItem(3, new[] { GridDirection.Left.toVector2Int() });

            var cellIndex = new Dictionary<Vector2Int, DummyGridItem> {
                { Vector2Int.zero, source },
                { GridDirection.Up.toVector2Int(), neighborUp },
                { GridDirection.Left.toVector2Int(), neighborLeft }
            };

            var directions = new[] { GridDirection.Right };

            // when
            var result = GridAdjacencySearch
                .getNeighborItems(source, cellIndex, directions)
                .ToArray();

            // then
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void shouldReturnMultipleNeighborsWhenPresentInDifferentDirections() {
            // given
            var source = new DummyGridItem(1, new[] { Vector2Int.zero });

            var neighborUp = new DummyGridItem(2, new[] { GridDirection.Up.toVector2Int() });

            var neighborRight = new DummyGridItem(3, new[] { GridDirection.Right.toVector2Int() });

            var cellIndex = new Dictionary<Vector2Int, DummyGridItem> {
                { Vector2Int.zero, source },
                { GridDirection.Up.toVector2Int(), neighborUp },
                { GridDirection.Right.toVector2Int(), neighborRight }
            };

            var directions = new[] { GridDirection.Up, GridDirection.Right };

            // when
            var result = GridAdjacencySearch
                .getNeighborItems(source, cellIndex, directions)
                .ToArray();

            // then
            var ids = result.Select(x => x.getId()).ToArray();
            Assert.That(ids, Is.EquivalentTo(new[] { 2L, 3L }));
        }

        [Test]
        public void shouldNotReturnDuplicatesForMultiCellItems() {
            // given
            var source = new DummyGridItem(1, new[] { Vector2Int.zero, GridDirection.Up.toVector2Int() });

            var neighbor = new DummyGridItem(
                2,
                new[] {
                    GridDirection.Right.toVector2Int(),
                    GridDirection.UpRight.toVector2Int()
                });

            var cellIndex = new Dictionary<Vector2Int, DummyGridItem> {
                { Vector2Int.zero, source },
                { GridDirection.Up.toVector2Int(), source },
                { GridDirection.Right.toVector2Int(), neighbor },
                { GridDirection.UpRight.toVector2Int(), neighbor }
            };

            var directions = new[] { GridDirection.Right };

            // when
            var result = GridAdjacencySearch
                .getNeighborItems(source, cellIndex, directions)
                .ToArray();

            // then
            Assert.That(result.Length, Is.EqualTo(1));
            Assert.That(result[0].getId(), Is.EqualTo(2));
        }

        [Test]
        public void shouldNotReturnSourceWhenDirectionIsNone() {
            // given
            var source = new DummyGridItem(
                1,
                new[] { Vector2Int.zero });

            var cellIndex = new Dictionary<Vector2Int, DummyGridItem> {
                { Vector2Int.zero, source }
            };

            var directions = new[] { GridDirection.None };

            // when
            var result = GridAdjacencySearch
                .getNeighborItems(source, cellIndex, directions)
                .ToArray();

            // then
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void shouldReturnEmptyWhenNeighborIsNotDirectlyAdjacent() {
            // given
            var source = new DummyGridItem(
                1,
                new[] { Vector2Int.zero });

            var neighbor = new DummyGridItem(
                2,
                new[] { GridDirection.Right.toVector2Int() * 2 });

            var cellIndex = new Dictionary<Vector2Int, DummyGridItem> {
                { Vector2Int.zero, source },
                { GridDirection.Right.toVector2Int() * 2, neighbor }
            };

            var directions = GridDirectionSets.Orthogonal;

            // when
            var result = GridAdjacencySearch
                .getNeighborItems(source, cellIndex, directions)
                .ToArray();

            // then
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void shouldFindAllNeighborsForLargeMultiCellItem() {
            // given
            // duży obiekt 3x2:
            // (0,1) (1,1) (2,1)
            // (0,0) (1,0) (2,0)
            var source = new DummyGridItem(
                1,
                new[] {
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                    new Vector2Int(2, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(1, 1),
                    new Vector2Int(2, 1)
                });

            // sąsiad z lewej, dotyka dwóch komórek źródła
            var leftNeighbor = new DummyGridItem(
                2,
                new[] {
                    new Vector2Int(-1, 0),
                    new Vector2Int(-1, 1)
                });

            var rightNeighbor = new DummyGridItem(
                3,
                new[] {
                    new Vector2Int(3, 0),
                    new Vector2Int(3, 1)
                });

            var topNeighbor = new DummyGridItem(
                4,
                new[] {
                    new Vector2Int(1, 2)
                });

            var bottomNeighbor = new DummyGridItem(
                5,
                new[] {
                    new Vector2Int(1, -1)
                });

            var cellIndex = new Dictionary<Vector2Int, DummyGridItem> {
                // źródło
                { new Vector2Int(0, 0), source },
                { new Vector2Int(1, 0), source },
                { new Vector2Int(2, 0), source },
                { new Vector2Int(0, 1), source },
                { new Vector2Int(1, 1), source },
                { new Vector2Int(2, 1), source },

                // lewy sąsiad
                { new Vector2Int(-1, 0), leftNeighbor },
                { new Vector2Int(-1, 1), leftNeighbor },

                // prawy sąsiad
                { new Vector2Int(3, 0), rightNeighbor },
                { new Vector2Int(3, 1), rightNeighbor },

                // górny sąsiad
                { new Vector2Int(1, 2), topNeighbor },

                // dolny sąsiad
                { new Vector2Int(1, -1), bottomNeighbor }
            };

            var directions = GridDirectionSets.Orthogonal;

            // when
            var result = GridAdjacencySearch
                .getNeighborItems(source, cellIndex, directions)
                .ToArray();

            // then
            var ids = result.Select(x => x.getId()).ToArray();

            Assert.That(ids.Length, Is.EqualTo(4));
            Assert.That(ids, Is.EquivalentTo(new[] { 2L, 3L, 4L, 5L }));
        }
    }

    internal class DummyGridItem : IGridItemPlaced {
        private readonly IReadOnlyCollection<Vector2Int> _cells;
        private readonly long _id;

        public DummyGridItem(long id, IEnumerable<Vector2Int> cells) {
            _id = id;
            _cells = cells.ToArray();
        }

        public long getId() {
            return _id;
        }

        public Vector2Int getOrigin() {
            return _cells.First();
        }

        public IReadOnlyCollection<Vector2Int> getOccupiedCells() {
            return _cells;
        }

        public ShapeArchetype getShape() {
            return null;
        }
    }
}