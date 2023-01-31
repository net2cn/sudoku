using System;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

using DG.Tweening;
using TMPro;

using Sudoku.Gameplay.Puzzle;
using UnityEngine.Playables;

namespace Sudoku.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        public GameObject puzzleGrid;
        public GameObject inputKeyboard;
        public GameObject overlay;
        public Button returnButton;

        [Tooltip("Setting this number higher than 41 will increase the probability of generating sudoku with more than 1 solution.")]
        public int removeCellCount = 31;    // I've read somewhere the upper bound of the empty cell count is 64. (that is given 17 clues)

        public float solvedFlipAnimationInterval = 0.08f;

        public Sprite[] solvedSprite;

        private Sudoku9x9 puzzle;

        private Image overlayImage;
        private Button overlayButton;

        private int currentCellIndex = -1;
        private int filledCount = 0;

        private static string dataFilePath = "progress.json";
        private static string difficultyKey = "difficulty";

        void Awake()
        {
            Assert.IsNotNull(puzzleGrid, "You probably forget to set puzzleGrid before you start the game.");
            Assert.IsNotNull(inputKeyboard, "You probably forget to set inputKeyboard before you start the game.");
            Assert.IsNotNull(overlay, "You probably forget to set overlay before you start the game.");
            Assert.IsNotNull(returnButton, "You probably forget to set returnButton before you start the game.");

            dataFilePath = Path.Combine(Application.persistentDataPath, dataFilePath);

            if (File.Exists(dataFilePath))
            {
                var json = File.ReadAllText(dataFilePath);
                puzzle = SudokuBase.Deserialize<Sudoku9x9>(json);
            }
            else
            {
                if (PlayerPrefs.HasKey(difficultyKey))
                {
                    removeCellCount = PlayerPrefs.GetInt(difficultyKey);
                }
                puzzle = new Sudoku9x9();
                puzzle.Generate(removeCellCount);
            }

            overlayImage = overlay.GetComponent<Image>();
            overlayButton = overlay.GetComponent<Button>();
        }

        // Start is called before the first frame update
        void Start()
        {
            Assert.AreEqual(puzzleGrid.transform.childCount, puzzle.Length, $"puzzleGrid should have exactly {puzzle.Length} child.");
            Assert.AreEqual(inputKeyboard.transform.childCount, puzzle.sideLength, $"inputKeyboard should have exactly {puzzle.sideLength} child.");
            Assert.AreEqual(puzzleGrid.transform.childCount, solvedSprite.Length, "solvedSprite should have exact same elements count as puzzleGrid child count.");

            // Set display grid and click events.
            for (int i = 0; i < puzzle.Length; i++)
            {
                var go = puzzleGrid.transform.GetChild(i);
                var btn = go.GetComponent<Button>();
                var tmp = go.GetComponentInChildren<TextMeshProUGUI>();

                tmp.text = puzzle[i].ToString();
                btn.transition = Selectable.Transition.None;
            }

            foreach (var i in puzzle.removedCellIndex)
            {
                var go = puzzleGrid.transform.GetChild(i);
                var btn = go.GetComponent<Button>();
                var tmp = go.GetComponentInChildren<TextMeshProUGUI>();

                if (puzzle[i] == 0)
                {
                    tmp.text = "";
                }
                tmp.color = Color.blue;
                btn.transition = Selectable.Transition.ColorTint;

                int index = i;
                btn.onClick.AddListener(delegate { OpenKeyboard(tmp.gameObject, index); });
            }

            // Set keyboard click events
            for (int i = 0; i < puzzle.sideLength; i++)
            {
                int value = i + 1;
                inputKeyboard.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate { SetCell(value); });
            }

            // Set overlay click event
            overlayButton.onClick.AddListener(delegate { SetCell(0); });    // Retract keyboard by setting 0.

            // Set return button click event
            returnButton.onClick.AddListener(delegate { SaveProgress(); });
        }

        void Update()
        {
            // Read input from keyboard.
            if (currentCellIndex != -1 && Input.inputString != "")
            {
                if (Int32.TryParse(Input.inputString, out int number) && number >= 0 && number < 10)
                {
                    SetCell(number);
                }
            }
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

                // Validate puzzle once all empty cells are filled.
                if (filledCount == puzzle.removedCellIndex.Length && puzzle.Validate())
                {
                    PuzzleSolvedHandler();
                }
            }

            currentCellIndex = -1;
            inputKeyboard.SetActive(false);
            overlayImage.raycastTarget = false;
            overlayButton.interactable = false;
        }

        void SaveProgress()
        {
            Debug.Log($"Saving game progress to {dataFilePath}...");
            var json = puzzle.Serialize();
            File.WriteAllText(dataFilePath, json);
        }

        void PuzzleSolvedHandler()
        {
            if (File.Exists(dataFilePath))
            {
                File.Delete(dataFilePath);
            }

            // Animate grid, start flipping animation from top left to bottom right
            var flipOutSequence = DOTween.Sequence();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < i + 1; j++)
                {
                    // top left triangle
                    int idx = i + j * 8;
                    var go = puzzleGrid.transform.GetChild(idx);
                    var btn = go.GetComponent<Button>();

                    go.GetComponent<Image>().color= Color.white;
                    btn.transition = Selectable.Transition.None;
                    btn.interactable = false;
                    flipOutSequence.Insert(solvedFlipAnimationInterval * i, go.transform.DORotate(new Vector3(0, 90, 0), solvedFlipAnimationInterval));

                    // bottom right triangle - start time reversed
                    if (i < 8)
                    {
                        idx = 80 - idx;
                        go = puzzleGrid.transform.GetChild(idx);
                        btn = go.GetComponent<Button>();

                        go.GetComponent<Image>().color = Color.white;
                        btn.transition = Selectable.Transition.None;
                        btn.interactable = false;
                        flipOutSequence.Insert(solvedFlipAnimationInterval * (16 - i), go.transform.DORotate(new Vector3(0, 90, 0), solvedFlipAnimationInterval));
                    }
                }
            }



            flipOutSequence.OnComplete(() =>
            {
                // remove all text
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
                        // top left triangle
                        int idx = i + j * 8;
                        var go = puzzleGrid.transform.GetChild(idx);

                        flipInSequence.Insert(solvedFlipAnimationInterval * i, go.transform.DORotate(new Vector3(0, 0, 0), solvedFlipAnimationInterval));

                        // bottom right triangle - start time reversed
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
}