using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

namespace Sudoku.Gameplay.Puzzle
{
    public abstract class SudokuBase
    {
        protected int[,] solution;

        private int[,] grid;
        protected int[,] Grid
        {
            get => grid;
            set
            {
                Assert.AreEqual(value.GetLength(0), value.GetLength(1));
                grid = value;
            }
        }

        public int Length
        {
            get => Grid.Length;
        }

        public virtual int this[int i, int j]
        {
            get
            {
                return Grid[i, j];
            }
        }

        public virtual int this[int i]
        {
            get
            {
                return Grid[i / Grid.GetLength(0), i % Grid.GetLength(1)];
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

        public virtual void Validate()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            int x = Grid.GetLength(0);
            int y = Grid.GetLength(1);
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y - 1; j++)
                {
                    sb.Append($"{Grid[i, j]} ");
                }
                sb.Append($"{Grid[i, y - 1]}\n");
            }
            return sb.ToString();
        }

        // Fisher-Yates shuffle algorithm
        protected void Shuffle<T>(ref T[] arr)
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