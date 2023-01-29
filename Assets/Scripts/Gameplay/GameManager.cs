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
    public GameObject inputKeyboard;

    private Sudoku9x9 puzzle = new Sudoku9x9();
    private float gridCellWidth = 0;
    private float gridSpacingWidth = 0;
    private int constraintCount = 0;

    private int currentCellIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(puzzleGrid, "You probably forget to set puzzle grid before you start the game.");
        Assert.IsNotNull(inputKeyboard, "You probably forget to set input keyboard before you start the game.");

        gridCellWidth = puzzleGrid.GetComponent<GridLayoutGroup>().cellSize.x;
        gridSpacingWidth = puzzleGrid.GetComponent<GridLayoutGroup>().spacing.x;
        constraintCount = inputKeyboard.GetComponent<GridLayoutGroup>().constraintCount;

        puzzle.Generate(31);
        Debug.Log(puzzle.ToString());

        Assert.AreEqual(puzzleGrid.transform.childCount, puzzle.Length);
        Assert.AreEqual(inputKeyboard.transform.childCount, puzzle.GetLength(0));

        for (int i = 0; i < puzzle.Length; i++)
        {
            var tmp = puzzleGrid.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
            if (puzzle[i] != 0)
            {
                tmp.text = puzzle[i].ToString();
                tmp.GetComponentInParent<Button>().transition = Selectable.Transition.None;
            }
            else
            {
                tmp.text = "";
                tmp.color = Color.blue;
                int index = i;
                tmp.GetComponentInParent<Button>().onClick.AddListener(delegate { OpenKeyboard(tmp.gameObject, index); });
            }
        }

        for (int i = 0; i < puzzle.GetLength(0); i++)
        {
            int value = i + 1;
            inputKeyboard.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate { SetCell(value); });
        }
    }

    void OpenKeyboard(GameObject button, int index)
    {
        int x = puzzle.GetLength(0);
        int i = index / x;
        int j = index % x;

        // gridCellWidth / 4 because the pivot of the grid element is located at the top left corner.
        // + gridSpacingWidth * constraintCount because of the spacing in between each cell.
        float offsetDist = gridCellWidth / 4 + gridSpacingWidth * constraintCount;

        inputKeyboard.transform.position = button.transform.position;

        // if the element is located at the edge of the grid, offset it by 1 cell.
        if (i == 0)
        {
            inputKeyboard.transform.position -= new Vector3(0, offsetDist);
        }
        else if (i == x - 1)
        {
            inputKeyboard.transform.position += new Vector3(0, offsetDist);
        }

        if (j == 0)
        {
            inputKeyboard.transform.position += new Vector3(offsetDist, 0);
        }
        else if (j == x - 1)
        {
            inputKeyboard.transform.position -= new Vector3(offsetDist, 0);
        }

        inputKeyboard.SetActive(true);
        currentCellIndex = index;
    }

    void SetCell(int value)
    {
        if (currentCellIndex == -1)
        {
            throw new System.InvalidOperationException("You should set currentCellIndex before calling SetCell()");
        }

        puzzle.Set(currentCellIndex, value);
        puzzleGrid.transform.GetChild(currentCellIndex).GetChild(0).GetComponent<TextMeshProUGUI>().text = value.ToString();
        currentCellIndex = -1;
        inputKeyboard.SetActive(false);
    }
}
