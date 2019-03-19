namespace Game.UIAddition {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using UnityEngine.UI;

    public class ButtonsQuickInteractible : MonoBehaviour {
        [InfoBox("Allows you to quickly change if multiple buttons are interactible or not")]
        [SerializeField, Required]
        private Button[] targetedButtons;


        /// <summary>
        /// Sets all buttons interactible value.
        /// </summary>
        /// <param name="value">If true, buttons will be interactible. Otherwise, they will not.</param>
        public void SetButtonsInteractible(bool value) {
            foreach (Button butt in targetedButtons) {
                butt.interactable = value;
            }
        }
        
    }
}