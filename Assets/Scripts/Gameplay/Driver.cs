using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sudoku.Gameplay.Puzzle;

public class Driver : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Sudoku9x9 puzzle = new Sudoku9x9();
        puzzle.Generate(10);
        Debug.Log(puzzle.ToString());
    }
}
