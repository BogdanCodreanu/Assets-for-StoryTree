namespace Game.Writing {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using UIAddition;

    public class PanelsControllerAndLinker : MonoBehaviour {
        [SerializeField, Required]
        private WritingStoryCreator writingStoryCreator;
        [SerializeField, Required]
        private CustomInputModule sceneInputModule;
        [SerializeField, Required, SceneObjectsOnly]
        private GameObject backgroundObject;
        [SerializeField]
        private string allowLinksOnTag = "Link Avaliable";

        private List<WritingPanel> WritingPanels { get {
                return writingStoryCreator.WritingPanels;
            } }

        [SerializeField, PropertyTooltip("When mouse is over linking head or link button")]
        private Color linkingColor;
        public Color LinkingColor { get { return linkingColor; } }

        public IWritingLinker StartedLinkingLinker { get; private set; }
        /// <summary>
        /// Is linking in process right now?
        /// </summary>
        public bool InLinkingProcess { get; private set; }

        private void Update() {
            DisableLinkingIfClickAnywhere();
        }

        /// <summary>
        /// Clears previous start node. Sets the previous start node to default state.
        /// </summary>
        /// <param name="newStartNode">The new start node</param>
        public void ClearsPreviousStartNode(WritingPanel newStartNode) {
            foreach (WritingPanel panel in WritingPanels) {
                if (panel == newStartNode)
                    continue;
                if (panel.NodeType == WritingPanel.Type.StartNode) {
                    panel.SetNodeType(WritingPanel.Type.Default);
                }
            }
        }
        
        public void BeginLinking(IWritingLinker startingLinker) {
            StartedLinkingLinker = startingLinker;
            InLinkingProcess = true;

            foreach (WritingPanel panel in WritingPanels) {
                panel.ActivateLinkingPreview(startingLinker);
            }
            
        }

        /// <summary>
        /// When the linking has been pressed correctly. On another panel.
        /// </summary>
        public void SelectedLinkEnd(WritingPanel selectedPanel) {
            bool isDifferentLink = StartedLinkingLinker.IsDifferentLink(selectedPanel);

            LinkingProcessEnded();
            selectedPanel.LinkPanelToLinker(StartedLinkingLinker);

            // if this is a different link than the linker previously had, then we
            // should register as an undo.
            if (isDifferentLink) {
                RegisterActions.Instance.RegisterNewState();
            }
        }

        /// <summary>
        /// When linking process has failed, or it just ended.
        /// </summary>
        private void LinkingProcessEnded() {
            if (!InLinkingProcess)
                return;


            InLinkingProcess = false;
            foreach (WritingPanel panel in WritingPanels) {
                panel.HideLinkingPreview();
            }
        }

        /// <summary>
        /// Disable linking process and register action if it's the case.
        /// Is called via event (on right quick click)
        /// </summary>
        public void DisableLinkingViaQuickRightClick() {
            if (!InLinkingProcess)
                return;

            bool isDifferentLink = StartedLinkingLinker.IsDifferentLink(null);

            LinkingProcessEnded();

            if (isDifferentLink) {
                RegisterActions.Instance.RegisterNewState();
            }
        }

        /// <summary>
        /// When linking, this method spawns a new panel and links the current link with it.
        /// </summary>
        public void CreatePanelAndLink() {
            if (!InLinkingProcess)
                return;

            WritingPanel spawnedPanel = 
                writingStoryCreator.SpawnWritingPanel(true);
            spawnedPanel.transform.position = CameraMovement.MouseToWorldConversion(
                Input.mousePosition);

            SelectedLinkEnd(spawnedPanel);
        }

        private void DisableLinkingIfClickAnywhere() {
            if (InLinkingProcess && (Input.GetMouseButtonDown(0))) {
                GameObject pressedGameobject = sceneInputModule.GetPointerData().pointerPress;
                if (pressedGameobject == null || !pressedGameobject.CompareTag(allowLinksOnTag)) {

                    bool isDifferentLink = StartedLinkingLinker.IsDifferentLink(null);
                    LinkingProcessEnded();

                    if (isDifferentLink) {
                        RegisterActions.Instance.RegisterNewState();
                    }
                }
            }
        }
    }
}