namespace Game.Writing {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using Saving;

    public class LoadingSavedStory : MonoBehaviour {
        [SerializeField, Required]
        private WritingStoryCreator storyCreator;
        [SerializeField, Required]
        private StoryFilesEngine storyFilesEngine;
        [SerializeField, Required]
        private RegisterActions registerActions;

        /// <summary>
        /// Should be called on awake
        /// </summary>
        public void ReadCurrentLoadedStory() {
            storyFilesEngine.LoadNeededStoryToScene(this);
        }

        public void CreateFreshNewStory() {
            storyCreator.AssignStoryToGame(null, false);
            storyCreator.ShowProperties(); // show the player the title and the author
        }

        public void CreatePanelsFromLoadedStory(Story story) {
            storyCreator.AssignStoryToGame(story, true);
            registerActions.SetCurrentStateSaved();
        }
    }
}