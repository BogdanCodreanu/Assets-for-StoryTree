namespace Game.Writing {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using UnityEngine.UI;

    /// <summary>
    /// This is used as a container for the writing panel.
    /// Because i want each panel to be on a different canvas.
    /// </summary>
    public class WritingCanvas : MonoBehaviour {
        [SerializeField, Required]
        private WritingPanel writingPanel;
        
        public WritingPanel WritingPanel { get { return writingPanel; } }

        public void MovePanelAt(Vector3 atPosition) {
            writingPanel.transform.position = atPosition;
        }

        /// <summary>
        /// Destroys the game object from scene. Also the containing panel
        /// </summary>
        public void DestroySelfAndWritingPanel() {
            Destroy(gameObject);
        }
    }
}