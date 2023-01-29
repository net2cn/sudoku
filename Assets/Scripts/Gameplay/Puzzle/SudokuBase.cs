using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using System.IO;
using Unity.VisualScripting;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Sudoku.Gameplay.Puzzle
{
    [DataContract]
    public abstract class SudokuBase
    {
        [DataMember] public int sideLength = 0;
        [DataMember] public int removedCellCount = 0;
        [DataMember] public int[] removedCellIndex;

        [DataMember] protected int[] solution;
        [DataMember] private int[] grid;
        protected int[] Grid
        {
            get => grid;
            set
            {
                double length = Math.Sqrt(value.Length);
                if (length % 1 != 0)
                {
                    throw new ArithmeticException("SudokuBase only takes a square grid.");
                }
                this.sideLength = (int)length;
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
                return Grid[i * sideLength + j];
            }
            set
            {
                Grid[i * sideLength + j] = value;
            }
        }

        public int this[int i]
        {
            get
            {
                return Grid[i];
            }
            set
            {
                Grid[i] = value;
            }
        }

        public virtual void Generate(int emptyCount = 0)
        {
            if (emptyCount > Length)
            {
                Debug.LogError($"You can't generate a puzzle with empty count of {emptyCount}. Maximum allowed is {Length}.");
            }
            removedCellCount = emptyCount;
            removedCellIndex = new int[emptyCount];
        }

        public virtual bool Validate() { throw new NotImplementedException(); }

        public virtual string Serialize() { throw new NotImplementedException(); }

        public static T Deserialize<T>(string json) where T : SudokuBase
        {
            var deserializer = new DataContractJsonSerializer(typeof(T));
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                return (T)deserializer.ReadObject(ms);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < sideLength; i++)
            {
                for (int j = 0; j < sideLength - 1; j++)
                {
                    sb.Append($"{this[i, j]} ");
                }
                sb.Append($"{this[i, sideLength - 1]}\n");
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