namespace RazzielModules.UIHelper {
    using System.Collections;
    using System.Collections.Generic;
    using System;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using DG.Tweening;
    using TMPro;
    using UnityEngine.UI;
#pragma warning disable 0649 // disable default value warning

    public class UIHelper : MonoBehaviour {
        [SerializeField, MinValue(0), BoxGroup("Information Message")]
        private float infoMsgDelayToFade, infoMsgFadeDuration,
            infoMsgUpGoingValue;
        [SerializeField, BoxGroup("Information Message"), Required]
        private TMP_Text textObjectPreset, errorObjectPreset;
        public enum InfoMessageType { Info, Error }

        [SerializeField, Required, BoxGroup("Confirm Action")]
        private UIHelperConfrimActionPanel confirmActionPanelPreset;
        [SerializeField, Required, BoxGroup("Confirm Action")]
        private Image confirmActionPanelBackgroundImage;
        [SerializeField, BoxGroup("Confirm Action")]
        private float confirmActionBgFadeDuration = .2f;
        private float initialConfirmPanelBackgroundAlpha;

        [SerializeField, BoxGroup("Tooltip")]
        private UIHelperTooltipObject tooltipPreset;

        public static UIHelper Instance { get; private set; }

        private void Awake() {
            Instance = this;
            initialConfirmPanelBackgroundAlpha = confirmActionPanelBackgroundImage.color.a;
        }

        /// <summary>
        /// Displays a quick information message on screen.
        /// </summary>
        public void DisplayInformationMessage(string msg, InfoMessageType type = InfoMessageType.Info) {
            TMP_Text spawn;
            switch (type) {
                case InfoMessageType.Error:
                    spawn = errorObjectPreset.gameObject.CreateObjectFromPreset()
                        .GetComponent<TMP_Text>(); break;
                default:
                    spawn = textObjectPreset.gameObject.CreateObjectFromPreset()
                        .GetComponent<TMP_Text>(); break;
            }


            spawn.text = msg;
            DOTween.Sequence()
                .Append(spawn.rectTransform.DOAnchorPosY(spawn.rectTransform.anchoredPosition.y +
                infoMsgUpGoingValue, infoMsgDelayToFade + infoMsgFadeDuration)
                    .SetEase(Ease.OutQuad))
                .Insert(0, spawn.DOFade(0, infoMsgFadeDuration).SetDelay(infoMsgDelayToFade)
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(delegate { Destroy(spawn.gameObject); }));
        }

        /// <summary>
        /// Shows a popup that asks a question for the player.
        /// </summary>
        /// <param name="questionMessage">e.g. "are you sure you want to delete object?"</param>
        /// <param name="onClickedYes">Action that happens if player clicks yes</param>
        /// <param name="onClickedCancel">Can be null.</param>
        public void DisplayConfirmActionPanel(string questionMessage, Action onClickedYes,
            Action onClickedCancel = null) {
            UIHelperConfrimActionPanel spawn =
                confirmActionPanelPreset.gameObject.CreateObjectFromPreset()
                .GetComponent<UIHelperConfrimActionPanel>();

            spawn.Initiazile(this, questionMessage, onClickedYes,
                onClickedCancel, delegate { CloseConfirmActionPanel(); });

            confirmActionPanelBackgroundImage.gameObject.SetActive(true);

            confirmActionPanelBackgroundImage.color = new Color
                (confirmActionPanelBackgroundImage.color.r, confirmActionPanelBackgroundImage.color.g,
                confirmActionPanelBackgroundImage.color.b, initialConfirmPanelBackgroundAlpha);

            confirmActionPanelBackgroundImage.DOFade(0, confirmActionBgFadeDuration).From();
        }

        /// <summary>
        /// Closes current confirm action panel. Called only by other objects
        /// </summary>
        private void CloseConfirmActionPanel() {
            confirmActionPanelBackgroundImage.DOFade(0, confirmActionBgFadeDuration)
                .OnComplete(delegate { confirmActionPanelBackgroundImage.gameObject.SetActive(false); });
        }

        public UIHelperTooltipObject CreateTooltip(string message) {
            UIHelperTooltipObject spawn = 
                tooltipPreset.gameObject.CreateObjectFromPreset().GetComponent<UIHelperTooltipObject>();
            spawn.Init(message);
            return spawn;
        }
    }

    public static class GameObjectExt {
        /// <summary>
        /// Creates a duplicate object of this game object, sets it as active and inherits the same
        /// transform data, including the parent
        /// </summary>
        /// <returns>The spawned object</returns>
        public static GameObject CreateObjectFromPreset(this GameObject presetObject) {
            GameObject spawn = GameObject.Instantiate(presetObject.gameObject,
                presetObject.transform.position, presetObject.transform.rotation,
                presetObject.transform.parent);

            spawn.gameObject.SetActive(true);
            return spawn;
        }
    }
}
