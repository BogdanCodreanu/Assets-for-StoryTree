namespace Game.MainMenu {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using TMPro;
    using UnityEngine.UI;
    using Saving;
    using System.IO;
    using UIAddition;
    using RazzielModules.UIHelper;
#pragma warning disable 0649 // disable default value warning

    public class SelectStoryPanelController : MonoBehaviour {
        private const string DeleteFileMsgConfirm = "Are you sure you want to delete this file?";

        [SerializeField, Required]
        private TMP_Text storyTitle, authorName, sizeInBytes;

        [SerializeField, Required, BoxGroup("Reading Part")]
        private Button newGameButton, resumeGameButton;
        [SerializeField, Required]
        private Button deleteButton, cancelButton;

        [SerializeField, Required, BoxGroup("Editing Part")]
        private Button continueEditingButton;

        [SerializeField, Required]
        private SceneController sceneController;
        [SerializeField, Required]
        private StoryPathToFilesEngine storyPathToFilesEngine;

        [SerializeField, Required]
        private PanelsQuickAnimations showAnimation;

        [SerializeField, Required]
        private GameObject backgroundObject, panelObject;

        [SerializeField, Required]
        private UISubtleClick backgroundClick;

        [SerializeField, Required]
        private ButtonsQuickInteractible quickInteractibles;

        /// <summary>
        /// Whether or not resume game button should be avaliable to press when
        /// the fade in animation has finished
        /// </summary>
        private bool willAllowResumeGameButton;

        private void Awake() {
            backgroundClick.ActionOnClick.AddListener(ClosePanel);
            cancelButton.onClick.AddListener(ClosePanel);
        }

        /// <summary>
        /// Sets active the buttons gameobjects for a specified type
        /// </summary>
        private void SetActive(LoadingType type) {
            if (type == LoadingType.ReadStory) {
                continueEditingButton.gameObject.SetActive(false);
                newGameButton.gameObject.SetActive(true);
                resumeGameButton.gameObject.SetActive(true);
            } else {
                continueEditingButton.gameObject.SetActive(true);
                newGameButton.gameObject.SetActive(false);
                resumeGameButton.gameObject.SetActive(false);
            }
        }

        private static string BytesToString(long byteCount) {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = (Mathf.Abs((int)byteCount));
            int place = (Mathf.FloorToInt(Mathf.Log(bytes, 1024)));
            double num = Mathf.Round(bytes / Mathf.Pow(1024, place));
            return (Mathf.Sign(byteCount) * num).ToString() + suf[place];
        }

        private void ShowPanel() {
            // dont allow interactibility while animating
            quickInteractibles.SetButtonsInteractible(false);

            backgroundObject.SetActive(true);
            panelObject.SetActive(true);
            showAnimation.PlayAllAnimations();

            this.ExecuteWithDelay(showAnimation.TotalAnimatingTime,
                delegate {
                    quickInteractibles.SetButtonsInteractible(true);
                    resumeGameButton.interactable = willAllowResumeGameButton;
                });
        }

        private void ClosePanel() {
            quickInteractibles.SetButtonsInteractible(false);

            showAnimation.PlayAllAnimationsReversed();
            this.ExecuteWithDelay(showAnimation.TotalAnimatingTime,
                delegate {
                    backgroundObject.SetActive(false);
                    panelObject.SetActive(false);
                });
        }

        /// <summary>
        /// Shows panel and fills data from file
        /// </summary>
        public void ShowWithFile(string pathToFile, StoryFilesEngine filesEngine, ButtonsGeneratorFromFiles
            creator, LoadingType type) {

            string filenameWithoutExtension = Path.GetFileNameWithoutExtension(pathToFile);
            string filenameWithExtension = Path.GetFileName(pathToFile);

            ShowPanel();
            SetActive(type);

            filesEngine.SetLoadedFile(filenameWithExtension);

            storyTitle.text = Story.ExtractStoryNameFromFilename(filenameWithoutExtension);
            authorName.text = Story.ExtractAuthorNameFromFilename(filenameWithoutExtension);

            sizeInBytes.text = "(" + BytesToString(new FileInfo(pathToFile).Length) + ")";

            deleteButton.onClick.RemoveAllListeners();
            deleteButton.onClick.AddListener(delegate {
                UIHelper.Instance.DisplayConfirmActionPanel(DeleteFileMsgConfirm +
                    " (" + Story.FriendlyNameFromFilename(filenameWithoutExtension) + ")",
                    delegate { ClosePanel(); creator.DeleteFile(pathToFile); });
            });

            if (type == LoadingType.ReadStory) {
                ShowReadingButtons(pathToFile, filesEngine, creator);
            } else {
                ShowWritingButtons(pathToFile, filesEngine, creator);
            }
        }

        private void ShowReadingButtons(string pathToFile, StoryFilesEngine filesEngine,
            ButtonsGeneratorFromFiles creator) {
            bool pathExistsForSaveFile = false;

            newGameButton.onClick.RemoveAllListeners();
            resumeGameButton.onClick.RemoveAllListeners();

            try {
                pathExistsForSaveFile = storyPathToFilesEngine.ExistsSaveDataForStoryFile(pathToFile);
            } catch (InvalidStoryFilenameException) {
                filesEngine.TryToGiveCorrectFilenameToInvalidFilename(pathToFile);
                ClosePanel();
                creator.SpawnButtons();
            }

            if (pathExistsForSaveFile) {
                willAllowResumeGameButton = true;

                newGameButton.onClick.AddListener(delegate {
                    UIHelper.Instance.DisplayConfirmActionPanel(
                        "Are you sure you want to create a new game?" +
                        " This action will overwrite existing save progress.",
                        CreateNewSaveForSelectedStory);
                });
            } else {
                willAllowResumeGameButton = false;
                // if there does not exist a save, then just change to play story.
                newGameButton.onClick.AddListener(delegate {
                    sceneController.ChangeToPlayStory();
                });
            }

            // resume game changes to play story without any additional markings.
            resumeGameButton.onClick.AddListener(delegate {
                sceneController.ChangeToPlayStory();
            });
        }

        private void ShowWritingButtons(string pathToFile, StoryFilesEngine filesEngine,
            ButtonsGeneratorFromFiles creator) {

            continueEditingButton.onClick.RemoveAllListeners();
            continueEditingButton.onClick.AddListener(sceneController.ChangeToEditTree);
        }

        private void CreateNewSaveForSelectedStory() {
            storyPathToFilesEngine.ResetNextPath();
            sceneController.ChangeToPlayStory();
        }
    }
}
