namespace Game.Writing {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using TMPro;
    using UnityEngine.UI;
    using UIAddition;
    using DG.Tweening;
#pragma warning disable 0649 // disable default value warning

    public class WritingPanelColors : MonoBehaviour {
        private Image image;


        [BoxGroup("Colors"), SerializeField]
        private Color defaultColor, startNodeColor, endNodeColor;

        private void Awake() {
            image = GetComponent<Image>();
        }

        /// <summary>
        /// Sets the panel colors for the given panel type
        /// </summary>
        /// <param name="forType"></param>
        public void AssignColors(WritingPanel.Type forType) {
            switch (forType) {
                case WritingPanel.Type.Default: SetPanelColor(defaultColor); break;
                case WritingPanel.Type.StartNode: SetPanelColor(startNodeColor); break;
                case WritingPanel.Type.EndNode: SetPanelColor(endNodeColor); break;
            }
        }

        private void SetPanelColor(Color color) {
            image.color = color;
        }


    }
}