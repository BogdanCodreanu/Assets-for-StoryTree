namespace Game.Writing {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using TMPro;
#pragma warning disable 0649 // disable default value warning

    public class DisplayUnsavedProgress : MonoBehaviour {
        [SerializeField, Required]
        private RegisterActions registerActions;

        private bool IsCurrentStateSaved { get { return registerActions.CurrentState.IsThisStateSaved; } }

        [SerializeField, Required]
        private TMP_Text markStarText;

        /// <summary>
        /// Checks if the current state of the story is saved and displays on screen of it's saved
        /// </summary>
        public void CheckIfSaved() {
            if (!IsCurrentStateSaved) {
                markStarText.text = "*";
            } else {
                markStarText.text = "";
            }
        }
    }
}