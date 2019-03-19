namespace Game {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Xml.Serialization;

    [System.Serializable]
    public class StoryPathContainer {
        /// <summary>
        /// Existing StoryPaths
        /// </summary>
        [XmlArray("StoryPath"), XmlArrayItem(typeof(StoryPath))]
        public List<StoryPath> StoryPaths { get; set; }

        public StoryPathContainer() {
            StoryPaths = new List<StoryPath>();
        }


        /// <summary>
        /// Get the serialized story path container text
        /// </summary>
        /// <returns></returns>
        public string GetSerialized() {
            return ObjectSerializer.SerializeObject(this);
        }

        /// <summary>
        /// Gets a story path container from serialized data
        /// </summary>
        public static StoryPathContainer GetDeserialized(string serializedData) {
            return (StoryPathContainer)ObjectSerializer
                .XmlDeserializeFromString(serializedData, typeof(StoryPathContainer));
        }

        /// <summary>
        /// Finds and returns the path for a story.
        /// Returns null if no path exists.
        /// </summary>
        public StoryPath FindExistingPath(Story forStory) {
            foreach (StoryPath path in StoryPaths) {
                if (path.IsForStory(forStory)) {
                    return path;
                }
            }
            return null;
        }

        /// <summary>
        /// Finds and returns the path for a story.
        /// Returns null if no path exists.
        /// </summary>
        public StoryPath FindExistingPath(string storyFilePath) {
            foreach (StoryPath path in StoryPaths) {
                if (path.IsForStory(storyFilePath)) {
                    return path;
                }
            }
            return null;
        }

        /// <summary>
        /// Sets a path in this container.
        /// </summary>
        /// <param name="forStory">The story that represents the path</param>
        /// <param name="setPath">The path to be set.</param>
        public void SetPathForStory(Story forStory, StoryPath setPath) {
            StoryPath existingPath = FindExistingPath(forStory);
            // we find if there already exists a path

            if (existingPath != null) {
                // if there is, we replace it
                int indexOfExistingPath = StoryPaths.IndexOf(existingPath);
                StoryPaths[indexOfExistingPath] = setPath;
            } else {
                // if there is not, we just add it
                StoryPaths.Add(setPath);
            }
        }

        /// <summary>
        /// Resets reading path progress for a story
        /// </summary>
        public void ResetPathForStory(Story forStory) {
            StoryPath existingPath = FindExistingPath(forStory);

            if (existingPath != null) {
                existingPath.ResetPath();
            }
            // if there is no existing path then it doesn't do anything.
        }
    }

    /// <summary>
    /// This is a path taken while reading the story. Used for saving reading progress.
    /// Or generating a string of the text from story start node to the current node.
    /// </summary>
    [System.Serializable, XmlRoot("StoryPathContainer")]
    public class StoryPath {
        /// <summary>
        /// The serialized steps indexes.
        /// These are the indexes of the chosen decisions.
        /// </summary>
        public List<int> StepsTaken { get; private set; }

        /// <summary>
        /// Used for comparing if this path is for a story.
        /// </summary>
        public string StoryName { get; set; }
        /// <summary>
        /// Used for comparing if this path is for a story.
        /// </summary>
        public string StoryAuthor { get; set; }
        /// <summary>
        /// Used for comparing if this path is for a story.
        /// </summary>
        public string StoryUniqueIdentifier { get; set; }
        /// <summary>
        /// Used for comparing if this path is for a for a published story publicated at a specific time.
        /// </summary>
        public string StoryPublishedDate { get; set; }

        /// <summary>
        /// The node path with all the details for every story line that the player has read.
        /// </summary>
        private List<StoryPathNode> constructedNodePath = new List<StoryPathNode>();


        /// <summary>
        /// This means that the path is complete and the story has been fully read.
        /// </summary>
        public bool IsPathComplete { get; set; }

        /// <summary>
        /// Whether or not the node path is constructed.
        /// </summary>
        private bool isNodePathConstructed = false;

        public class StoryPathNode {
            /// <summary>
            /// Constructor of a node
            /// </summary>
            /// <param name="currentStoryLine">Current story line in this point.</param>
            /// <param name="previous">The story line before this point. Can be null if it's the starting point.</param>
            /// <param name="next">The next story line after this point. Can be null if it's the end panel or if progress
            /// stops here.</param>
            /// <param name="decisionThatCameHere">Decision that has been selected from the previous story line to reach this story line.
            /// Can be null if it was a quick next panel or if it was the starting point.</param>
            /// <param name="decisionSelectedFromHere">Decision that was selected from the current panel to reach the next panel.
            /// Can be null if the current panel is a quick next panel or no selection has been made or
            /// it's the end panel.</param>
            public StoryPathNode(StoryLine currentStoryLine, StoryLine previous, StoryLine next,
                Decision decisionThatCameHere, Decision decisionSelectedFromHere) {
                CurrentStoryLine = currentStoryLine;
                PreviousStoryLine = previous;
                NextStoryLine = next;

                DecisionTakenToThis = decisionThatCameHere;
                DecisionTakenFromThis = decisionSelectedFromHere;
            }

            /// <summary>
            /// Current story line in this point.
            /// </summary>
            public StoryLine CurrentStoryLine { get; private set; }
            /// <summary>
            /// The story line before this point. Can be null if it's the starting point.
            /// </summary>
            public StoryLine PreviousStoryLine { get; private set; }
            /// <summary>
            /// The next story line after this point. Can be null if it's the end panel or if progress
            /// stops here.
            /// </summary>
            public StoryLine NextStoryLine { get; private set; }
            /// <summary>
            /// Decision that has been selected from the previous story line to reach this story line.
            /// Can be null if it was a quick next panel or if it was the starting point.
            /// </summary>
            public Decision DecisionTakenToThis { get; private set; }
            /// <summary>
            /// Decision that was selected from the current panel to reach the next panel.
            /// Can be null if the current panel is a quick next panel or no selection has been made or
            /// it's the end panel.
            /// </summary>
            public Decision DecisionTakenFromThis { get; private set; }
            /// <summary>
            /// Is this point a quick next panel that goes to the next one?
            /// </summary>
            public bool IsQuickNext { get { return !CurrentStoryLine.UsesDecisions; } }
        }

        public StoryPath() {
            StepsTaken = new List<int>();
            constructedNodePath = new List<StoryPathNode>();
            IsPathComplete = false;
        }

        /// <summary>
        /// Restes current game progress
        /// </summary>
        public void ResetPath() {
            StepsTaken = new List<int>();
            IsPathComplete = false;
            isNodePathConstructed = false;
        }

        /// <summary>
        /// Function that passes variables from a story to a path, used to later compare if a path is
        /// assigned to a story.
        /// </summary>
        public void ReceiveDataFromStory(Story story) {
            StoryName = story.Name;
            StoryAuthor = story.AuthorName;
            StoryUniqueIdentifier = story.UniqueIdentifier;
            StoryPublishedDate = story.DateTimePublished;
        }

        /// <summary>
        /// Whether or not this path is used for a story
        /// </summary>
        public bool IsForStory(Story story) {
            return story.Name == StoryName && story.AuthorName == StoryAuthor &&
                story.UniqueIdentifier == StoryUniqueIdentifier;
        }

        /// <summary>
        /// Whether or not this path is used for a story
        /// </summary>
        /// <returns></returns>
        public bool IsForStory(string storyFilePath) {
            string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(storyFilePath);

            string extractedStoryName = Story.ExtractStoryNameFromFilename(fileNameWithoutExtension);
            string extractedAuthorName = Story.ExtractAuthorNameFromFilename(fileNameWithoutExtension);
            string extractedUniquieIdentifier = Story.ExtractUniqueIdentifierFromFilename(fileNameWithoutExtension);

            return extractedStoryName == this.StoryName && extractedAuthorName == this.StoryAuthor &&
                extractedUniquieIdentifier == this.StoryUniqueIdentifier;
        }

        /// <summary>
        /// Returns true if the current saved path publish date is different than the 
        /// story published date.
        /// </summary>
        public bool ShouldResetProgressDueNewPublish(Story story) {
            return story.DateTimePublished != StoryPublishedDate;
        }

        /// <summary>
        /// Get the serialized story path text
        /// </summary>
        /// <returns></returns>
        public string GetSerialized() {
            return ObjectSerializer.SerializeObject(this);
        }

        /// <summary>
        /// Gets a story path from serialized data
        /// </summary>
        public static StoryPath GetDeserialized(string serializedData) {
            return (StoryPath)ObjectSerializer.XmlDeserializeFromString(serializedData, typeof(StoryPath));
        }

        /// <summary>
        /// Saves a decision choice
        /// </summary>
        /// <param name="decisionIndex">The index of the chosen decision. Does not matter if 
        /// it's a quick next panel.</param>
        public void SaveDecisionStep(int decisionIndex) {
            StepsTaken.Add(decisionIndex);
            isNodePathConstructed = false;
            IsPathComplete = false;
        }
        

        /// <summary>
        /// This constructs the logic path of the story.
        /// </summary>
        /// <param name="forStory">Construct for this story</param>
        private void ConstructPath(Story forStory) {
            if (isNodePathConstructed)
                return;
            isNodePathConstructed = true;

            // initial values starting at the starting node.
            StoryLine previousStoryLine = null;
            StoryLine currentStoryLine = forStory.StartingStoryLine;
            StoryLine nextStoryLine;
            Decision previousSelectedDecision = null;
            Decision currentSelectedDecision;

            constructedNodePath = new List<StoryPathNode>();

            for (int i = 0; i < StepsTaken.Count; i++) {

                currentSelectedDecision = currentStoryLine.GetDecisionAtIndex(StepsTaken[i]);
                nextStoryLine = forStory.NextStoryLine(currentStoryLine, currentSelectedDecision);
                

                constructedNodePath.Add(
                    new StoryPathNode(
                        currentStoryLine,
                        previousStoryLine,
                        nextStoryLine,
                        previousSelectedDecision,
                        currentSelectedDecision
                    ));

                previousStoryLine = currentStoryLine;
                previousSelectedDecision = currentSelectedDecision;

                currentStoryLine = nextStoryLine;
            }
            constructedNodePath.Add(
                new StoryPathNode(
                    currentStoryLine,
                    previousStoryLine,
                    null,
                    previousSelectedDecision,
                    null
            ));
        }

        /// <summary>
        /// Gets the last node in the constructed path (where the player has reached reading)
        /// </summary>
        private StoryPathNode LastStoryNode(Story story) {
            ConstructPath(story);
            return constructedNodePath[constructedNodePath.Count - 1];
        }

        /// <summary>
        /// Gets where the player left off reading.
        /// </summary>
        /// <returns>The last saved node in the story path.</returns>
        public StoryLine GetSavedStoryLine(Story story) {
            return LastStoryNode(story).CurrentStoryLine;
        }

        /// <summary>
        /// Whether or not the story has been completed.
        /// </summary>
        public bool HasStoryBeedCompletelyRead(Story story) {
            return IsPathComplete;
        }
    }
}
