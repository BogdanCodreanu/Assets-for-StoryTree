namespace Game.Writing {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// A part that checks if it's valid for saving and playing the story
    /// </summary>
    public interface IWritingConditionPart {
        /// <summary>
        ///  Checks if it's valid for saving. Can throw InvalidPartForPlayingException.
        /// </summary>
        void CheckIfPartIsValidForSaving();
    }

    public class InvalidPartForPlayingException : System.Exception {

        /// <summary>
        /// What message should be displayied to explain the error?
        /// </summary>
        public string ErrorShownMessage { get; private set; }

        /// <summary>
        /// What message should be displayied to explain how to fix the error?
        /// </summary>
        public string HowToFixErrorMessage { get; private set; }

        /// <summary>
        /// Should the camera jump to a location when showing error?
        /// </summary>
        public bool JumpToLocation { get; private set; }

        /// <summary>
        /// The location where the error is present. The camera should jump here.
        /// </summary>
        public Vector3 LocationToJump { get; private set; }

        /// <summary>
        /// The location where the error is present. The camera should jump here.
        /// </summary>
        public System.Action SpawnPropertiesAction { get; private set; }


        /// <summary>
        /// Creates a return code.
        /// </summary>
        /// <param name="isError">If the part is considered to have an error</param>
        /// <param name="errorMessage">What message should be displayied to explain the error?</param>
        /// <param name="shouldCameraJump">Should the camera jump to a location when showing error?</param>
        /// <param name="jumpLocation">The location where the error is present. The camera should jump here.</param>
        /// <param name="actionToSpawnProperties">Showing the properties if needed. To point out
        /// that some info is missing</param>
        public InvalidPartForPlayingException(string errorMessage, string howToFixMessage,
            bool shouldCameraJump, Vector3 jumpLocation, System.Action actionToSpawnProperties = null) {
            
            ErrorShownMessage = errorMessage;
            JumpToLocation = shouldCameraJump;
            LocationToJump = jumpLocation;
            HowToFixErrorMessage = howToFixMessage;
        }
        /// <summary>
        /// Creates a return code. The camera will not jump anywhere.
        /// </summary>
        /// <param name="isError">If the part is considered to have an error</param>
        /// <param name="errorMessage">What message should be displayied to explain the error?</param>
        /// <param name="actionToSpawnProperties">Showing the properties if needed. To point out
        /// that some info is missing</param>
        public InvalidPartForPlayingException(string errorMessage, string howToFixMessage, 
            System.Action actionToSpawnProperties = null) {

            ErrorShownMessage = errorMessage;
            JumpToLocation = false;
            LocationToJump = Vector3.zero;
            HowToFixErrorMessage = howToFixMessage;
            SpawnPropertiesAction = actionToSpawnProperties;
        }
    }
}