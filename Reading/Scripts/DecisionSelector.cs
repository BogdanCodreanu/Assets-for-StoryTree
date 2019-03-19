namespace Game.Reading.DefaultPanelReader {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using UnityEngine.UI;
    using TMPro;
    using UIAddition;
    using RazzielModules.UIHelper;

    public class DecisionSelector : MonoBehaviour {
        [SerializeField, Required]
        private TMP_Text decisionText;

        private RectTransform TextRectTrans { get { return (RectTransform)decisionText.transform; } }
        private RectTransform rectTransform { get { return (RectTransform)transform; } }

        [SerializeField, PropertyTooltip("Offset used when scaling the main canvas to fit the text")]
        private float horizontalOffsetSize = 20f;

        [SerializeField, Required]
        private Button decisionButton;
        [SerializeField, Required]
        private ScrollRect buttonScrollRect;

        [SerializeField, Required]
        private PanelsQuickAnimations wasNotSelectedAnimation;

        public bool ButtonInteractible {
            get { return decisionButton.interactable; }
            set { decisionButton.interactable = value; }
        }

        /// <summary>
        /// Event called when this decision was selected.
        /// </summary>
        public Button.ButtonClickedEvent OnSelectDecision { get { return decisionButton.onClick; } }

        public void GiveText(string decisionText) {
            this.decisionText.text = decisionText;

            ResizeCanvasToFitTextHorizontally();
        }

        private void ResizeCanvasToFitTextHorizontally() {
            StartCoroutine(ResizeCoroutine());
        }

        private IEnumerator ResizeCoroutine() {
            yield return new WaitForEndOfFrame();
            rectTransform.sizeDelta = new Vector2(TextRectTrans.sizeDelta.x + horizontalOffsetSize,
                rectTransform.sizeDelta.y);
            buttonScrollRect.verticalNormalizedPosition = 1;
        }

        /// <summary>
        /// When another decision is selected.
        /// </summary>
        public void WasNotSelected() {
            wasNotSelectedAnimation.PlayAllAnimations();
        }

        /// <summary>
        /// When this decision is selected
        /// </summary>
        public void WasSelected() {

        }
    }
}
