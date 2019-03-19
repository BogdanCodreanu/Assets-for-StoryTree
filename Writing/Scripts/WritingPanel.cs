namespace Game.Writing {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using TMPro;
    using UnityEngine.UI;
    using UIAddition;
    using RazzielModules.UIHelper;
#pragma warning disable 0649 // disable default value warning

    public class WritingPanel : MonoBehaviour, IWritingConditionPart {
        private WritingStoryCreator storyCreator;
        /// <summary>
        /// The index in the array of all story lines.
        /// </summary>
        public int StoryLineIndex {
            get {
                return storyCreator.GetIndexOfPanel(this);
            }
        }

        private RectTransform rectTransform { get { return (RectTransform)transform; } }
        /// <summary>
        /// To know what size delta does this rect transform have as default.
        /// Because it's changed while playing animations.
        /// </summary>
        private Vector2 defaultSizeDelta;
        [SerializeField, Required]
        private ResizePanel resizeHandle;

        public enum Type { Default, StartNode, EndNode }
        public Type NodeType { get; private set; }

        /// <summary>
        /// The panels controller and linker.
        /// </summary>
        public PanelsControllerAndLinker PanelsController 
            { get { return storyCreator.ControllerAndLinker; } }

        [SerializeField, Required]
        private TMP_InputField storyText;
        /// <summary>
        /// Used to compare the previous and the new given text.
        /// In order to know if we should register an undo.
        /// </summary>
        private string previousStoryText;
        public string CurrentStoryText { get { return storyText.text; } }

        /// <summary>
        /// Middle position of the panel.
        /// </summary>
        public Vector3 CenterPanelPosition { get { return storyText.transform.position; } }
        /// <summary>
        /// Is this node the start node?
        /// </summary>
        public bool IsStartNode { get { return NodeType == Type.StartNode; } }
        [SerializeField, Required]
        private Button deleteButton, isFinalButton;

        [SerializeField, Required]
        private PanelsQuickAnimations createAnimation, destroyAnimation;

        [SerializeField, Required]
        private LinkingEndPreview linkingEndPreview;
        [SerializeField, Required, PropertyTooltip("Where does the linking line comes to?")]
        private Transform linkingEndHead;
        public Vector3 LinkEndHead { get { return linkingEndHead.position; } }

        /// <summary>
        /// The decisions that have a link to this node.
        /// </summary>
        [ReadOnly, ShowInInspector]
        public HashSet<IWritingLinker> LinkersLinkedTo { get { return linkersLinkedTo; } }
        private HashSet<IWritingLinker> linkersLinkedTo = new HashSet<IWritingLinker>();


        /// <summary>
        /// Decisions controller
        /// </summary>
        [SerializeField, Required]
        private WrittenDecisions decisionsHolder;
        /// <summary>
        /// Decisions Controller / Holder
        /// </summary>
        public WrittenDecisions DecisionsHolderController { get { return decisionsHolder; } }

        [SerializeField, PropertyTooltip("On deletion, animation will start and then after this many seconds, " +
            "the gameobject will be destroied")]
        private float deathDelay;

        private WritingPanelColors panelColorsController;

        /// <summary>
        /// If this panel has a link from a decision.
        /// </summary>
        public bool HasLinkUpwards { get { return linkersLinkedTo.Count > 0; } }

        private WritingPanelNextQuickPanel nextQuickPanel;

        [SerializeField, Required]
        private WritingCanvas containingCanvas;
        /// <summary>
        /// Used to delete the canvas when wanting to delete the panel.
        /// </summary>
        public WritingCanvas ContainingCanvas { get { return containingCanvas; } }

        private void Awake() {
            panelColorsController = GetComponent<WritingPanelColors>();
            nextQuickPanel = GetComponent<WritingPanelNextQuickPanel>();
            AddUndoRedoFunctionality();
        }

        private void AddUndoRedoFunctionality() {
            isFinalButton.onClick.AddListener(RegisterActions.Instance.RegisterNewState);
            deleteButton.onClick.AddListener(RegisterActions.Instance.RegisterNewState);

            storyText.onSelect.AddListener(delegate(string value) { previousStoryText = value; });
            storyText.onEndEdit.AddListener(delegate(string newValue) {
                if (previousStoryText != newValue) {
                    RegisterActions.Instance.RegisterNewState();
                }
            });
        }

        /// <summary>
        /// Initializes panel on creation
        /// </summary>
        /// <param name="creator">The creator</param>
        /// <param name="useAnimations">Whether or not to use animations on spawning</param>
        public void InitOnCreation(WritingStoryCreator creator, bool useAnimations) {
            storyCreator = creator;
            defaultSizeDelta = rectTransform.sizeDelta;

            if (useAnimations) {
                createAnimation.PlayAllAnimations();
                decisionsHolder.PlayShowAnimation();
            }

        }

        /// <summary>
        /// Deletes this writing panel
        /// </summary>
        /// <param name="useAnimation">Use animation</param>
        public void DeletePanel(bool useAnimation) {
            if (!CanDeleteNode)
                return;

            UnlinkPanelFromAll();
            storyCreator.RemoveWritingPanel(this);

            if (useAnimation) {
                decisionsHolder.PlayHideAnimation();
                destroyAnimation.PlayAllAnimations();
                this.ExecuteWithDelay(deathDelay, delegate { storyCreator.DeleteWritingPanel(this); });
            } else {
                storyCreator.DeleteWritingPanel(this);
            }
        }

        private bool CanDeleteNode {
            get {
                return !IsStartNode;
            }
        }


        /// <summary>
        /// Creates the story line class and gets it
        /// </summary>
        public StoryLine GetTheStoryLine() {
            StoryLine storyLine = new StoryLine {
                StoryLineIndex = this.StoryLineIndex,
                Text = storyText.text,
                NodeType = NodeType,
                Decisions = null,
                UsesDecisions = decisionsHolder.UsesDecisions,
                NextQuickStoryLineIndex = -1,
            };

            if (resizeHandle.ValidMinParentRectTrans(rectTransform)) {
                storyLine.GraphPosition = new GraphPosition(rectTransform);
            } else {
                storyLine.GraphPosition = new GraphPosition(rectTransform,
                    defaultSizeDelta);
            }

            if (storyLine.UsesDecisions) {
                storyLine.Decisions = decisionsHolder.DecisionsValuesForSerialization();
            } else {
                storyLine.NextQuickStoryLineIndex = nextQuickPanel.NextPanelStoryLineIndex;
            }

            return storyLine;
        }

        /// <summary>
        /// Assigns data from a storyline. Does not create or links anything.
        /// </summary>
        public void AssignDataByStoryLine(StoryLine storyLine) {
            storyLine.GraphPosition.ApplyPositioning((RectTransform)transform);
            storyText.text = storyLine.Text;
            SetNodeType(storyLine.NodeType);
        }

        /// <summary>
        /// Creates decisions and links panels or makes the next quick link
        /// . AFTER all writing panels were assigned and we're ready
        /// for linking.
        /// </summary>
        public void AssignDecisionsByStoryLineOrQuickNextPanel(StoryLine storyLine,
            List<WritingPanel> allPanels) {

            if (storyLine.UsesDecisions) {
                foreach (Decision decision in storyLine.Decisions) {
                    decisionsHolder.SpawnDecision(false).AssignDataByDecision(decision, allPanels);
                }
            } else {
                nextQuickPanel.AssignDataByStoryLine(storyLine, allPanels);
            }
        }

        /// <summary>
        /// Makes this the start node. Takes care of other existing start nodes
        /// </summary>
        public void SetAsStartNode() {
            SetNodeType(Type.StartNode);
            PanelsController.ClearsPreviousStartNode(this);
            storyCreator.SetStartingPanelVariable(this);
            UnlinkPanelFromAll();
        }

        /// <summary>
        /// Sets the node type to a new type
        /// </summary>
        public void SetNodeType(Type newType) {
            if (newType == Type.StartNode) {
                isFinalButton.interactable = deleteButton.interactable = false;
            } else {
                isFinalButton.interactable = deleteButton.interactable = true;
            }

            if ((NodeType == Type.Default && newType == Type.EndNode) ||
                (NodeType == Type.EndNode && newType == Type.Default)) {
                HideOrShowDecisionsWhenEndNodeChanged();
            }

            NodeType = newType;
            panelColorsController.AssignColors(newType);
        }
        
        /// <summary>
        /// Raises the preview for linking
        /// </summary>
        /// <param name="startedLinker"></param>
        public void ActivateLinkingPreview(IWritingLinker startedLinker) {
            if (!ValidForLinkingFrom(startedLinker)) {
                return;
            }

            linkingEndPreview.ToggleState(true);
        }

        private bool ValidForLinkingFrom(IWritingLinker llinker) {
            if (IsStartNode || IsRecursiveParentOf(llinker.ContainingPanel))
                return false;


            return true;
        }

        public void HideLinkingPreview() {
            linkingEndPreview.ToggleState(false);
        }

        /// <summary>
        /// When linking has been completed. This also tells the linker to finish linking.
        /// This auto-calls linker.linkWith() and only this one should be called.
        /// </summary>
        public void LinkPanelToLinker(IWritingLinker linker) {
            LinkersLinkedTo.Add(linker);
            linker.LinkWith(this); // only here it's allowed to make this call
        }

        /// <summary>
        /// Unlinks panel from any decision that has a link to it.
        /// </summary>
        /// <param name="fromLinker">Removes this decision link</param>
        public void UnlinkPanel(IWritingLinker fromLinker, bool alsoRemoveFromList = true) {
            if (!LinkersLinkedTo.Contains(fromLinker))
                return;

            IWritingLinker wasLinkedTo = fromLinker;
            LinkersLinkedTo.Remove(fromLinker);
            wasLinkedTo.Unlink();
        }

        /// <summary>
        /// Unlinks all decisions from this panel
        /// </summary>
        public void UnlinkPanelFromAll() {
            HashSet<IWritingLinker> willBeRemovedLinkers = new HashSet<IWritingLinker>(linkersLinkedTo);

            foreach (IWritingLinker linker in willBeRemovedLinkers) {
                linker.Unlink();
            }
            LinkersLinkedTo.Clear();
        }

        /// <summary>
        /// Is this node a child by linking to the given panel?
        /// </summary>
        /// <param name="fatherPanel">The possible father panel</param>
        public bool IsRecursiveChildOf(WritingPanel fatherPanel) {
            if (fatherPanel == this) {
                return true;
            }

            if (!HasLinkUpwards) {
                return false;
            }

            foreach (IWritingLinker linkedTo in LinkersLinkedTo) {
                if (linkedTo.ContainingPanel.IsRecursiveChildOf(fatherPanel))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Is this node a parent by linking to the given panel?
        /// </summary>
        /// <param name="childPanel">The possible child panel</param>
        public bool IsRecursiveParentOf(WritingPanel childPanel) {
            return childPanel.IsRecursiveChildOf(this);
        }

        /// <summary>
        /// Gets all linked children in a DFS order
        /// </summary>
        /// <param name="currentChildren">Should be an empty list on first call. This is the
        /// list result</param>
        public void GetAllLinkedChildren(ref List<WritingPanel> currentChildren) {
            foreach (IWritingLinker linker in decisionsHolder.Decisions) {
                if (linker.PanelLinkedTo) {
                    currentChildren.Add(linker.PanelLinkedTo);
                    linker.PanelLinkedTo.GetAllLinkedChildren(ref currentChildren);
                }
            }
            if (nextQuickPanel.PanelLinkedTo) {
                currentChildren.Add(nextQuickPanel.PanelLinkedTo);
                nextQuickPanel.PanelLinkedTo.GetAllLinkedChildren(ref currentChildren);
            }
        }
        
        /// <summary>
        /// Hides or shows decisions when end node value changed
        /// </summary>
        private void HideOrShowDecisionsWhenEndNodeChanged() {
            if (NodeType == Type.Default) {
                decisionsHolder.PlayHideAnimation();
            } else { // if it's an end node
                decisionsHolder.PlayShowAnimation();
            }
        }

        public void ToggleIsFinalNode() {
            SetNodeType(NodeType == Type.Default ? Type.EndNode : Type.Default);
        }

        public void CheckIfPartIsValidForSaving() {
            if (string.IsNullOrEmpty(storyText.text)) {
                throw new InvalidPartForPlayingException(
                    "A writing panel is not allowed to have empty story text",
                    "Add some text to this story text",
                    true, CenterPanelPosition);
            }

            // checking if children are assigned correctly
            if (NodeType == Type.Default || NodeType == Type.StartNode) {
                if (decisionsHolder.UsesDecisions) {
                    foreach (WrittenDecision decision in decisionsHolder.Decisions) {
                        decision.CheckIfPartIsValidForSaving();
                    }
                } else {
                    nextQuickPanel.CheckIfPartIsValidForSaving();
                }
            }
        }
        
    }
}