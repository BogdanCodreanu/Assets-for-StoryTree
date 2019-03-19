namespace Game.Writing {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using UnityEngine.UI;
    using DG.Tweening;
    using UIAddition;
    using RazzielModules.UIHelper;

    [RequireComponent(typeof(WritingPanel))]
    public class WritingPanelNextQuickPanel : MonoBehaviour, IWritingLinker, IWritingConditionPart {
        private WritingPanel writingPanel;

        [SerializeField, Required]
        private PanelsQuickAnimations showAnimation, hideAnimation;
        [SerializeField, Required]
        private Button selectNextPanelButton;

        /// <summary>
        /// Assigns interactibilty of the next panel button
        /// </summary>
        private bool NextPanelButtonInteractible { set { selectNextPanelButton.interactable = value; } }

        public WritingPanel PanelLinkedTo { get; private set; }
        /// <summary>
        /// This records the previously linked panel. This value is updated when the linker is unlinked
        /// </summary>
        private WritingPanel previouslyLinkedPanel = null;

        public enum State { OffScreen, OnScreen }
        private State state = State.OnScreen;

        private PanelsControllerAndLinker PanelsLinker { get { return writingPanel.PanelsController; } }
        public WritingPanel ContainingPanel { get { return writingPanel; } }


        [SerializeField, Required, PropertyTooltip("Where should the line renderer when linking start" +
            " from?")]
        private Transform startingLineRendPointLinker;
        public bool ShouldDisplayLineRend {
            get {
                return PanelLinkedTo != null || (PanelsLinker.InLinkingProcess &&
                    PanelsLinker.StartedLinkingLinker == (IWritingLinker)this);
            }
        }
        public Vector3 DisplayLineRendToPoint {
            get {
                return PanelLinkedTo != null ? PanelLinkedTo.LinkEndHead :
                    CameraMovement.MouseToWorldConversion(Input.mousePosition);
            }
        }
        public Vector3 DisplayLineRendFromPoint {
            get {
                return startingLineRendPointLinker.position;
            }
        }

        /// <summary>
        /// Does the writing panel uses next quick panel ?
        /// </summary>
        public bool UsesNextQuickPanel {
            get { return ContainingPanel.DecisionsHolderController.Decisions.Count == 0; }
        }
        /// <summary>
        /// The index in the array of all story lines of the panel linked to. Returns -1 if it's no panel assigned
        /// </summary>
        public int NextPanelStoryLineIndex {
            get {
                return PanelLinkedTo == null ? -1 :
                    PanelLinkedTo.StoryLineIndex;
            }
        }

        private void Awake() {
            writingPanel = GetComponent<WritingPanel>();
        }

        /// <summary>
        /// Hides the panel if the number of decisions is greater then one, otherwise shows it.
        /// </summary>
        /// <param name="nrOfExistingDecisions">Number of existing decisions</param>
        public void ShowOrHideByExistingDecisions(int nrOfExistingDecisions) {
            if (nrOfExistingDecisions <= 0) {
                ShowPart();
            } else {
                HidePart();
            }
        }

        /// <summary>
        /// Shows the part
        /// </summary>
        public void ShowPart() {
            if (state == State.OnScreen)
                return;
            state = State.OnScreen;
            NextPanelButtonInteractible = true;
            showAnimation.PlayAllAnimations();
        }

        /// <summary>
        /// Hides the part
        /// </summary>
        public void HidePart() {
            if (state == State.OffScreen)
                return;
            Unlink();
            state = State.OffScreen;
            NextPanelButtonInteractible = false;
            hideAnimation.PlayAllAnimations();
        }

        /// <summary>
        /// Called by start linking button
        /// </summary>
        public void StartLinking() {
            previouslyLinkedPanel = PanelLinkedTo;
            Unlink();
            PanelsLinker.BeginLinking(this);
        }

        /// <summary>
        /// When linking has been completed. And we need to reset variables.
        /// If you want to link a linker to a panel use panel.LinkWith(linker) !!
        /// </summary>
        public void LinkWith(WritingPanel panel) {
            PanelLinkedTo = panel;
        }

        /// <summary>
        /// Unlinks the current link.
        /// </summary>
        public void Unlink() {
            if (PanelLinkedTo == null)
                return;
            WritingPanel wasLinkedTo = PanelLinkedTo;
            PanelLinkedTo = null;
            wasLinkedTo.UnlinkPanel(this);
        }

        public bool IsDifferentLink(WritingPanel newLinkedPanel) {
            return previouslyLinkedPanel != newLinkedPanel;
        }

        public void AssignDataByStoryLine(StoryLine storyLine, List<WritingPanel> allPanels) {
            if (storyLine.NextQuickStoryLineIndex != -1) {
                allPanels[storyLine.NextQuickStoryLineIndex].LinkPanelToLinker(this);
            }
        }

        public void CheckIfPartIsValidForSaving() {
            if (PanelLinkedTo == null) {
                throw new InvalidPartForPlayingException(
                    "A writing panel is required to have a connection to another node",
                    "Connect the panel to another panel. If you don't want a connection, then" +
                    " assign this panel to be \'final\'", true,
                    selectNextPanelButton.transform.position);
            }
        }
    }
}