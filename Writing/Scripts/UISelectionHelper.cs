namespace Game.Writing {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;

    public class UISelectionHelper : MonoBehaviour {
        [SerializeField, Required]
        private EventSystem eventSystem;
        [SerializeField, Required]
        private PropertiesPanel propertiesPanel;

        /// <summary>
        /// Deselects any ui element and
        /// </summary>
        public void DeselectAnythingAndCloseProperties() {
            propertiesPanel.ClosePanel();

            eventSystem.SetSelectedGameObject(null);
        }
    }
}