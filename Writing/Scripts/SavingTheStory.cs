namespace Game.Saving {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using System.Xml.Serialization;
    using System.IO;
    using Writing;
    using RazzielModules.UIHelper;

    public class SavingTheStory : MonoBehaviour {
        [SerializeField, Required]
        private WritingStoryCreator storyCreator;
        [SerializeField, Required]
        private GameEvent OnSaveStory;

        [SerializeField, Required]
        private new CameraMovement camera;

        [SerializeField, Required]
        private StoryFilesEngine filesEngine;

        [SerializeField, Required]
        private RegisterActions registerActions;

        private bool InputToSave {
            get { return Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftControl); }
        }

        /// <summary>
        /// Checks if the Story is ready to be published
        /// </summary>
        /// <returns>True if it's ready. False if not, and the error will also be highlighted</returns>
        private bool CheckStoryReadyToPlay() {
            try {
                storyCreator.CheckIfPartIsValidForSaving();
            } catch (InvalidPartForPlayingException exception) {
                DisplayStorySavingError(exception);
                return false;
            }
            return true;
        }

        private void DisplayStorySavingError(InvalidPartForPlayingException error) {
            if (error.SpawnPropertiesAction != null) {
                error.SpawnPropertiesAction();
            }
            if (error.JumpToLocation) {
                camera.JumpToPosition(error.LocationToJump);
            }
            UIHelper.Instance.DisplayInformationMessage
                (error.ErrorShownMessage + ". " + error.HowToFixErrorMessage,
                UIHelper.InfoMessageType.Error);
        }

        /// <summary>
        /// This saves the entire story to file.
        /// </summary>
        public void SaveStoryToFile() {
            OnSaveStory.Raise();
            Story savedStory = storyCreator.GetTheStory();

            filesEngine.SaveToFile(savedStory);

            registerActions.SetCurrentStateSaved();
        }

        public void PublishStory() {
            if (CheckStoryReadyToPlay()) {
                filesEngine.PublishStoryToFile(storyCreator.GetTheStory());
            }
        }

        private void Update() {
            if (InputToSave) {
                SaveStoryToFile();
            }
        }
    }
}