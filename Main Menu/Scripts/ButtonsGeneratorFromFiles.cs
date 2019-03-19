namespace Game.MainMenu {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using Saving;
    using System.IO;
    using UnityEngine.UI;
#pragma warning disable 0649 // disable default value warning
    public enum LoadingType { WriteStory, ReadStory }

    public class ButtonsGeneratorFromFiles : MonoBehaviour {
        [SerializeField, Required]
        private ButtonToLoadFile presetButton;
        [SerializeField, Required]
        private StoryFilesEngine filesEngine;
        [SerializeField, Required]
        private SceneController sceneController;
        [SerializeField]
        private LoadingType loadingType;

        private List<ButtonToLoadFile> spawns = new List<ButtonToLoadFile>();

        public void SpawnButtons() {
            string[] allPaths;
            if (loadingType == LoadingType.ReadStory) {
                allPaths = filesEngine.GetAllPublishedFilePaths();
            } else {
                allPaths = filesEngine.GetAllSavedFilePaths();
            }

            ButtonToLoadFile spawn;

            DeletePreviousSpawns();

            foreach (string path in allPaths) {
                spawn = presetButton.gameObject.CreateObjectFromPreset()
                    .GetComponent<ButtonToLoadFile>();
                spawns.Add(spawn);

                try {
                    spawn.Init(path, filesEngine, this, sceneController, loadingType);
                } catch (InvalidStoryFilenameException) {
                    filesEngine.TryToGiveCorrectFilenameToInvalidFilename(path);
                    SpawnButtons();
                    return;
                }
            }
        }

        private void DeletePreviousSpawns() {
            for (int i = spawns.Count - 1; i >= 0; i--) {
                Destroy(spawns[i].gameObject);
            }
            spawns = new List<ButtonToLoadFile>();
        }

        public void DeleteFile(string atPath) {
            filesEngine.DeleteFile(atPath);
            SpawnButtons();
        }
    }
}
