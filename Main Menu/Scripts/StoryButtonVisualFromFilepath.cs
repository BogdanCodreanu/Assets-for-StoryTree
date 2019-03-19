namespace Game.MainMenu {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using TMPro;
#pragma warning disable 0649 // disable default value warning

    public class StoryButtonVisualFromFilepath : MonoBehaviour {
        [SerializeField, Required]
        private TMP_Text storyTitle, authorName;


        /// <summary>
        /// Extracts data from the filename without any extension.
        /// </summary>
        /// <param name="filename">The filename without extension</param>
        public void ExtractDataFromFilename(string fileNameWithoutExtension) {
            storyTitle.text = Story.ExtractStoryNameFromFilename(fileNameWithoutExtension);
            authorName.text = Story.ExtractAuthorNameFromFilename(fileNameWithoutExtension);
        }
    }
}
