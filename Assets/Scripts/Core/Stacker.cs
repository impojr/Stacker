using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Core
{
    public enum MovementDirection
    {
        Left,
        Right
    }

    public class Stacker : IStacker
    {
        /// <summary>
        /// The width of the grid. <b>Not</b> the width of the moving stack.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// The height of the grid.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// The playing grid. 
        /// </summary>
        public Square[,] Stack { get; private set; }

        /// <summary>
        /// The move direction of the stack.
        /// </summary>
        public MovementDirection MoveDir { get; private set; }

        /// <summary>
        /// The row in the grid where the stack is currently moving.
        /// </summary>
        public int ActiveRow { get; private set; }

        /// <summary>
        /// The width of the moving (current) stack.
        /// </summary>
        public int StackWidth { get; private set; }

        public Stacker(int width, int height, int initialStackWidth)
        {
            Width = width;
            Height = height;
            StackWidth = initialStackWidth;
            ResetGrid();
        }

        /// <summary>
        /// Resets the grid to empty.
        /// </summary>
        public void ResetGrid()
        {
            Stack = new Square[Width, Height];
            ActiveRow = 0;
            MoveDir = MovementDirection.Right;

            for (var i = 0; i < Width; i++)
            {  
                for (var j = 0; j < Height; j++)
                {
                    Stack[i, j] = new Square();
                }
            }
        }

        /// <summary>
        /// Move the stack in the grid.
        /// This will allow move rows if the grid was placed beforehand.
        /// </summary>
        public void Tick()
        {
            var cellsToSwitch = new List<int>();

            for (var i = 0; i < Width; i++)
            {
                if (Stack[i, ActiveRow].State == State.Occupied)
                {
                    cellsToSwitch.Add(i);
                }
            }

            // If there are no cells to switch, then we are starting a new row
            if (!cellsToSwitch.Any())
            {
                StartNewRow();
            }
            else
            {
                MoveStackInRow(cellsToSwitch);
            }
        }

        private void StartNewRow()
        {
            // If there is no active row, it is the start of the game
            // Start from left
            if (ActiveRow == 0)
            {
                FillRow(0);
            }
            // Otherwise, start row from previous stack
            else
            {
                var previousRow = ActiveRow - 1;
                var nextRowStart = 0;
                for (var i = 0; i < Width; i++)
                {
                    if (Stack[i, previousRow].State == State.Occupied)
                    {
                        nextRowStart = i;
                        break;
                    }
                }

                FillRow(nextRowStart);
            }
        }

        private void MoveStackInRow(List<int> cellsToSwitch)
        {
            // If we are going left and the first item in list is at the beginning of the row,
            // Do nothing and change movement direction
            if (MoveDir == MovementDirection.Left)
            {
                if (cellsToSwitch.First() == 0)
                {
                    MoveDir = MovementDirection.Right;
                    return;
                }

                MoveStack(cellsToSwitch, -1);
            }
            else
            {
                // If we are going right and the last item in list at the end of the row,
                // Do nothing and change movement direction
                if (cellsToSwitch.Last() == Width - 1)
                {
                    MoveDir = MovementDirection.Left;
                    return;
                }

                MoveStack(cellsToSwitch, 1);
            }
        }

        private void MoveStack(IList<int> cellsToSwitch, int increment)
        {
            for (var i = 0; i < cellsToSwitch.Count; i++)
            {
                cellsToSwitch[i] += increment;
            }

            for (var i = 0; i < Width; i++)
            {
                Stack[i, ActiveRow].State = cellsToSwitch.Contains(i) ? State.Occupied : State.Vacant;
            }
        }

        private void FillRow(int startIndex)
        {
            for (var i = 0; i < StackWidth; i++)
            {
                Stack[i + startIndex, ActiveRow].State = State.Occupied;
            }
        }

        /// <summary>
        /// Places the stack and goes to the next row.
        /// </summary>
        /// <returns>A list of missed stack positions.</returns>
        public List<MissResult> Place()
        {
            var misses = new List<MissResult>();

            // If we are placing at the start of the stack there is no chance for a miss
            if (ActiveRow != 0)
            {
                for (var i = 0; i < Width; i++)
                {
                    if (Stack[i, ActiveRow].State != State.Occupied)
                        continue;

                    if (Stack[i, ActiveRow - 1].State != State.Vacant)
                        continue;

                    Stack[i, ActiveRow].State = State.Vacant;
                    misses.Add(new MissResult(i, ActiveRow));
                    StackWidth--;
                }
            }

            IncreaseActiveRow();

            return misses;
        }

        /// <summary>
        /// This will move the stack down so the game can continue endlessly.
        /// </summary>
        public void ResetHeight()
        {
            // Get the top row variables
            var highestRow = new Square[Width];
            for (var i = 0; i < Width; i++)
            {
                // The active row should be increased ever time a stack is placed,
                // So the highest row should be 1 below the current active row
                highestRow[i] = Stack[i, ActiveRow - 1];
            }

            ResetGrid();

            // The active row is now 0 after being reset
            for (var i = 0; i < Width; i++)
            {
                Stack[i, ActiveRow] = highestRow[i];
            }

            IncreaseActiveRow();
        }

        private void IncreaseActiveRow()
        {
            ActiveRow++;
        }
    }
}