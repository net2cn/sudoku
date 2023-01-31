using System;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

using DG.Tweening;
using TMPro;

using Sudoku.Gameplay.Puzzle;

namespace Sudoku.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        public GameObject puzzleGrid;
        public GameObject inputKeyboard;
        public GameObject overlay;
        public Button returnButton;

        [Tooltip("Setting this number higher than 41 will increase the probability of generating sudoku with more than 1 solution.")]
        public int removeCellCount = 21;    // I've read somewhere the upper bound of the empty cell count is 64. (that is given 17 clues)

        public float solvedFlipAnimationInterval = 0.08f;

        public Sprite[] solvedSprite;

        private Sudoku9x9 _puzzle;

        private Image _overlayImage;
        private Button _overlayButton;

        private int _currentCellIndex = -1;
        private int _filledCount = 0;

        void Awake()
        {
            Assert.IsNotNull(puzzleGrid, "You probably forget to set puzzleGrid before you start the game.");
            Assert.IsNotNull(inputKeyboard, "You probably forget to set inputKeyboard before you start the game.");
            Assert.IsNotNull(overlay, "You probably forget to set overlay before you start the game.");
            Assert.IsNotNull(returnButton, "You probably forget to set returnButton before you start the game.");

            Globals.PROGRESS_DATA_FILE_PATH = Path.Combine(Application.persistentDataPath, Globals.PROGRESS_DATA_FILE_PATH);

            if (File.Exists(Globals.PROGRESS_DATA_FILE_PATH))
            {
                // Load existing progress
                var json = File.ReadAllText(Globals.PROGRESS_DATA_FILE_PATH);
                _puzzle = SudokuBase.Deserialize<Sudoku9x9>(json);
                if (_puzzle.solved == true) // Solved puzzle should be discarded.
                {
                    File.Delete(Globals.PROGRESS_DATA_FILE_PATH);
                    _puzzle = null;
                }
            }

            if (_puzzle == null) // No available progress.
            {
                // Start game
                if (PlayerPrefs.HasKey(Globals.DIFFICULTY_KEY))
                {
                    removeCellCount = PlayerPrefs.GetInt(Globals.DIFFICULTY_KEY);
                }
                else
                {
                    removeCellCount = 21;
                }
                _puzzle = new Sudoku9x9();
                _puzzle.Generate(removeCellCount);
            }

            _overlayImage = overlay.GetComponent<Image>();
            _overlayButton = overlay.GetComponent<Button>();
        }

        // Start is called before the first frame update
        void Start()
        {
            Assert.AreEqual(puzzleGrid.transform.childCount, _puzzle.Length, $"puzzleGrid should have exactly {_puzzle.Length} child.");
            Assert.AreEqual(inputKeyboard.transform.childCount, _puzzle.sideLength, $"inputKeyboard should have exactly {_puzzle.sideLength} child.");
            Assert.AreEqual(puzzleGrid.transform.childCount, solvedSprite.Length, "solvedSprite should have exact same elements count as puzzleGrid child count.");

            // Set display grid and click events.
            for (int i = 0; i < _puzzle.Length; i++)
            {
                var go = puzzleGrid.transform.GetChild(i);
                var btn = go.GetComponent<Button>();
                var tmp = go.GetComponentInChildren<TextMeshProUGUI>();

                tmp.text = _puzzle[i].ToString();
                btn.transition = Selectable.Transition.None;
            }

            foreach (var i in _puzzle.removedCellIndex)
            {
                var go = puzzleGrid.transform.GetChild(i);
                var btn = go.GetComponent<Button>();
                var tmp = go.GetComponentInChildren<TextMeshProUGUI>();

                if (_puzzle[i] == 0)
                {
                    tmp.text = "";
                }
                tmp.color = Color.blue;
                btn.transition = Selectable.Transition.ColorTint;

                int index = i;
                btn.onClick.AddListener(delegate { OpenKeyboard(tmp.gameObject, index); });
            }

            // Set keyboard click events
            for (int i = 0; i < _puzzle.sideLength; i++)
            {
                int value = i + 1;
                inputKeyboard.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate { SetCell(value); });
            }

            _overlayButton.onClick.AddListener(delegate { SetCell(0); });    // Retract keyboard by setting 0.

            returnButton.onClick.AddListener(delegate { Close(); });
        }

        void Update()
        {
            // Read input from keyboard.
            if (_currentCellIndex != -1 && Input.inputString != "")
            {
                if (Int32.TryParse(Input.inputString, out int number) && number >= 0 && number < 10)
                {
                    SetCell(number);
                }
            }
        }

        void OnApplicationFocus(bool focus)
        {
            if (!focus)
            {
                SaveProgress();
            }
        }

        void OnApplicationQuit()
        {
            SaveProgress();
        }

        void OpenKeyboard(GameObject button, int index)
        {
            int x = _puzzle.sideLength;
            int i = index / x;
            int j = index % x;

            // if the element is located at the edge of the grid, offset it by 1 cell.
            i += (i == 0) ? 1 : ((i == x - 1) ? -1 : 0);
            j += (j == 0) ? 1 : ((j == x - 1) ? -1 : 0);

            inputKeyboard.transform.position = puzzleGrid.transform.GetChild(i * x + j).position;

            _overlayButton.interactable = true;
            _overlayImage.raycastTarget = true;
            inputKeyboard.SetActive(true);
            _currentCellIndex = index;
        }

        void SetCell(int value)
        {
            if (_currentCellIndex == -1)
            {
                throw new System.InvalidOperationException("You should set currentCellIndex before calling SetCell()");
            }

            if (value != 0)
            {
                if (_puzzle[_currentCellIndex] == 0)
                {
                    _filledCount++;
                }
                _puzzle[_currentCellIndex] = value;
                puzzleGrid.transform.GetChild(_currentCellIndex).GetChild(0).GetComponent<TextMeshProUGUI>().text = value.ToString();

                // Validate puzzle once all empty cells are filled.
                if (_filledCount == _puzzle.removedCellIndex.Length && _puzzle.Validate())
                {
                    PuzzleSolved();
                }
            }

            _currentCellIndex = -1;
            inputKeyboard.SetActive(false);
            _overlayImage.raycastTarget = false;
            _overlayButton.interactable = false;
        }

        void PuzzleSolved()
        {
            _puzzle.solved = true;
            if (File.Exists(Globals.PROGRESS_DATA_FILE_PATH))
            {
                File.Delete(Globals.PROGRESS_DATA_FILE_PATH);
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

                    btn.transition = Selectable.Transition.None;
                    btn.interactable = false;
                    flipOutSequence.Insert(solvedFlipAnimationInterval * i, go.transform.DORotate(new Vector3(0, 90, 0), solvedFlipAnimationInterval));

                    // bottom right triangle - start time reversed
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

                        go.GetComponent<Image>().color = Color.white;
                        flipInSequence.Insert(solvedFlipAnimationInterval * i, go.transform.DORotate(new Vector3(0, 0, 0), solvedFlipAnimationInterval));

                        // bottom right triangle - start time reversed
                        if (i < 8)
                        {
                            idx = 80 - idx;
                            go = puzzleGrid.transform.GetChild(idx);

                            go.GetComponent<Image>().color = Color.white;
                            flipInSequence.Insert(solvedFlipAnimationInterval * (16 - i), go.transform.DORotate(new Vector3(0, 0, 0), solvedFlipAnimationInterval));
                        }
                    }
                }
                flipInSequence.Play();
            }).Play();
        }

        void SaveProgress()
        {
            Debug.Log($"Saving game progress to {Globals.PROGRESS_DATA_FILE_PATH}...");
            var json = _puzzle.Serialize();
            File.WriteAllText(Globals.PROGRESS_DATA_FILE_PATH, json);
        }

        void Close()
        {
            if (!_puzzle.solved)
            {
                SaveProgress();
            }

            StartCoroutine(Globals.LoadSceneAsync(Globals.ENTRANCE_SCENE_NAME));
        }
    }
}