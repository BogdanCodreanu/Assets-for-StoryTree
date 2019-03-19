﻿namespace Game.Saving {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using System.IO;
    using UnityEngine.SceneManagement;
    using RazzielModules.UIHelper;

    [CreateAssetMenu(menuName = "Game/Story Files Engine", fileName = "Story Files Engine")]
    /// <summary>
    /// Finds files, writes files, changes names of saved files.
    /// </summary>
    public class StoryFilesEngine : ScriptableObject {

        #region Published stories variables

        public const string STORY_PUBLISHED_FILE_EXTENSION = "StoryTree";
        public const string STORY_PUBLISHED_DIRECTORY_NAME = "Published Stories";
        private string PublishedDirectoryPath { get { return Application.dataPath + "/"
                    + STORY_PUBLISHED_DIRECTORY_NAME; }
        }
        /// <summary>
        /// Gets the path to the published file given as the parameter.
        /// </summary>
        private string PathLocationToPublishedFile(string publishedFileName) {
            return PublishedDirectoryPath + "/" + publishedFileName;
        }

        /// <summary>
        /// Generates a file name with extension for a published story.
        /// </summary>
        public string GeneratePublishedFileName(Story forStory) {
            return forStory.GeneratePublishedFilename + "." + STORY_PUBLISHED_FILE_EXTENSION;
        }

        #endregion
        #region Saved story tree variables

        public const string STORY_SAVED_FILE_EXTENSION = "SavedStoryTree";
        public const string STORY_OVERWRITTEN_MSG = "(Overwritten)";
        public const string SAVED_DIRECTORY_NAME = "Saved Stories";
        private string SaveDirectoryPath { get { return Application.dataPath + "/"
                    + SAVED_DIRECTORY_NAME; } }
        /// <summary>
        /// Gets the path to the save file given as the parameter.
        /// </summary>
        private string PathLocationToSaveFile(string savedFileName) {
            return SaveDirectoryPath + "/" + savedFileName;
        }
        /// <summary>
        /// Generates a file name for a saved story.
        /// </summary>
        /// <param name="storyFilename">The current name generated by the story</param>
        private string AddSavedExtensionToStoryTreeFilename(string storyFilename) {
            return storyFilename + "." + STORY_SAVED_FILE_EXTENSION;
        }

        /// <summary>
        /// Adds the overwritten tag to the string representing the new filename
        /// </summary>
        /// <param name="filenameWithoutExtension">The filename without extension</param>
        /// <returns>The filename + overwritten tag without extension</returns>
        private string AddOverwrittenTag(string filenameWithoutExtension) {
            return filenameWithoutExtension += STORY_OVERWRITTEN_MSG;
        }

        #endregion

        /// <summary>
        /// Previous file name where it saved, so it will always remove this file while saving another.
        /// The current file name that was last saved.
        /// This can also be used to load a published story file.
        /// </summary>
        [ReadOnly, ShowInInspector]
        private string loadedFilenameWithExtension;


        /// <summary>
        /// No file will be overwritten on next saving.
        /// </summary>
        public void CreateNewSaveFile() {
            loadedFilenameWithExtension = "";
        }

        /// <summary>
        /// Next save will overwrite this file. Or next played story will be this one.
        /// </summary>
        public void SetLoadedFile(string filenameWithExtension) {
            loadedFilenameWithExtension = filenameWithExtension;
        }
        
        private void CreateDirectoryIfNeeded(string directoryPath) {
            if (!Directory.Exists(directoryPath)) {
                Directory.CreateDirectory(directoryPath);
            }
        }

        /// <summary>
        /// Saves current story tree to a saved story tree file
        /// </summary>
        public void SaveToFile(Story story) {
            string newFileName = AddSavedExtensionToStoryTreeFilename(story.GenerateFilename);

            CreateDirectoryIfNeeded(SaveDirectoryPath);

            // if previous saved file name is considered existing
            if (!string.IsNullOrEmpty(loadedFilenameWithExtension)) {
                // but no file with that name exists
                if (!File.Exists(PathLocationToSaveFile(loadedFilenameWithExtension))) {
                    // it's a bug.
                    Debug.LogError("Should never have the variable to a previous file name that " +
                        "does not exist");

                    UIHelper.Instance.DisplayInformationMessage("Unable to save. A ghost loaded file exists," +
                        " named " + loadedFilenameWithExtension);
                    loadedFilenameWithExtension = "";
                    return;
                }

                File.Delete(PathLocationToSaveFile(loadedFilenameWithExtension));
                Debug.Log(loadedFilenameWithExtension + " was deleted");
            }

            // if another file exists with the same name
            AddOverwrittenTagToPossibleExistingFilename(story.GenerateFilename);


            using (StreamWriter writer = new StreamWriter(PathLocationToSaveFile(newFileName), false)) {
                writer.Write(story.GetSerialized());
            }
            

            UIHelper.Instance.DisplayInformationMessage("Story \"" + 
                story.FriendlyName + "\" saved.");

            loadedFilenameWithExtension = newFileName;
        }

        /// <summary>
        /// Adds the overwritten tag to any filename that exists with the given name.
        /// Can be called even if we're not sure that a file with that name exists.
        /// It will also recursivly rename files with multiple overwritten tags until we find a place for
        /// everyone.
        /// </summary>
        private void AddOverwrittenTagToPossibleExistingFilename(string occupiedFilenameWithoutExtension) {
            if (File.Exists(PathLocationToSaveFile
                (AddSavedExtensionToStoryTreeFilename(occupiedFilenameWithoutExtension)))) {

                AddOverwrittenTagToPossibleExistingFilename(
                    AddOverwrittenTag(occupiedFilenameWithoutExtension));

                File.Move(
                    PathLocationToSaveFile(AddSavedExtensionToStoryTreeFilename(occupiedFilenameWithoutExtension)),
                    PathLocationToSaveFile(AddSavedExtensionToStoryTreeFilename(
                        AddOverwrittenTag(occupiedFilenameWithoutExtension))));
            }
        }

        /// <summary>
        /// Paths to all saved story trees
        /// </summary>
        public string[] GetAllSavedFilePaths() {
            CreateDirectoryIfNeeded(SaveDirectoryPath);
            return Directory.GetFiles(SaveDirectoryPath, "*." + STORY_SAVED_FILE_EXTENSION);
        }

        /// <summary>
        /// Loads the set saved file to the game. Or sets up the scene for a fresh new story.
        /// </summary>
        public void LoadNeededStoryToScene(Writing.LoadingSavedStory loadingSavedStory) {
            if (string.IsNullOrEmpty(loadedFilenameWithExtension)) {
                loadingSavedStory.CreateFreshNewStory();
            } else {
                loadingSavedStory.CreatePanelsFromLoadedStory(
                    Story.GetDeserialized(
                        File.ReadAllText(PathLocationToSaveFile(loadedFilenameWithExtension))
                    )
                );
            }
        }


        /// <summary>
        /// Creates file of a playing story. Should be called after the condition for publishing check!
        /// </summary>
        public void PublishStoryToFile(Story story) {
            string fileName = GeneratePublishedFileName(story);
            CreateDirectoryIfNeeded(PublishedDirectoryPath);

            // if file with same name exists, a popup asking to overwrite will appear.
            if (File.Exists(PathLocationToPublishedFile(fileName))) {
                UIHelper.Instance.DisplayConfirmActionPanel("A published story named \"" +
                    story.FriendlyName + "\" already exists. Do you wish to overwrite it?",
                    delegate {
                        File.Delete(PathLocationToPublishedFile(fileName));
                        WritePublishedStoryToFile(story, PathLocationToPublishedFile(fileName));
                    },
                    delegate {
                        UIHelper.Instance.DisplayInformationMessage("Publishing canceled.");
                    });

                return;
            }


            // otherwise it will just save it
            WritePublishedStoryToFile(story, PathLocationToPublishedFile(fileName));
        }

        /// <summary>
        /// Method used to write the published story (also encryps it, in the future)
        /// </summary>
        private void WritePublishedStoryToFile(Story story, string atPath) {
            using (StreamWriter writer = new StreamWriter(atPath, false)) {
                writer.Write(story.GetSerialized());
            }
            UIHelper.Instance.DisplayInformationMessage("Story \"" + story.FriendlyName + "\" published.");
        }

        /// <summary>
        /// Paths to all published stories
        /// </summary>
        public string[] GetAllPublishedFilePaths() {
            CreateDirectoryIfNeeded(PublishedDirectoryPath);
            return Directory.GetFiles(PublishedDirectoryPath, "*." + STORY_PUBLISHED_FILE_EXTENSION);
        }


        /// <summary>
        /// Loads the set file to the game.
        /// </summary>
        public Story LoadPublishedStoryToScene() {
            return Story.GetDeserialized(
                File.ReadAllText(PathLocationToPublishedFile(loadedFilenameWithExtension)));
        }

        /// <summary>
        /// Deletes a file at a given path
        /// </summary>
        public void DeleteFile(string path) {
            if (File.Exists(path)) {
                File.Delete(path);
            }
        }

        /// <summary>
        /// Use when an invalid filename has been found. This will try to rename the file to the current
        /// supported filenaming
        /// </summary>
        public void TryToGiveCorrectFilenameToInvalidFilename(string atPath) {
            string extension = Path.GetExtension(atPath).Substring(1);
            Story readStory = Story.GetDeserialized(File.ReadAllText(atPath));
            string goodName;

            if (extension == STORY_PUBLISHED_FILE_EXTENSION) {
                // if it's published file
                goodName = GeneratePublishedFileName(readStory);
            } else {
                // it's saved story tree file
                goodName = AddSavedExtensionToStoryTreeFilename(readStory.GenerateFilename);
                // if another file exists with the same name
                AddOverwrittenTagToPossibleExistingFilename(readStory.GenerateFilename);
            }

            File.Move(atPath, Path.GetDirectoryName(atPath) + "/" + goodName);
            Debug.Log("Renamed \n" + atPath + "\n to \n" + Path.GetDirectoryName(atPath) + "/" + goodName);
        }
    }
}