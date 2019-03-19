namespace Game.Reading {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using Saving;

    public class StoryPlayer : MonoBehaviour {
        private Story story;
        [SerializeField, Required]
        private StoryFilesEngine filesEngine;
        [SerializeField, Required]
        private StoryVisualReader storyVisualReader;

        private StoryLine currentStoryLine;
        private string selectedDecisionText;

        /// <summary>
        /// Current story path.
        /// </summary>
        public StoryPath StoryPath { get; private set; }

        [SerializeField, Required]
        private StoryPathToFilesEngine storyPathFilesEngine;

        private void AskForStory() {
            story = filesEngine.LoadPublishedStoryToScene();
        }

        private void Awake() {
            AskForStory();
            GetExistingStoryPath();
            ContinueReading();
        }

        private void ContinueReading() {
            currentStoryLine = StoryPath.GetSavedStoryLine(story);
            storyVisualReader.GiveStoryLine(currentStoryLine);
        }

        /// <summary>
        /// Plays the story, starting from the start node.
        /// Deleting any current reading progress
        /// </summary>
        //private void StartNewReading() {
        //    currentStoryLine = story.StartingStoryLine;
        //    storyVisualReader.GiveStoryLine(currentStoryLine);

        //    StoryPath = new StoryPath();
        //    SavePath();
        //}

        /// <summary>
        /// Should be called by the story visual reader itself.
        /// Makes story player to load the next story line and is ready to give a new line when asked.
        /// </summary>
        /// <param name="selectedDecision">Can be null if it's a quick next panel.</param>
        public void DecisionWasSelected(Decision selectedDecision) {
            StoryLine nextStoryLine = story.NextStoryLine(currentStoryLine, selectedDecision);
            StoryPath.SaveDecisionStep(currentStoryLine.IndexOfDecision(selectedDecision));
            currentStoryLine = nextStoryLine;

            SavePath();
        }

        /// <summary>
        /// Player has pressed last continue button. The story has ended
        /// </summary>
        public void PressedEndButton() {
            StoryPath.IsPathComplete = true;
            SavePath();
        }

        /// <summary>
        /// Called by the visual reader. Makes the story player give the new line to the visual reader.
        /// </summary>
        public void AskForNewLine() {
            storyVisualReader.GiveStoryLine(currentStoryLine);
        }

        /// <summary>
        /// Saves the reading progression of the current story to file.
        /// </summary>
        private void SavePath() {
            storyPathFilesEngine.SaveStoryProgress(story, StoryPath);
        }

        /// <summary>
        /// Gets from disk the existing story path and loads it in storyPath
        /// </summary>
        private void GetExistingStoryPath() {
            StoryPath loadedPath = storyPathFilesEngine.LoadStoryPath(story);

            if (loadedPath == null) {
                StoryPath = new StoryPath();
            } else {
                StoryPath = loadedPath;
            }

            storyVisualReader.RecieveStoryPath(StoryPath);
        }
    }
}
