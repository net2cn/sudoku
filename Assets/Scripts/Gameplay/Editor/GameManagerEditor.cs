using DG.DOTweenEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Sudoku.Gameplay
{
    [CustomEditor(typeof(GameManager))]
    public class GameManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();

            GUILayout.Label("Gameplay preview controls");
            EditorGUI.BeginDisabledGroup(!Application.isPlaying);

            GameManager manager = (GameManager)target;
            if (GUILayout.Button("Preview solved animation"))
            {
                manager.PuzzleSolved();
            }

            EditorGUI.EndDisabledGroup();
        }
    }
}