using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Sudoku.Gameplay.Puzzle
{
    public abstract class SudokuBase
    {
        private int[,] grid;
        protected int[,] Grid
        {
            get => grid;
            set
            {
                if (value.GetLength(0) != value.GetLength(1))
                {
                    throw new ArithmeticException("SudokuBase only takes a square grid.");
                }
                grid = value;
            }
        }

        public int Length
        {
            get => Grid.Length;
        }

        public int this[int i, int j]
        {
            get
            {
                return Grid[i, j];
            }
        }

        public int this[int i]
        {
            get
            {
                return Grid[i / GetLength(0), i % GetLength(0)];
            }
        }

        public virtual void Set(int i, int j, int value)
        {
            throw new NotImplementedException();
        }

        public void Set(int i, int value)
        {
            Set(i / GetLength(0), i % GetLength(0), value);
        }

        public virtual void Generate(int emptyCount = 0)
        {
            throw new NotImplementedException();
        }

        public virtual bool Validate()
        {
            throw new NotImplementedException();
        }

        public int GetLength(int dimension)
        {
            if (dimension < 0 || dimension >= 2)
            {
                throw new ArgumentOutOfRangeException($"Argument {nameof(dimension)} takes input range [0,1], getting {dimension}.");
            }
            return Grid.GetLength(dimension);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            int x = GetLength(0);
            int y = GetLength(1);
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