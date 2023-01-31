using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Sudoku.Entrance
{
    public class DifficultyHandler : MonoBehaviour
    {
        public Button easyButton;
        public Button mediumButton;
        public Button hardButton;
        public Button closeButton;

        void Awake()
        {
            Assert.IsNotNull(easyButton, "You probably forget to set closeButton before you start the game.");
            Assert.IsNotNull(mediumButton, "You probably forget to set closeButton before you start the game.");
            Assert.IsNotNull(hardButton, "You probably forget to set closeButton before you start the game.");
            Assert.IsNotNull(closeButton, "You probably forget to set closeButton before you start the game.");
        }

        // Start is called before the first frame update
        void Start()
        {
            easyButton.onClick.AddListener(delegate { StartGame(0); });
            mediumButton.onClick.AddListener(delegate { StartGame(1); });
            hardButton.onClick.AddListener(delegate { StartGame(2); });
            closeButton.onClick.AddListener(delegate { Close(); });
        }

        #region Event Handlers
        void StartGame(int difficultyLevel)
        {
            PlayerPrefs.SetInt(Globals.DIFFICULTY_KEY, 10 * (difficultyLevel + 2) + 1);

            // TODO: Ask player to confirm progress deletion.
            if (File.Exists(Globals.PROGRESS_DATA_FILE_PATH))   // Delete existing progress before start game.
            {
                File.Delete(Globals.PROGRESS_DATA_FILE_PATH);
            }

            StartCoroutine(Globals.LoadSceneAsync(Globals.GAME_VIEW_SCENE_NAME));
        }

        void Close()
        {
            this.gameObject.SetActive(false);
        }
        #endregion
    }
}