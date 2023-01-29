using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using UnityEngine;

namespace Sudoku.Gameplay.Puzzle
{
    // https://www.sudokuoftheday.com/creation
    public sealed class Sudoku9x9 : SudokuBase
    {
        public Sudoku9x9()
        {
            Grid = new int[81];
        }

        public Sudoku9x9(int[] initialGrid)
        {
            if (initialGrid.Length == 81)
            {
                Grid = initialGrid;
            }
            else
            {
                throw new ArgumentException("A 9x9 matrix is required.");
            }
        }

        // Start is called before the first frame update
        public override void Generate(int emptyCount = 0)
        {
            base.Generate(emptyCount);
            FillDiagnonalBox();
            DFS(0, 3);
            solution = (int[])Grid.Clone();
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
                // Check each removed cell.
                foreach (var idx in removedCellIndex)
                {
                    int temp = this[idx];
                    this[idx] = 0;
                    if (!CheckIsNumberAvailable(idx / sideLength, idx % sideLength, temp))
                    {
                        Debug.Log("Not unique!");
                        return false;
                    }
                    this[idx] = temp;
                }
                return true;
            }

            return false;
        }

        public override string Serialize()
        {
            var serializer = new DataContractJsonSerializer(typeof(Sudoku9x9));
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, this);
                return Encoding.Default.GetString(ms.ToArray());
            }
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
                        this[j + i * 3, k + i * 3] = nums[j * 3 + k];
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
            if (this[i, j] != 0)
            {
                return DFS(i, j + 1);
            }

            for (int num = 1; num <= 9; num++)
            {
                if (CheckIsNumberAvailable(i, j, num))
                {
                    this[i, j] = num;
                    if (DFS(i, j + 1))
                    {
                        return true;
                    }
                    this[i, j] = 0;
                }
            }
            return false;
        }

        private bool CheckIsNumberAvailable(int i, int j, int num)
        {
            for (int k = 0; k < 9; k++)
            {
                // Horizontal
                if (this[i, k] == num)
                {
                    return false;
                }
                // Vertical
                if (this[k, j] == num)
                {
                    return false;
                }
                // Box
                if (this[i - i % 3 + k / 3, j - j % 3 + k % 3] == num)
                {
                    return false;
                }
            }
            return true;
        }

        private bool TryRemovePair(int i, int j)
        {
            int availableCount = 0;
            int tempI = this[i];
            this[i] = 0;
            int tempJ = this[j];
            this[j] = 0;
            for (int num = 0; num < 9; num++)
            {
                if (CheckIsNumberAvailable(i / 9, i % 9, num))
                {
                    availableCount++;
                }
                if (CheckIsNumberAvailable(j / 9, j % 9, num))
                {
                    availableCount++;
                }
            }
            if (availableCount > 2)
            {
                this[i] = tempI;
                this[j] = tempJ;
                return false;
            }
            return true;
        }

        private void RemoveElements(int count)
        {
            // Make sure remove exactly the count
            int[] seq = Enumerable.Range(0, 39).ToArray();
            Shuffle(ref seq);
            // TODO: Test solvability before removal.
            int removedPair = 0;
            int idx = 0;
            while (removedPair < count / 2 && idx<81)
            {
                if (TryRemovePair(seq[idx], 80 - seq[idx]))
                {
                    removedCellIndex[removedPair * 2] = seq[idx];
                    removedCellIndex[removedPair * 2 + 1] = 80 - seq[idx];    // Also remove its rotational counterpart as suggested.
                    removedPair++;
                }
                else
                {
                    Debug.Log($"Removing cell at index {idx} and {80-idx} will cause the sudoku has more than 1 solution, skipping...");
                }
                idx++;
            }

            // Failed all attempts at removing elements, go around.
            if (idx == 81)
            {
                for(int i = 0; i < Length; i++) {
                    Grid[i] = solution[i];
                }
                RemoveElements(count);
            }

            // For convinience remove the center element.
            if (count % 2 == 1)
            {
                this[4, 4] = 0;
                removedCellIndex[^1] = 40;
            }
        }
    }
}