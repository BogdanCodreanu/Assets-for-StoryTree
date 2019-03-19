namespace Game {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Xml.Serialization;
    using System.Runtime.Serialization;
    using Writing;
    
    [System.Serializable]
    public class Story : IStoryLogicPart {
        /// <summary>
        /// The name of the story
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The name of the author of this story.
        /// </summary>
        public string AuthorName { get; set; }
        
        /// <summary>
        /// Sets data to the story. Story name / author name / etc. Not panel information
        /// </summary>
        public void CopyPrimitiveData(Story from) {
            Name = from.Name;
            AuthorName = from.AuthorName;
        }

        public static void GiveDefaultData(ref Story to) {
            to.Name = "Untitled Story";
            to.AuthorName = "Unkown";
        }

        /// <summary>
        /// Index of the starting node
        /// </summary>
        public int StartStoryLineIndex { get; set; }

        /// <summary>
        /// The starting node, calculated based on index.
        /// </summary>
        public StoryLine StartingStoryLine {
            get {
                return AllStoryLines[StartStoryLineIndex];
            }
        }
        
        [XmlArray("StoryLine"), XmlArrayItem(typeof(StoryLine))]
        public List<StoryLine> AllStoryLines { get; set; }

        /// <summary>
        /// This is the unique identifier of the creator that made this file
        /// </summary>
        public string UniqueIdentifier { get; private set; }

        /// <summary>
        /// Assigns the unique device identifier to the story.
        /// </summary>
        private void GenerateUniqueIdentifier() {
            UniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
        }

        /// <summary>
        /// The time that this story has been published.
        /// Used to compare if it's modified the current saved published path data is the same
        /// for the current story. (The published story can be republished so we need new 
        /// save path data)
        /// </summary>
        public string DateTimePublished { get; private set; }

        /// <summary>
        /// Generates the date time for this story
        /// </summary>
        /// <returns></returns>
        private string GenerateDateTime() {
            return System.DateTime.Now.ToString();
        }

        /// <summary>
        /// Gets the serialized story text.
        /// </summary>
        public string GetSerialized() {
            return ObjectSerializer.SerializeObject(this);
        }

        /// <summary>
        /// Gets a story from serialized data
        /// </summary>
        public static Story GetDeserialized(string serializedData) {
            return (Story)ObjectSerializer.XmlDeserializeFromString(serializedData, typeof(Story));
        }

        private const string FILENAME_SEPARATOR = "+";

        /// <summary>
        /// Generates a file name for this story.
        /// </summary>
        public string GenerateFilename { get 
                { return Name + FILENAME_SEPARATOR + AuthorName + FILENAME_SEPARATOR  + "0"; } }
        // we also add a simple 0 at the end to have the same format as a published filename

        /// <summary>
        /// Generates a filename for this story as a published file. Also generates a unique identifier
        /// </summary>
        public string GeneratePublishedFilename { get {
                GenerateUniqueIdentifier();
                DateTimePublished = GenerateDateTime();
                return Name + FILENAME_SEPARATOR + AuthorName + FILENAME_SEPARATOR +
                    UniqueIdentifier;
        } }

        /// <summary>
        /// A friendly name used to reffer to this story so the player understands.
        /// Do not use this in any naming. Only in displaying.
        /// </summary>
        public string FriendlyName { get { return Name + " by " + AuthorName; } }

        /// <summary>
        /// Gets a friendly name for a filename. WIHTOUT extension (works the same)
        /// </summary>
        public static string FriendlyNameFromFilename(string filenameWithoutExtension) {
            return ExtractStoryNameFromFilename(filenameWithoutExtension) + " by " +
                ExtractAuthorNameFromFilename(filenameWithoutExtension);
        }

        /// <summary>
        /// Gets the story name from the filename. WIHTOUT extension (works the same)
        /// </summary>
        public static string ExtractStoryNameFromFilename(string filenameWithoutExtension) {
            return filenameWithoutExtension.Split(FILENAME_SEPARATOR[0])[0];
        }
        /// <summary>
        /// Gets the author name from the filename. WIHTOUT extension (works the same)
        /// </summary>
        public static string ExtractAuthorNameFromFilename(string filenameWithoutExtension) {
            try {
                return filenameWithoutExtension.Split(FILENAME_SEPARATOR[0])[1];
            } catch (System.IndexOutOfRangeException) {
                throw new InvalidStoryFilenameException
                    (filenameWithoutExtension + " is not a valid filename for a story file");
            }
        }

        /// <summary>
        /// Gets the author name from the filename. With or withour extension (works the same)
        /// </summary>
        public static string ExtractUniqueIdentifierFromFilename(string filename) {
            try {
                return filename.Split(FILENAME_SEPARATOR[0])[2];
            } catch (System.IndexOutOfRangeException) {
                throw new InvalidStoryFilenameException
                    (filename + " is not a valid filename for a story file");
            }
        }

        /// <summary>
        /// Find the next story line.
        /// </summary>
        /// <param name="from">The current story line</param>
        /// <param name="selectedDecision">The decision that has been selected. Can also be null if
        /// the current story line does not use decisions</param>
        /// <returns>The next story line</returns>
        public StoryLine NextStoryLine(StoryLine from, Decision selectedDecision) {
            if (from.UsesDecisions) {
                return AllStoryLines[selectedDecision.NextStoryLineIndex];
            } else {
                return AllStoryLines[from.NextQuickStoryLineIndex];
            }
        }
        /// <summary>
        /// Find the next story line.
        /// </summary>
        /// <param name="from">The current story line</param>
        /// <param name="selectedDecisionIndex">The index of the decision that has been selected
        /// Can also be null if the current story line does not use decisions</param>
        /// <returns>The next story line</returns>
        public StoryLine NextStoryLine(StoryLine from, int selectedDecisionIndex) {
            if (from.UsesDecisions) {
                return AllStoryLines[from.Decisions[selectedDecisionIndex].NextStoryLineIndex];
            } else {
                return AllStoryLines[from.NextQuickStoryLineIndex];
            }
        }

        /// <summary>
        /// The camera position when saved.
        /// </summary>
        public Vector3 CameraPosition { get; set; }
    }


    [System.Serializable]
    public class InvalidStoryFilenameException : System.Exception {
        public InvalidStoryFilenameException() { }
        public InvalidStoryFilenameException(string message) : base(message) { }
    }
}
