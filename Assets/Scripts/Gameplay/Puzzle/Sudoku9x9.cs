using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Sudoku.Gameplay.Puzzle
{
    public sealed class Sudoku9x9 : SudokuBase
    {
        public Sudoku9x9()
        {
            grid = new int[9, 9];
        }

        public Sudoku9x9(int[,] initialGrid)
        {
            if (initialGrid.Rank == 2 && initialGrid.GetLength(0) == 9 && initialGrid.GetLength(1) == 9)
            {
                grid = initialGrid;
            }
            else
            {
                throw new ArgumentException("A 9x9 matrix is required.");
            }
        }

        public override bool Set(int i, int j, int num)
        {
            if (CheckIsNumberAvailable(i, j, num))
            {
                grid[i, j] = num;
                return true;
            }
            return false;
        }

        // Start is called before the first frame update
        public override void Generate(int emptyCount = 0)
        {
            FillDiagnonalBox();
            BacktrackAllCells(0, 3);

            RemoveElements(emptyCount);
        }

        public override void Solve()
        {
            BacktrackAllCells(0, 0);
        }

        private void FillDiagnonalBox()
        {
            // Sudoku puzzle has an interesting property that the diagnonal boxes (3x3 matrix) are all independent of the other boxes.
            // Exploiting this property ensures we have a randomized puzzle.
            int[] nums = Enumerable.Range(1, 9).ToArray();
            for (int i = 0; i < 3; i++)
            {
                Shuffle(ref nums);
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        grid[j + i * 3, k + i * 3] = nums[j * 3 + k];
                    }
                }
            }
        }

        private bool BacktrackAllCells(int i, int j)
        {
            // Zig-zag traverse through the whole grid.
            if (j >= 9)
            {
                if (i < 8)
                {
                    i += 1;
                    j = 0;
                }
                else
                {
                    return true;
                }
            }

            // Skip non-zero tiles.
            if (grid[i, j] != 0)
            {
                return BacktrackAllCells(i, j + 1);
            }

            for (int num = 1; num <= 9; num++)
            {
                if (CheckIsNumberAvailable(i, j, num))
                {
                    grid[i, j] = num;
                    if (BacktrackAllCells(i, j + 1))
                    {
                        return true;
                    }
                    grid[i, j] = 0;
                }
            }
            return false;
        }

        private bool CheckIsNumberAvailable(int i, int j, int num)
        {
            for (int k = 0; k < 9; k++)
            {
                // Horizontal
                if (grid[i, k] == num)
                {
                    return false;
                }
                // Vertical
                if (grid[k, j] == num)
                {
                    return false;
                }
                // Box
                if (grid[i - i % 3 + k / 3, j - j % 3 + k % 3] == num)
                {
                    return false;
                }
            }
            return true;
        }

        private void RemoveElements(int count)
        {
            // Make sure remove exactly the count
            int[] seq = Enumerable.Range(0, 80).ToArray();
            Shuffle(ref seq);
            for (int i = 0; i < count; i++)
            {
                grid[seq[i] / 9, seq[i] % 9] = 0;
            }
        }

        // Fisher-Yates shuffle algorithm
        private void Shuffle<T>(ref T[] arr)
        {
            var rng = new System.Random();
            int n = arr.Length;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (arr[k], arr[n]) = (arr[n], arr[k]);
            }
        }
    }
}