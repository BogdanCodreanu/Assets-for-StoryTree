namespace Game.Writing {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using UnityEngine.UI;
#pragma warning disable 0649 // disable default value warning

    public class RegisterActions : MonoBehaviour {
        [SerializeField, Required]
        private WritingStoryCreator storyCreator;
        [SerializeField]
        private int undoLimitRegister = 10;

        [ShowInInspector, ReadOnly]
        private List<RegisteredState> registeredStates = new List<RegisteredState>();

        [ShowInInspector, ReadOnly]
        private int currentStateIndex = -1;

        public RegisteredState CurrentState { get { return registeredStates[currentStateIndex]; } }

        [SerializeField, Required]
        private UISelectionHelper uiSelectionHelper;
        
        public static RegisterActions Instance { get; private set; }

        private bool recording = false;
        [SerializeField]
        private float disabledRecordingTime = .3f;

        private Coroutine currentEnablingCoroutine = null;

        [SerializeField, Required]
        private DisplayUnsavedProgress displayUnsaved;

        private bool InputToUndo {
            get { return Input.GetKeyDown(KeyCode.Z) && Input.GetKey(KeyCode.LeftControl) && 
                    !Input.GetKey(KeyCode.LeftShift); }
        }
        private bool InputToRedo {
            get { return (Input.GetKeyDown(KeyCode.Z) && Input.GetKey(KeyCode.LeftControl)
                    && Input.GetKey(KeyCode.LeftShift)) ||
                    (Input.GetKeyDown(KeyCode.Y) && (Input.GetKey(KeyCode.LeftControl))); }
        }

        /// <summary>
        /// Marks true when no saved state exists and this will mark the next registered state as already saved.
        /// It's used when the story is loaded from file on scene loading.
        /// </summary>
        private bool nextStateIsSaved = false;

        private void Awake() {
            Instance = this;

            // initial recording
            DisableRecordingForSmallTime(true);
        }

        /// <summary>
        /// Will not record anything for a small amount of frames.
        /// </summary>
        /// <param name="registerStateAfter">If true, then it will also register a new state
        /// after a pause from recording has been made</param>
        public void DisableRecordingForSmallTime(bool registerStateAfter) {
            if (currentEnablingCoroutine != null) {
                StopCoroutine(currentEnablingCoroutine);
            }
            recording = false;
            currentEnablingCoroutine =
                StartCoroutine(EnableRecordingAfterFrames(registerStateAfter));
        }

        private IEnumerator EnableRecordingAfterFrames(bool alsoRegisterStateAfter) {
            yield return new WaitForSecondsRealtime(disabledRecordingTime);
            recording = true;
            currentEnablingCoroutine = null;
            if (alsoRegisterStateAfter)
                RegisterNewState();
        }

        [Button]
        public void RegisterNewState() {
            if (!recording) {
                return;
            }

            if (currentStateIndex != -1) {
                registeredStates.RemoveRange(currentStateIndex + 1, registeredStates.Count - currentStateIndex - 1);
            }

            registeredStates.Add(new RegisteredState(storyCreator.GetTheStory()));
            currentStateIndex++;

            if (currentStateIndex >= undoLimitRegister) {
                registeredStates.RemoveAt(0);
                currentStateIndex--;
            }

            if (nextStateIsSaved) {
                nextStateIsSaved = false;
                SetCurrentStateSaved();
            }
            displayUnsaved.CheckIfSaved();
        }

        [Button]
        public void Undo() {
            if (currentStateIndex <= 0) {
                return;
            }
            currentStateIndex--;

            ApplyState(registeredStates[currentStateIndex]);

            DisableRecordingForSmallTime(false);
            displayUnsaved.CheckIfSaved();
        }

        [Button]
        public void Redo() {
            if (currentStateIndex >= registeredStates.Count - 1) {
                return;
            }
            currentStateIndex++;

            ApplyState(registeredStates[currentStateIndex]);

            DisableRecordingForSmallTime(false);
            displayUnsaved.CheckIfSaved();
        }

        private void ApplyState(RegisteredState state) {
            storyCreator.ApplyStoryToGame(Story.GetDeserialized(state.SerializedData));
        }

        private void Update() {
            if (InputToUndo)
                Undo();
            if (InputToRedo)
                Redo();
        }

        /// <summary>
        /// Current state has been saved to file. This marks the current state 'IsThisStateSaved' variable
        /// to true and all other states are marked with this variable as false.
        /// </summary>
        public void SetCurrentStateSaved() {
            if (registeredStates.Count == 0) {
                nextStateIsSaved = true;
                return;
            }

            foreach (RegisteredState state in registeredStates) {
                state.IsThisStateSaved = false;
            }
            CurrentState.IsThisStateSaved = true;

            displayUnsaved.CheckIfSaved();
        }


        public class RegisteredState {
            public RegisteredState(Story story) {
                SerializedData = story.GetSerialized();
                IsThisStateSaved = false;
            }

            public string SerializedData { get; private set; }

            /// <summary>
            /// Is this state saved to file by the file system?
            /// </summary>
            public bool IsThisStateSaved { get; set; }
        }
    }
}