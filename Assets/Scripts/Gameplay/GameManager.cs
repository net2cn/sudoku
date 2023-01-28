using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sudoku.Gameplay.Puzzle;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject puzzleGrid;

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(puzzleGrid, "You probably forget to set puzzle grid before you start the game.");

        Sudoku9x9 puzzle = new Sudoku9x9();
        puzzle.Generate(31);
        Debug.Log(puzzle.ToString());

        Assert.AreEqual(puzzleGrid.transform.childCount, puzzle.Length);

        for(int i = 0; i < puzzle.Length; i++)
        {
            if (puzzle[i] != 0)
            {
                puzzleGrid.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = puzzle[i].ToString();
            }
            else
            {
                puzzleGrid.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            }
        }
    }
}
