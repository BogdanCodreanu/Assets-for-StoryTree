namespace Game.Saving {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.IO;
    using Sirenix.OdinInspector;

    [CreateAssetMenu(menuName = "Game/Story Path Saver", fileName = "Story Path Saver")]
    /// <summary>
    /// Saves storyPaths to files.
    /// </summary>
    public class StoryPathToFilesEngine : ScriptableObject {
        public const string DIRECTORY_NAME = "Saved Progress Files";
        public const string FILENAME = "Save";
        

        private StoryPathContainer pathsContainer;

        private static string DirectoryPath {
            get { return Application.dataPath + "/" + DIRECTORY_NAME; }
        }

        private static string PathLocationToFile(string filename) {
            return DirectoryPath + "/" + filename;
        }

        private static void CreateDirectoryIfNeeded() {
            if (!Directory.Exists(DirectoryPath)) {
                Directory.CreateDirectory(DirectoryPath);
            }
        }

        /// <summary>
        /// Used when new game is selected. It will reset the path of the next loaded file path.
        /// </summary>
        private bool resetNextPath = false;

        private void ReadCurrentPathContainer(string pathContainerFileName) {
            CreateDirectoryIfNeeded();
            pathsContainer = new StoryPathContainer();

            if (!File.Exists(PathLocationToFile(pathContainerFileName))) {
                // if there is no existing file for the pathsContainer then only create new one.
                // for an empty paths container.
                WritePathsContainerToFile(pathContainerFileName);
                return;
            }

            // if there is, we read it.
            pathsContainer = StoryPathContainer.GetDeserialized(
                File.ReadAllText(PathLocationToFile(pathContainerFileName)));
        }

        /// <summary>
        /// Write the paths container to file.
        /// </summary>
        private void WritePathsContainerToFile(string filename) {
            if (File.Exists(PathLocationToFile(filename))) {
                File.Delete(PathLocationToFile(filename));
            }
            using (StreamWriter writer = new StreamWriter(PathLocationToFile(filename), false)) {
                writer.Write(pathsContainer.GetSerialized());
            }
        }

        public void SaveStoryProgress(Story story, StoryPath currentPath) {
            string filenameToWriteTo = FILENAME;

            // we add the story data to the path.
            currentPath.ReceiveDataFromStory(story);

            // we read the current container
            ReadCurrentPathContainer(filenameToWriteTo);

            // we add the path to the current container
            pathsContainer.SetPathForStory(story, currentPath);

            // we write the container to file
            WritePathsContainerToFile(filenameToWriteTo);
        }

        /// <summary>
        /// Get the path of a story from saved file. Can also return null if it's new story.
        /// </summary>
        public StoryPath LoadStoryPath(Story forStory) {
            string filenameToReadFrom = FILENAME;
            ReadCurrentPathContainer(filenameToReadFrom);

            StoryPath path = pathsContainer.FindExistingPath(forStory);

            if (resetNextPath ||
                (path != null && path.ShouldResetProgressDueNewPublish(forStory))) {

                resetNextPath = false;
                path.ResetPath();
                SaveStoryProgress(forStory, path);
            }

            return path;
        }

        /// <summary>
        /// Deletes saved reading progress for a story.
        /// </summary>
        public void ResetPathProgress(Story forStory) {
            string filenameToWriteTo = FILENAME;
            ReadCurrentPathContainer(filenameToWriteTo);
            pathsContainer.ResetPathForStory(forStory);
        }

        /// <summary>
        /// Resets next loaded saved file path. For a new game.
        /// </summary>
        public void ResetNextPath() {
            resetNextPath = true;
        }

        /// <summary>
        /// Whether or not there exists a save file for a story file.
        /// </summary>
        public bool ExistsSaveDataForStoryFile(string storyFilePath, bool isRecursiveCall = false) {
            string filenameToReadFrom = FILENAME;
            ReadCurrentPathContainer(filenameToReadFrom);

            if (pathsContainer.FindExistingPath(storyFilePath) == null) {
                // return false if we don't find anything.
                return false;
            }
            return true;
        }
    }
}