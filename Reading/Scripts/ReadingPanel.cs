namespace Game.Reading.DefaultPanelReader {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using UnityEngine.UI;
    using TMPro;
    using DG.Tweening;
    using UIAddition;
    using System;
    using RazzielModules.UIHelper;
#pragma warning disable 0649 // disable default value warning

    public class ReadingPanel : StoryVisualReader {
        [SerializeField, Required]
        private TMP_Text mainTextArea, selectedDecisionText;
        [SerializeField, Required]
        private ScrollRect readingScrollRect;

        [SerializeField, Required]
        private Button continueButton;
        [SerializeField, Required]
        private PanelsQuickAnimations fadeOutContinueButton, fadeInContinueButton;

        [SerializeField, Required]
        private DecisionsHolder decisionsHolder;

        [SerializeField, BoxGroup("Show Decisions Animations"), Required]
        private RectTransform decisionHolderTrans, panelInFrontOfDecisions;
        [SerializeField, BoxGroup("Show Decisions Animations")]
        private float showDecisionsAnimationTime = 1f;
        [SerializeField, BoxGroup("Show Decisions Animations"), PropertyTooltip("Delay time taken to " +
            "hide decisions after a decision has been selected")]
        private float hideDecisionsDelay = 1f;

        [SerializeField, Required, BoxGroup("Text Animations")]
        private PanelsQuickAnimations textFadeOut, textFadeIn;

        [SerializeField, MinValue(0)]
        private float decisionChangeTextDelay, quickNextChangeTextDelay;

        protected override float GiveNewLineDelayDecision {
            get {
                return decisionChangeTextDelay;
            }
        }

        protected override float GiveNewLineDelayQuickNext {
            get {
                return quickNextChangeTextDelay;
            }
        }

        private void Awake() {
            continueButton.onClick.AddListener(ContinueStory);
        }
        
        private void HideContinueButton() {
            continueButton.interactable = false;
            fadeOutContinueButton.PlayAllAnimations();
        }

        protected override void OnApplyNewStoryLineToScreen() {
            mainTextArea.text = CurrentStoryLine.Text;

            if (PreviouslySelectedDecision != null) {
                selectedDecisionText.text = PreviouslySelectedDecision.NameText;
            } else {
                selectedDecisionText.text = "";
            }

            textFadeIn.PlayAllAnimations();
            fadeInContinueButton.PlayAllAnimations();

            continueButton.interactable = false;
            this.ExecuteWithDelay(fadeInContinueButton.TotalAnimatingTime,
                delegate { continueButton.interactable = true; });
        }

        protected override void OnPressedContinue() {
            HideContinueButton();
        }

        protected override void ShowDecisions() {
            decisionsHolder.SpawnDecisions(CurrentStoryLine.Decisions);

            panelInFrontOfDecisions.DOAnchorPosY(decisionHolderTrans.sizeDelta.y, showDecisionsAnimationTime)
                .SetEase(Ease.InOutQuad);
            decisionsHolder.MakeDecisionsInteractible(showDecisionsAnimationTime);
        }
        protected override void HideDecisions() {
            panelInFrontOfDecisions.DOAnchorPosY(0, showDecisionsAnimationTime)
                .SetEase(Ease.InOutQuad).SetDelay(hideDecisionsDelay);
        }

        protected override void OnFinishedReadingStory() { }

        protected override void OnBeforeChangeText() {
            textFadeOut.PlayAllAnimations();
        }
    }
}
