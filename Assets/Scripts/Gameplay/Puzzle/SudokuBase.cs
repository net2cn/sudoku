using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Sudoku.Gameplay.Puzzle
{
    public abstract class SudokuBase
    {
        protected int[,] grid;

        public virtual int this[int i, int j]
        {
            get
            {
                return grid[i, j];
            }
        }

        public virtual bool Set(int i, int j, int num)
        {
            throw new NotImplementedException();
        }

        public virtual void Generate(int emptyCount = 0)
        {
            throw new NotImplementedException();
        }

        public virtual void Solve()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            int x = grid.GetLength(0);
            int y = grid.GetLength(1);
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y - 1; j++)
                {
                    sb.Append($"{grid[i, j]} ");
                }
                sb.Append($"{grid[i, y - 1]}\n");
            }
            return sb.ToString();
        }
    }
}