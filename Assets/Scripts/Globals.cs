using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sudoku
{
    public static class Globals
    {
        public const string DIFFICULTY_KEY = "difficulty";
        public const string ENTRANCE_SCENE_NAME = "Entrance";
        public const string GAME_VIEW_SCENE_NAME = "GameView";

        public static string PROGRESS_DATA_FILE_PATH = Path.Combine(Application.persistentDataPath, "progress.json");

        public static IEnumerator LoadSceneAsync(string sceneName)
        {
            var loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            while (!loadOperation.isDone)
            {
                yield return null;
            }
        }
    }
}
