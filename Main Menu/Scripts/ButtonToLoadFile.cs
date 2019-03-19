namespace Game.MainMenu {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using Saving;
    using System.IO;
    using UnityEngine.UI;
#pragma warning disable 0649 // disable default value warning

    public class ButtonToLoadFile : MonoBehaviour {

        [SerializeField, Required]
        private StoryButtonVisualFromFilepath storyVisualButton;
        [SerializeField, Required]
        private Button loadFileButton;

        [SerializeField, Required]
        private SelectStoryPanelController selectStoryController;
        
        public void Init(string pathToFile, StoryFilesEngine filesEngine, ButtonsGeneratorFromFiles
            creator, SceneController sceneController, LoadingType type) {

            string filenameWithoutExtension = Path.GetFileNameWithoutExtension(pathToFile);

            storyVisualButton.ExtractDataFromFilename(filenameWithoutExtension);

            loadFileButton.onClick.AddListener(delegate {
                selectStoryController.ShowWithFile(pathToFile, filesEngine, creator, type);
            });
        }


    }
}
