using System.Linq;
using Assets.Scripts.Core;
using NUnit.Framework;

namespace Assets.Tests
{
    public class GridTests
    {
        [Test]
        public void Stacker_Constructor_InitializesFieldsCorrectly()
        {
            // Use the Assert class to test conditions
            var grid = new Stacker(6, 12, 3);

            Assert.AreEqual(grid.Width, 6);
            Assert.AreEqual(grid.Height, 12);
            Assert.AreEqual(grid.ActiveRow, 0);
            Assert.AreEqual(grid.StackWidth, 3);
        }

        [Test]
        public void Stacker_Reset_GridIsVacant()
        {
            var grid = new Stacker(6, 12, 3);

            grid.Tick();
            grid.Tick();

            grid.ResetGrid();


            bool allVacant = true;
            for (var i = 0; i < grid.Width; i++)
            {
                for (var j = 0; j < grid.Height; j++)
                {
                    if (grid.Stack[i, j].State == State.Occupied)
                    {
                        allVacant = false;
                    }
                }
            }

            Assert.IsTrue(allVacant);
        }

        [Test]
        public void Stacker_Tick_MovesStack()
        {
            var grid = new Stacker(6, 12, 3);

            grid.Tick();
            grid.Tick();

            Assert.AreEqual(State.Vacant, grid.Stack[0, 0].State);
            Assert.AreEqual(State.Occupied, grid.Stack[1, 0].State);
            Assert.AreEqual(State.Occupied, grid.Stack[2, 0].State);
            Assert.AreEqual(State.Occupied, grid.Stack[3, 0].State);
            Assert.AreEqual(State.Vacant, grid.Stack[4, 0].State);
        }

        [Test]
        public void Stacker_Tick_ChangesDirection()
        {
            var grid = new Stacker(6, 12, 3);

            grid.Tick();
            grid.Tick();
            grid.Tick();
            grid.Tick();

            Assert.AreEqual(State.Vacant, grid.Stack[2, 0].State);
            Assert.AreEqual(State.Occupied, grid.Stack[3, 0].State);
            Assert.AreEqual(State.Occupied, grid.Stack[4, 0].State);
            Assert.AreEqual(State.Occupied, grid.Stack[5, 0].State);
            Assert.AreEqual(grid.MoveDir, MovementDirection.Right);

            grid.Tick();

            Assert.AreEqual(State.Vacant, grid.Stack[2, 0].State);
            Assert.AreEqual(State.Occupied, grid.Stack[3, 0].State);
            Assert.AreEqual(State.Occupied, grid.Stack[4, 0].State);
            Assert.AreEqual(State.Occupied, grid.Stack[5, 0].State);
            Assert.AreEqual(grid.MoveDir, MovementDirection.Left);

            grid.Tick();

            Assert.AreEqual(State.Vacant, grid.Stack[1, 0].State);
            Assert.AreEqual(State.Occupied, grid.Stack[2, 0].State);
            Assert.AreEqual(State.Occupied, grid.Stack[3, 0].State);
            Assert.AreEqual(State.Occupied, grid.Stack[4, 0].State);
            Assert.AreEqual(State.Vacant, grid.Stack[5, 0].State);

            grid.Tick();
            grid.Tick();

            Assert.AreEqual(State.Occupied, grid.Stack[0, 0].State);
            Assert.AreEqual(State.Occupied, grid.Stack[1, 0].State);
            Assert.AreEqual(State.Occupied, grid.Stack[2, 0].State);
            Assert.AreEqual(State.Vacant, grid.Stack[3, 0].State);
            Assert.AreEqual(grid.MoveDir, MovementDirection.Left);

            grid.Tick();

            Assert.AreEqual(State.Occupied, grid.Stack[0, 0].State);
            Assert.AreEqual(State.Occupied, grid.Stack[1, 0].State);
            Assert.AreEqual(State.Occupied, grid.Stack[2, 0].State);
            Assert.AreEqual(State.Vacant, grid.Stack[3, 0].State);
            Assert.AreEqual(grid.MoveDir, MovementDirection.Right);
        }

