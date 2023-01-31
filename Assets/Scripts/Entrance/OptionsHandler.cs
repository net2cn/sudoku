using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Sudoku.Entrance
{
    public class OptionsHandler : MonoBehaviour
    {
        public Button newGameButton;
        public Button resumeButton;
        public Button aboutButton;
        public Button exitButton;

        public GameObject aboutGroup;
        public GameObject difficultyGroup;

        void Awake()
        {
            Assert.IsNotNull(newGameButton, "You probably forget to set newGameButton before you start the game.");
            Assert.IsNotNull(resumeButton, "You probably forget to set resumeButton before you start the game.");
            Assert.IsNotNull(aboutButton, "You probably forget to set aboutButton before you start the game.");
            Assert.IsNotNull(exitButton, "You probably forget to set exitButton before you start the game.");

            Assert.IsNotNull(aboutGroup, "You probably forget to set aboutGroup before you start the game.");
            Assert.IsNotNull(difficultyGroup, "You probably forget to set difficultyGroup before you start the game.");
        }

        // Start is called before the first frame update
        void Start()
        {
            newGameButton.onClick.AddListener(delegate { NewGame(); });
            resumeButton.onClick.AddListener(delegate { StartCoroutine(Globals.LoadSceneAsync(Globals.GAME_VIEW_SCENE_NAME)); });
            aboutButton.onClick.AddListener(delegate { About(); });
            exitButton.onClick.AddListener(delegate { Exit(); });

            if (!File.Exists(Globals.PROGRESS_DATA_FILE_PATH))
            {
                resumeButton.interactable = false;
            }
        }

        #region Event Handlers
        void NewGame()
        {
            difficultyGroup.SetActive(true);
        }

        void About()
        {
            aboutGroup.SetActive(true);
        }

        void Exit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        #endregion
    }
}