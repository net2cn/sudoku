using System;
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
        private const int REMOVAL_ATTEMPS = 10;
        
        public Sudoku9x9()
        {
            Grid = new int[81];
        }

        public Sudoku9x9(int[] initialGrid)
        {
            if (initialGrid.Length != 81)
            {
                throw new ArgumentException("A 9x9 matrix is required.");
            }

            Grid = initialGrid;
        }

        // Start is called before the first frame update
        public override void Generate(int emptyCount = 0)
        {
            base.Generate(emptyCount);
            FillDiagnonalBox();
            DFS(0, 3);
            _solution = (int[])Grid.Clone();
            RemoveElements(emptyCount);
        }

        public override bool Validate()
        {
            // 405 as the complete sum of a solved sudoku grid
            if (Grid.Sum() != 405)
            {
                return false;
            }

            // Check each removed cell.
            foreach (var idx in removedCellIndex)
            {
                int temp = this[idx];
                this[idx] = 0;
                if (!CheckIsNumberAvailable(idx / sideLength, idx % sideLength, temp))
                {
                    Debug.Log($"Not unique at ({idx / sideLength}, {idx % sideLength})!");
                    return false;
                }
                this[idx] = temp;
            }

            return true;
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
                if (i >= 8)
                {
                    return true;
                }

                i += 1;
                j = 0;
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
                }
            }

            this[i, j] = 0;
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

        private bool TryRemovePair(int a, int b, bool force = false)
        {
            int availableCount = 0;
            int tempI = this[a];
            this[a] = 0;
            int tempJ = this[b];
            this[b] = 0;
            if (!force) // When force is set to true, the given pair will be removed anyway without check solution uniqueness after removal.
            {
                for (int num = 0; num < 9; num++)
                {
                    if (CheckIsNumberAvailable(a / 9, a % 9, num))
                    {
                        availableCount++;
                    }
                    if (CheckIsNumberAvailable(b / 9, b % 9, num))
                    {
                        availableCount++;
                    }
                }
                if (availableCount > 2)
                {
                    this[a] = tempI;
                    this[b] = tempJ;
                    return false;
                }
            }

            return true;
        }

        private void RemoveElements(int count, int attemp = 0)
        {
            // Remove by pair
            int[] seq = Enumerable.Range(0, 40).ToArray();
            Shuffle(ref seq);
            // TODO: Test solvability before removal.
            int removedPair = 0;
            int seqIdx = 0;
            while (removedPair < count / 2 && seqIdx < seq.Length)
            {
                if (TryRemovePair(seq[seqIdx], 80 - seq[seqIdx], attemp == REMOVAL_ATTEMPS - 1))
                {
                    removedCellIndex[removedPair * 2] = seq[seqIdx];
                    removedCellIndex[removedPair * 2 + 1] = 80 - seq[seqIdx];    // Also remove its rotational counterpart as suggested.
                    removedPair++;
                }
                else
                {
                    Debug.Log($"Removing cell at index {seq[seqIdx]} and {80 - seq[seqIdx]} will cause the sudoku has more than 1 solution, skipping...");
                }
                seqIdx++;
            }

            // Failed all attempts at removing elements, go around.
            if (seqIdx == seq.Length)
            {
                if (attemp < REMOVAL_ATTEMPS - 1)
                {
                    Debug.LogWarning($"Unable to generate a sudoku with unique solution. Retrying attemp #{attemp + 2} out of maximum {REMOVAL_ATTEMPS}");

                    for (int i = 0; i < Length; i++)
                    {
                        Grid[i] = _solution[i];
                    }
                    RemoveElements(count, attemp + 1);
                    return;
                }
            }

            if (attemp==REMOVAL_ATTEMPS - 1)
            {
                Debug.LogError($"Unable to generate a sudoku with unique solution at maximum {REMOVAL_ATTEMPS} attempts. A sudoku with probably more than 1 solution is generated.");
            }

            // For convinience remove the center element if odd count is present.
            if (count % 2 == 1)
            {
                this[4, 4] = 0;
                removedCellIndex[^1] = 40;
            }
        }
    }
}