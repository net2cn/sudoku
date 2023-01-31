using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Sudoku.Entrance
{
    public class AboutHandler : MonoBehaviour
    {
        public Button aboutBox;

        void Awake()
        {
            Assert.IsNotNull(aboutBox, "You probably forget to set closeButton before you start the game.");
        }

        // Start is called before the first frame update
        void Start()
        {
            aboutBox.onClick.AddListener(delegate { Close(); });
        }

        #region Event Handlers
        void Close()
        {
            this.gameObject.SetActive(false);
        }
        #endregion
    }
}