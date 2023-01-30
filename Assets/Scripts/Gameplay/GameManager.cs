using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sudoku.Gameplay.Puzzle;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public GameObject puzzleGrid;
    public GameObject inputKeyboard;
    public GameObject overlay;

    [Tooltip("Setting this number higher than 41 will increase the probability of generating sudoku with more than 1 solution.")]
    public int removeCellCount = 31;    // I've read somewhere the upper bound of the empty cell count is 64. (that is given 17 clues)

    public float solvedFlipAnimationInterval = 0.08f;

    public Sprite[] solvedSprite;

    private Sudoku9x9 puzzle = new Sudoku9x9();

    private Image overlayImage;
    private Button overlayButton;

    private int currentCellIndex = -1;
    private int filledCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(puzzleGrid, "You probably forget to set puzzle grid before you start the game.");
        Assert.IsNotNull(inputKeyboard, "You probably forget to set input keyboard before you start the game.");
        Assert.IsNotNull(overlay, "You probably forget to set overlay before you start the game.");
        Assert.AreEqual(puzzleGrid.transform.childCount, puzzle.Length, $"puzzleGrid should have exactly {puzzle.Length} child.");
        Assert.AreEqual(inputKeyboard.transform.childCount, puzzle.sideLength, $"inputKeyboard should have exactly {puzzle.sideLength} child.");
        Assert.AreEqual(puzzleGrid.transform.childCount, solvedSprite.Length, "solvedSprite should have exact same elements count as puzzleGrid child count.");

        overlayImage = overlay.GetComponent<Image>();
        overlayButton = overlay.GetComponent<Button>();

        puzzle.Generate(removeCellCount);

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

        for (int i = 0; i < puzzle.sideLength; i++)
        {
            int value = i + 1;
            inputKeyboard.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate { SetCell(value); });
        }

        overlayButton.onClick.AddListener(delegate { SetCell(0); });

        // serialization driver code
        var json = puzzle.Serialize();
        Debug.Log(json);
        Debug.Log(SudokuBase.Deserialize<Sudoku9x9>(json));
    }

    void OpenKeyboard(GameObject button, int index)
    {
        int x = puzzle.sideLength;
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
            if (puzzle[currentCellIndex] == 0)
            {
                filledCount++;
            }
            puzzle[currentCellIndex] = value;
            puzzleGrid.transform.GetChild(currentCellIndex).GetChild(0).GetComponent<TextMeshProUGUI>().text = value.ToString();
        }

        // Validate puzzle once all empty cells are filled.
        if (filledCount == puzzle.removedCellIndex.Length && puzzle.Validate())
        {
            SolvedAnimation();
        }

        currentCellIndex = -1;
        inputKeyboard.SetActive(false);
        overlayImage.raycastTarget = false;
        overlayButton.interactable = false;
    }

    void SolvedAnimation()
    {
        var flipOutSequence = DOTween.Sequence();
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < i + 1; j++)
            {
                int idx = i + j * 8;
                var go = puzzleGrid.transform.GetChild(idx);
                var btn = go.GetComponent<Button>();
                btn.transition = Selectable.Transition.None;
                btn.interactable = false;
                flipOutSequence.Insert(solvedFlipAnimationInterval * i, go.transform.DORotate(new Vector3(0, 90, 0), solvedFlipAnimationInterval));

                if (i < 8)
                {
                    idx = 80 - idx;
                    go = puzzleGrid.transform.GetChild(idx);
                    btn = go.GetComponent<Button>();
                    btn.transition = Selectable.Transition.None;
                    btn.interactable = false;
                    flipOutSequence.Insert(solvedFlipAnimationInterval * (16 - i), go.transform.DORotate(new Vector3(0, 90, 0), solvedFlipAnimationInterval));
                }
            }
        }



        flipOutSequence.OnComplete(() =>
        {
            for (int i = 0; i < puzzleGrid.transform.childCount; i++)
            {
                var go = puzzleGrid.transform.GetChild(i);
                go.GetComponent<Image>().sprite = solvedSprite[i];
                go.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
            var flipInSequence = DOTween.Sequence();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < i + 1; j++)
                {
                    int idx = i + j * 8;
                    var go = puzzleGrid.transform.GetChild(idx);

                    flipInSequence.Insert(solvedFlipAnimationInterval * i, go.transform.DORotate(new Vector3(0, 0, 0), solvedFlipAnimationInterval));

                    if (i < 8)
                    {
                        idx = 80 - idx;
                        go = puzzleGrid.transform.GetChild(idx);
                        flipInSequence.Insert(solvedFlipAnimationInterval * (16 - i), go.transform.DORotate(new Vector3(0, 0, 0), solvedFlipAnimationInterval));
                    }
                }
            }
            flipInSequence.Play();
        }).Play();
    }
}
