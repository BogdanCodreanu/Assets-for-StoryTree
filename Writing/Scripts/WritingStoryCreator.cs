namespace Game.Writing {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using Properties;

    public class WritingStoryCreator : MonoBehaviour, IPropertySpawner<Story>, IWritingConditionPart {
        [ShowInInspector, ReadOnly]
        private Story story = new Story();
        public Story StoryLogicPart { get { return story; } }

        [SerializeField, Required, AssetsOnly]
        private WritingCanvas writingCanvasPrefab;
        /// <summary>
        /// Panels controller and linker.
        /// </summary>
        public PanelsControllerAndLinker ControllerAndLinker { get; private set; }

        private List<WritingPanel> writingPanels = new List<WritingPanel>();
        /// <summary>
        /// All the current Writing panels.
        /// </summary>
        [ReadOnly, ShowInInspector]
        public List<WritingPanel> WritingPanels { get { return writingPanels; } }
        [ReadOnly, ShowInInspector]
        private WritingPanel startingPanel;
        /// <summary>
        /// The starting panel.
        /// </summary>
        public WritingPanel StartingPanel { get { return startingPanel; } }

        [SerializeField, Required, PropertyTooltip("Where to spawn panels")]
        private Transform panelsContainer;

        [SerializeField, Required]
        private Transform cameraTransform;

        [SerializeField]
        private WritingPanel initialStartNode;
        private bool skipInitialStartNodeSpawning;

        [SerializeField, Required, SceneObjectsOnly]
        private PropertiesPanel propertiesPanel;

        [SerializeField, Required, AssetsOnly]
        private StoryProperties storyPropertiesPrefab;

        private void Awake() {
            ControllerAndLinker = GetComponent<PanelsControllerAndLinker>();
        }

        private void Start() {
            if (initialStartNode && !skipInitialStartNodeSpawning) {
                AssignDataToNewPanel(initialStartNode, false);
            }
            if (skipInitialStartNodeSpawning) {
                Destroy(initialStartNode.gameObject);
            }
        }

        public void SpawnWritingPanelViaButton() {
            SpawnWritingPanel(true);
        }

        /// <summary>
        /// Spawns a writing panel
        /// </summary>
        /// <returns>Returns the spawn Panel.</returns>
        public WritingPanel SpawnWritingPanel(bool spawnWithAnimations) {
            //WritingPanel spawn =
            //    Instantiate(writingPanelPrefab.gameObject,
            //    new Vector3(cameraTransform.position.x, cameraTransform.position.y, 0),
            //    Quaternion.identity, panelsContainer)
            //    .GetComponent<WritingPanel>();

            Vector3 panelAtPos =
                new Vector3(cameraTransform.position.x, cameraTransform.position.y, 0);
            WritingCanvas spawnedCanvas = 
                Instantiate(writingCanvasPrefab.gameObject, Vector3.zero, Quaternion.identity,
                panelsContainer)
                .GetComponent<WritingCanvas>();

            spawnedCanvas.MovePanelAt(panelAtPos);

            //spawn.transform.SetSiblingIndex(transform.childCount - 2);
            AssignDataToNewPanel(spawnedCanvas.WritingPanel, spawnWithAnimations);
            return spawnedCanvas.WritingPanel;
        }

        /// <summary>
        /// Creates all data for this new panel
        /// </summary>
        private void AssignDataToNewPanel(WritingPanel panel, bool playSpawningAnimations) {
            writingPanels.Add(panel);
            panel.InitOnCreation(this, playSpawningAnimations);

            if (startingPanel == null) {
                panel.SetAsStartNode();
            } else {
                panel.SetNodeType(WritingPanel.Type.Default);
            }
        }

        /// <summary>
        /// Removes panel from panels list (should be called when a panel should be considered removed)
        /// </summary>
        public void RemoveWritingPanel(WritingPanel panel) {
            if (startingPanel == panel) {
                startingPanel = null;
            }
            writingPanels.Remove(panel);
        }

        /// <summary>
        /// Deletes a panel. (also takes care of list if it's necessary)
        /// </summary>
        public void DeleteWritingPanel(WritingPanel panel) {
            if (startingPanel == panel) {
                startingPanel = null;
            }
            if (writingPanels.Contains(panel)) {
                writingPanels.Remove(panel);
            }
            panel.ContainingCanvas.DestroySelfAndWritingPanel();
        }

        public void SetStartingPanelVariable(WritingPanel to) {
            startingPanel = to;
        }

        /// <summary>
        /// The index of the given panel in the list of all panels
        /// </summary>
        public int GetIndexOfPanel(WritingPanel panel) {
            return writingPanels.IndexOf(panel);
        }

        private List<StoryLine> GetAllPanelStories() {
            List<StoryLine> panelStories = new List<StoryLine>();
            foreach (WritingPanel panel in writingPanels) {
                panelStories.Add(panel.GetTheStoryLine());
            }
            return panelStories;
        }

        /// <summary>
        /// Deletes and removes all panels
        /// </summary>
        private void DeleteCurrentPanels() {
            for (int i = WritingPanels.Count - 1; i >= 0; i--) {
                WritingPanels[i].ContainingCanvas.DestroySelfAndWritingPanel();
            }
            writingPanels = new List<WritingPanel>();
        }

        /// <summary>
        /// Creates the story class and gets it
        /// </summary>
        public Story GetTheStory() {
            story.StartStoryLineIndex = startingPanel.StoryLineIndex;
            story.AllStoryLines = GetAllPanelStories();
            story.CameraPosition = cameraTransform.position;
            return story;
        }

        /// <summary>
        /// Initializes story panels, also loads all panels and creates them if a story is given.
        /// </summary>
        /// <param name="story">Loaded Story (can be null if the story is not loaded)</param>
        /// <param name="isLoadedStory">True if a story is loaded and given as a parameter,
        /// false if there should be a fresh new default story on screen.</param>
        public void AssignStoryToGame(Story story, bool isLoadedStory) {

            if (isLoadedStory) {
                skipInitialStartNodeSpawning = true;
                ApplyStoryToGame(story);
                cameraTransform.position = story.CameraPosition;

            } else {
                // if i don't load the story from a file, then it will have some default values.
                Story.GiveDefaultData(ref this.story);
            }
        }

        /// <summary>
        /// Removes any panels and data from scene and creates data from the given story.
        /// </summary>
        public void ApplyStoryToGame(Story story) {
            SpawnPanelsFromStoryData(story);
            this.story.CopyPrimitiveData(story);
        }

        private void SpawnPanelsFromStoryData(Story story) {
            DeleteCurrentPanels();

            Dictionary<WritingPanel, StoryLine> assignedStories = new Dictionary<WritingPanel, StoryLine>();

            foreach (StoryLine storyLine in story.AllStoryLines) {
                WritingPanel spawn = SpawnWritingPanel(false);
                assignedStories.Add(spawn, storyLine);

                spawn.AssignDataByStoryLine(storyLine);
            }

            foreach (WritingPanel panel in WritingPanels) {
                panel.AssignDecisionsByStoryLineOrQuickNextPanel(assignedStories[panel], WritingPanels);
            }

            WritingPanels[story.StartStoryLineIndex].SetAsStartNode();
        }


        public void ShowProperties() {
            GameObject spawn =
                propertiesPanel.SpawnAndShowProperties(storyPropertiesPrefab.gameObject, "Story");
            if (spawn == null) {
                return;
            }

            IPropertyContentForPanel<Story> storyProperties = 
                spawn.GetComponent<IPropertyContentForPanel<Story>>();

            storyProperties.Initialize(this);
            storyProperties.AssignInitialValues();
        }

        public void CheckIfPartIsValidForSaving() {
            if (startingPanel == null) {
                throw new InvalidPartForPlayingException("No existing starting panel", "Assign a starting panel");
            }

            if (string.IsNullOrEmpty(story.Name)) {
                throw new InvalidPartForPlayingException("No story title assigned",
                    "Write a title to the story",
                    ShowProperties);
            }
            if (string.IsNullOrEmpty(story.AuthorName)) {
                throw new InvalidPartForPlayingException("No author assigned",
                    "Write your author name",
                    ShowProperties);
            }

            foreach (WritingPanel panel in WritingPanels) {
                panel.CheckIfPartIsValidForSaving();
            }
        }
    }
}