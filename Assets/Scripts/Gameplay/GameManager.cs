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
    public GameObject overlay;

    private Sudoku9x9 puzzle = new Sudoku9x9();

    private Image overlayImage;
    private Button overlayButton;

    private int currentCellIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(puzzleGrid, "You probably forget to set puzzle grid before you start the game.");
        Assert.IsNotNull(inputKeyboard, "You probably forget to set input keyboard before you start the game.");
        Assert.IsNotNull(overlay, "You probably forget to set overlay before you start the game.");

        overlayImage = overlay.GetComponent<Image>();
        overlayButton = overlay.GetComponent<Button>();

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

        overlayButton.onClick.AddListener(delegate { SetCell(0); });
    }

    void OpenKeyboard(GameObject button, int index)
    {
        int x = puzzle.GetLength(0);
        int i = index / x;
        int j = index % x;

        // if the element is located at the edge of the grid, offset it by 1 cell.
        i += (i == 0) ? 1 : ((i == x - 1) ? -1 : 0);
        j += (j == 0) ? 1 : ((j == x - 1) ? -1 : 0);

        inputKeyboard.transform.position = puzzleGrid.transform.GetChild(i * x + j).position;

        overlayButton.interactable = true;
        overlayImage.raycastTarget = true;
        inputKeyboard.SetActive(true);
        currentCellIndex = index;
    }

    void SetCell(int value)
    {
        if (currentCellIndex == -1)
        {
            throw new System.InvalidOperationException("You should set currentCellIndex before calling SetCell()");
        }

        if (value != 0)
        {
            puzzle[currentCellIndex] = value;
            puzzleGrid.transform.GetChild(currentCellIndex).GetChild(0).GetComponent<TextMeshProUGUI>().text = value.ToString();
        }

        currentCellIndex = -1;
        inputKeyboard.SetActive(false);
        overlayImage.raycastTarget = false;
        overlayButton.interactable = false;
    }
}
