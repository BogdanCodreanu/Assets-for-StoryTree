namespace Game.Reading {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
#pragma warning disable 0649 // disable default value warning

    public abstract class StoryVisualReader : MonoBehaviour {
        /// <summary>
        /// This is the latest story line given. It's the currently read story line.
        /// Currently given storyLine.
        /// </summary>
        protected StoryLine CurrentStoryLine { get; private set; }
        /// <summary>
        /// This is the last decision that has been selected.
        /// </summary>
        protected Decision PreviouslySelectedDecision { get; private set; }

        protected StoryPath StoryPath { get { return storyPlayer.StoryPath; } }


        [SerializeField, Required]
        private StoryPlayer storyPlayer;


        /// <summary>
        /// The delay to recieve a new story line after a decision has been selected
        /// </summary>
        protected abstract float GiveNewLineDelayDecision { get; }
        /// <summary>
        /// The delay to recieve a new story line after continue to quick next panel has been selected
        /// </summary>
        protected abstract float GiveNewLineDelayQuickNext { get; }

        public void GiveStoryLine(StoryLine storyLine) {
            CurrentStoryLine = storyLine;
            OnApplyNewStoryLineToScreen();
        }

        /// <summary>
        /// Display the new story line text on screen. Access the story line using CurrentStoryLine.
        /// </summary>
        protected abstract void OnApplyNewStoryLineToScreen();

        /// <summary>
        /// This happends when the player chooses to continue the story.
        /// Shows decisions, goes to quick next panel or ends the story.
        /// </summary>
        public void ContinueStory() {
            OnPressedContinue();
            if (CurrentStoryLine.IsNodeFinal) {
                storyPlayer.PressedEndButton();
                OnFinishedReadingStory();
                return;
            }

            if (CurrentStoryLine.UsesDecisions) {
                ShowDecisions();
            } else {
                GoToQuickNextPanel();
            }
        }

        /// <summary>
        /// Can be used to hide continue button, animations.
        /// Is called before "OnShowDecisions" or "OnQuickNextContinue" or "OnFinishedReading"
        /// </summary>
        protected abstract void OnPressedContinue();

        /// <summary>
        /// This is called just after a decision has been selected or the quick next panel
        /// button has been pressed.
        /// It's called before "OnQuickNext" or "OnDecisionSelected"
        /// </summary>
        protected virtual void OnBeforeChangeText() { }

        /// <summary>
        /// Shows the decision options and lets the player choose a decision.
        /// </summary>
        protected abstract void ShowDecisions();

        /// <summary>
        /// A decision has been selected.
        /// Can use this to animate the selected decision and animate other not selected decisions
        /// </summary>
        /// <param name="selectedDecision">The selected decision.</param>
        protected virtual void OnDecisionHasBeenSelected(Decision selectedDecision) { }

        /// <summary>
        /// Hides all decisions. Does not care which decision has been chosen.
        /// </summary>
        protected abstract void HideDecisions();

        /// <summary>
        /// Continues the story with a quick next story line.
        /// Can use this to apply effects or animations.
        /// </summary>
        protected virtual void OnQuickNextContinue() { }

        /// <summary>
        /// When the continue button has been pressed and it was a final node.
        /// The story has ended.
        /// </summary>
        protected abstract void OnFinishedReadingStory();


        /// <summary>
        /// This should be called when a decision button has been pressed.
        /// A decision has been selected.
        /// </summary>
        /// <param name="selectedDecision">The selected decision</param>
        public void HasSelectedDecision(Decision selectedDecision) {
            storyPlayer.DecisionWasSelected(selectedDecision);
            PreviouslySelectedDecision = selectedDecision;

            HideDecisions();

            OnBeforeChangeText();
            OnDecisionHasBeenSelected(selectedDecision);

            this.ExecuteWithDelay(GiveNewLineDelayDecision, storyPlayer.AskForNewLine);
        }

        /// <summary>
        /// Goes to the quick next panel.
        /// </summary>
        private void GoToQuickNextPanel() {
            storyPlayer.DecisionWasSelected(null);
            PreviouslySelectedDecision = null;

            OnBeforeChangeText();
            OnQuickNextContinue();

            this.ExecuteWithDelay(GiveNewLineDelayQuickNext, storyPlayer.AskForNewLine);
        }


        /// <summary>
        /// Recieve loaded story path
        /// </summary>
        /// <param name="storyPath">Loaded story path</param>
        public void RecieveStoryPath(StoryPath storyPath) {
            OnRecieveStoryPath(storyPath);
        }

        /// <summary>
        /// What should happend when a story path has been recieved (effects / load 
        /// previous text on an open book)
        /// </summary>
        /// <param name="storyPath"></param>
        protected virtual void OnRecieveStoryPath(StoryPath storyPath) { }
    }
}
