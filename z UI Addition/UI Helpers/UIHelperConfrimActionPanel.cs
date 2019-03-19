namespace RazzielModules.UIHelper {
    using System.Collections;
    using System.Collections.Generic;
    using System;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using DG.Tweening;
    using TMPro;
    using UnityEngine.UI;
    using Game.UIAddition;
#pragma warning disable 0649 // disable default value warning

    public class UIHelperConfrimActionPanel : MonoBehaviour {
        [SerializeField, Required]
        private Button yesButton, cancelButton;
        [SerializeField, Required]
        private TMP_Text questionMessageText;

        [SerializeField, Required]
        private PanelsQuickAnimations startAnimations, closeAnimations;

        [SerializeField]
        private float delayToKillAfterClose = .2f;


        public void Initiazile(UIHelper creator, string questionMsg, Action onClickedYes, Action onClickedCancel,
            Action fadeBackground) {
            yesButton.onClick.AddListener(delegate {
                ClosePanel();
                fadeBackground();
                onClickedYes();
            });

            cancelButton.onClick.AddListener(delegate {
                ClosePanel();
                fadeBackground();
                if (onClickedCancel != null)
                    onClickedCancel();
            });

            questionMessageText.text = questionMsg;
            ShowPanel();
        }

        private void ShowPanel() {
            startAnimations.PlayAllAnimations();
        }

        private void ClosePanel() {
            yesButton.interactable = cancelButton.interactable = false;
            closeAnimations.PlayAllAnimations();
            Destroy(gameObject, delayToKillAfterClose);
        }
    }
}
