using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Sudoku.Gameplay.Puzzle
{
    // https://www.sudokuoftheday.com/creation
    public sealed class Sudoku9x9 : SudokuBase
    {
        private int[] removedIndex;

        public Sudoku9x9()
        {
            Grid = new int[9, 9];
        }

        public Sudoku9x9(int[,] initialGrid)
        {
            if (initialGrid.Rank == 2 && initialGrid.GetLength(0) == 9 && initialGrid.GetLength(1) == 9)
            {
                Grid = initialGrid;
            }
            else
            {
                throw new ArgumentException("A 9x9 matrix is required.");
            }
        }

        public override void Set(int i, int j, int num)
        {
            Grid[i, j] = num;
        }

        // Start is called before the first frame update
        public override void Generate(int emptyCount = 0)
        {
            if (emptyCount > 81)
            {
                Debug.LogError($"You can't generate a puzzle with empty count of {emptyCount}. Maximum allowed is 81");
            }
            removedIndex = new int[emptyCount];
            FillDiagnonalBox();
            DFS(0, 3);
            RemoveElements(emptyCount);
        }

        public override bool Validate()
        {
            int sum = 0;
            for (int i = 0; i < Grid.Length; i++)
            {
                sum += this[i];
            }

            if (sum == 405) // 405 as the sum of a complete sudoku
            {
                // Check each cell.
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        int temp = Grid[i, j];
                        Grid[i, j] = 0;
                        if (!CheckIsNumberAvailable(i, j, temp))
                        {
                            Debug.Log("Not unique!");
                            return false;
                        }
                        Grid[i, j] = temp;
                    }
                }
                return true;
            }

            return false;
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
                        Grid[j + i * 3, k + i * 3] = nums[j * 3 + k];
                    }
                }
            }
        }

        private bool DFS(int i, int j)
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
            if (Grid[i, j] != 0)
            {
                return DFS(i, j + 1);
            }

            for (int num = 1; num <= 9; num++)
            {
                if (CheckIsNumberAvailable(i, j, num))
                {
                    Grid[i, j] = num;
                    if (DFS(i, j + 1))
                    {
                        return true;
                    }
                    Grid[i, j] = 0;
                }
            }
            return false;
        }

        private bool CheckIsNumberAvailable(int i, int j, int num)
        {
            for (int k = 0; k < 9; k++)
            {
                // Horizontal
                if (Grid[i, k] == num)
                {
                    return false;
                }
                // Vertical
                if (Grid[k, j] == num)
                {
                    return false;
                }
                // Box
                if (Grid[i - i % 3 + k / 3, j - j % 3 + k % 3] == num)
                {
                    return false;
                }
            }
            return true;
        }

        private void RemoveElements(int count)
        {
            // Make sure remove exactly the count
            int[] seq = Enumerable.Range(0, 39).ToArray();
            Shuffle(ref seq);
            // TODO: Test solvability before removal.
            for (int i = 0; i < count / 2; i++)
            {
                Grid[seq[i] / 9, seq[i] % 9] = 0;
                // Remove its rotational counterpart as well.
                Grid[8 - seq[i] / 9, 8 - seq[i] % 9] = 0;

                removedIndex[i * 2] = seq[i];
                removedIndex[i * 2 + 1] = 80 - seq[i];
            }

            // For convinience remove the center element.
            if (count % 2 == 1)
            {
                Grid[4, 4] = 0;
                removedIndex[^1] = 40;
            }
        }
    }
}