        [Test]
        public void Stacker_Tick_StartsFirstRow()
        {
            var grid = new Stacker(6, 12, 3);

            grid.Tick();

            Assert.AreEqual(State.Occupied, grid.Stack[0, 0].State);
            Assert.AreEqual(State.Occupied, grid.Stack[1, 0].State);
            Assert.AreEqual(State.Occupied, grid.Stack[2, 0].State);
            Assert.AreEqual(State.Vacant, grid.Stack[3, 0].State);
        }

        [Test]
        public void Stacker_Tick_StartsSecondRow()
        {
            var grid = new Stacker(6, 12, 3);

            grid.Tick();
            grid.Place();
            grid.Tick();

            Assert.AreEqual(State.Occupied, grid.Stack[0, 1].State);
            Assert.AreEqual(State.Occupied, grid.Stack[1, 1].State);
            Assert.AreEqual(State.Occupied, grid.Stack[2, 1].State);
            Assert.AreEqual(State.Vacant, grid.Stack[3, 1].State);
        }

        [Test]
        public void Stacker_Tick_PlacesReducedStackWidth()
        {
            var grid = new Stacker(6, 12, 3);

            grid.Tick();
            grid.Place();
            grid.Tick();
            grid.Tick();
            grid.Place();

            Assert.AreEqual(State.Vacant, grid.Stack[0, 1].State);
            Assert.AreEqual(State.Occupied, grid.Stack[1, 1].State);
            Assert.AreEqual(State.Occupied, grid.Stack[2, 1].State);
            Assert.AreEqual(State.Vacant, grid.Stack[3, 1].State);

            grid.Tick();

            Assert.AreEqual(State.Vacant, grid.Stack[0, 2].State);
            Assert.AreEqual(State.Occupied, grid.Stack[1, 2].State);
            Assert.AreEqual(State.Occupied, grid.Stack[2, 2].State);
            Assert.AreEqual(State.Vacant, grid.Stack[3, 2].State);
        }

        [Test]
        public void Stacker_ResetHeight_MovesStackDown()
        {
            var grid = new Stacker(6, 12, 3);
            grid.Tick();
            grid.Place();
            grid.Tick();
            grid.Place();
            grid.Tick();
            grid.Place();
            grid.Tick();
            grid.Place();

            grid.ResetHeight();

            Assert.AreEqual(1, grid.ActiveRow);
            Assert.AreEqual(State.Occupied, grid.Stack[0, 0].State);
            Assert.AreEqual(State.Occupied, grid.Stack[1, 0].State);
            Assert.AreEqual(State.Occupied, grid.Stack[2, 0].State);
            Assert.AreEqual(State.Vacant, grid.Stack[3, 0].State);

            Assert.AreEqual(State.Vacant, grid.Stack[0, 1].State);
            Assert.AreEqual(State.Vacant, grid.Stack[1, 1].State);
            Assert.AreEqual(State.Vacant, grid.Stack[2, 1].State);
            Assert.AreEqual(State.Vacant, grid.Stack[3, 1].State);
        }

        [Test]
        public void Stacker_Place_FirstRow()
        {
            var grid = new Stacker(6, 12, 3);
            grid.Tick();

            var result = grid.Place();

            Assert.IsFalse(result.Any());
        }

        [Test]
        public void Stacker_Place_NoMisses()
        {
            var grid = new Stacker(6, 12, 3);
            grid.Tick();
            grid.Place();
            grid.Tick();

            var result = grid.Place();

            Assert.IsFalse(result.Any());
        }

        [Test]
        public void Stacker_Place_OneMissed()
        {
            var grid = new Stacker(6, 12, 3);
            grid.Tick();
            grid.Place();
            grid.Tick();
            grid.Tick();

            var result = grid.Place();

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void Stacker_Place_TwoMissed()
        {
            var grid = new Stacker(6, 12, 3);
            grid.Tick();
            grid.Place();
            grid.Tick();
            grid.Tick();
            grid.Tick();

            var result = grid.Place();

            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void Stacker_Place_ThreeMissed()
        {
            var grid = new Stacker(6, 12, 3);
            grid.Tick();
            grid.Place();
            grid.Tick();
            grid.Tick();
            grid.Tick();
            grid.Tick();

            var result = grid.Place();

            Assert.AreEqual(3, result.Count);
        }
    }
}
