namespace Game.Writing {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using UnityEngine.UI;
    using TMPro;
    using DG.Tweening;
    using UIAddition;
    using RazzielModules.UIHelper;

    public class WrittenDecision : MonoBehaviour, IWritingLinker, IWritingConditionPart {
        private RectTransform rectTransform { get { return (RectTransform)transform; } }
        /// <summary>
        /// To know what size delta does this rect transform have as default.
        /// Because it's changed while playing animations.
        /// </summary>
        private Vector2 defaultSizeDelta;

        [SerializeField, Required]
        private TMP_InputField decisionText;
        /// <summary>
        /// Used to compare the previous and the new given text.
        /// In order to know if we should register an undo.
        /// </summary>
        private string previousDecisionText;

        private WrittenDecisions decisionsController;
        /// <summary>
        /// The panel that contains this decision
        /// </summary>
        public WritingPanel ContainingPanel { get { return decisionsController.WritingPanel; } }

        [SerializeField, Required]
        private PanelsQuickAnimations animationSpawn, animationDelete;

        [SerializeField, Required]
        private ResizePanel resizeHandle;

        [SerializeField, Required, PropertyTooltip("Image of the button that selects a node")]
        private Image selectNodeImage;
        private Color initialSelectNodeColor;

        /// <summary>
        /// The panel that this decision links with
        /// </summary>
        [ReadOnly, ShowInInspector]
        public WritingPanel PanelLinkedTo { get; private set; }
        /// <summary>
        /// This records the previously linked panel. This value is updated when the linker is unlinked
        /// </summary>
        private WritingPanel previouslyLinkedPanel = null;

        [SerializeField, Required, PropertyTooltip("Where should the line renderer when linking start" +
            " from?")]
        private Transform startingLineRendPointLinker;

        public bool ShouldDisplayLineRend {
            get {
                return PanelLinkedTo != null || (WrittenPanelsControllerAndLinker.InLinkingProcess &&
                    WrittenPanelsControllerAndLinker.StartedLinkingLinker == (IWritingLinker)this);
            }
        }
        public Vector3 DisplayLineRendToPoint {
            get {
                return PanelLinkedTo != null ? PanelLinkedTo.LinkEndHead :
                    CameraMovement.MouseToWorldConversion(Input.mousePosition); }
        }
        public Vector3 DisplayLineRendFromPoint {
            get {
                return startingLineRendPointLinker.position;
            }
        }

        private PanelsControllerAndLinker WrittenPanelsControllerAndLinker {
            get {
                return decisionsController.WritingPanel.PanelsController;
            }
        }

        [SerializeField, PropertyTooltip("On deletion, animation will start and then after this " +
            "many seconds, the gameobject will be destroied")]
        private float deathDelay;

        private void Awake() {
            initialSelectNodeColor = selectNodeImage.color;
            AddUndoRedoFunctionality();
        }

        private void AddUndoRedoFunctionality() {
            decisionText.onSelect.AddListener(delegate (string value) { previousDecisionText = value; });
            decisionText.onEndEdit.AddListener(delegate (string newValue) {
                if (previousDecisionText != newValue) {
                    RegisterActions.Instance.RegisterNewState();
                }
            });
        }

        public void InitOnCreation(WrittenDecisions controller, bool useAnimation) {
            decisionsController = controller;
            defaultSizeDelta = rectTransform.sizeDelta;
            if (useAnimation) {
                animationSpawn.PlayAllAnimations();
            }
        }

        /// <summary>
        /// Delete decision via UI button
        /// </summary>
        public void DeleteViaButton() {
            DeleteDecision(true);
            RegisterActions.Instance.RegisterNewState();
        }

        /// <summary>
        /// Deletes this decision
        /// </summary>
        /// <param name="useAnimation">With or without animation</param>
        public void DeleteDecision(bool useAnimation) {
            Unlink();
            decisionsController.RemoveDecisionVariable(this);

            if (useAnimation) {
                animationDelete.PlayAllAnimations();
                this.ExecuteWithDelay(deathDelay, delegate { decisionsController.DeleteDecision(this); });
            } else {
                decisionsController.DeleteDecision(this);
            }
        }
                
        /// <summary>
        /// Creates the decision class and gets it
        /// </summary>
        public Decision GetTheDecision() {
            Decision decision = new Decision() {
                NameText = decisionText.text,
                NextStoryLineIndex = -1,
                GraphPosition = new GraphPosition((RectTransform)transform),
            };

            if (resizeHandle.ValidMinParentRectTrans(rectTransform)) {
                decision.GraphPosition = new GraphPosition(rectTransform);
            } else {
                decision.GraphPosition = new GraphPosition(rectTransform,
                    defaultSizeDelta);
            }

            if (PanelLinkedTo != null) {
                decision.NextStoryLineIndex = PanelLinkedTo.StoryLineIndex;
            }

            return decision;
        }

        /// <summary>
        /// Called by start linking button.
        /// </summary>
        public void StartLinking() {
            previouslyLinkedPanel = PanelLinkedTo;
            Unlink(); // unlink previous decision
            WrittenPanelsControllerAndLinker.BeginLinking(this);
        }

        /// <summary>
        /// Colors to the selection node for linking
        /// </summary>
        public void SetSelectNodeButtonColor(Color to) {
            selectNodeImage.DOColor(to, .1f);
        }
        /// <summary>
        /// Resets colors to the selection node.
        /// </summary>
        public void ResetSelectNodeButtonColor() {
            selectNodeImage.DOColor(initialSelectNodeColor, .1f);
        }

        /// <summary>
        /// When linking has been completed. And we need to reset variables.
        /// If you want to link a linker to a panel use panel.LinkWith(linker) !!
        /// </summary>
        public void LinkWith(WritingPanel panel) {
            PanelLinkedTo = panel;
        }

        /// <summary>
        /// Unlinks the decision from the current link.
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

        public void AssignDataByDecision(Decision decision, List<WritingPanel> allPanels) {
            decisionText.text = decision.NameText;
            decision.GraphPosition.ApplyPositioning((RectTransform)transform);

            if (decision.NextStoryLineIndex != -1) {
                allPanels[decision.NextStoryLineIndex].LinkPanelToLinker(this);
            }
        }

        public void CheckIfPartIsValidForSaving() {
            if (PanelLinkedTo == null) {
                throw new InvalidPartForPlayingException(
                    "A decision is required to have a connection to another panel",
                    "Connect the decision to another panel", true,
                    transform.position);
            }
            if (string.IsNullOrEmpty(decisionText.text)) {
                throw new InvalidPartForPlayingException(
                    "A decision is not allowed to have empty text", "Add some text to the decision", true,
                    transform.position);
            }
        }
    }
}