namespace Game.Writing {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using DG.Tweening;
    using UnityEngine.UI;
    using Sirenix.Serialization;

    public class CameraMovementLimits : MonoBehaviour {
        [SerializeField, Required]
        private WritingStoryCreator storyCreator;
        private List<WritingPanel> WritingPanels { get { return storyCreator.WritingPanels; } }


        [SerializeField, MinValue(0)]
        private float boundsExpanding = 10f;

        private Bounds bounds;
        private float cameraZ;
        private Vector3 closestPoint;

        /// <summary>
        /// Whether or not a point is inside the writing panel created bounds.
        /// </summary>
        public bool IsPointInBounds(Vector3 point) {
            point.z = 0f; // z does not matter

            bounds = new Bounds();

            foreach (WritingPanel panel in WritingPanels) {
                bounds.Encapsulate(panel.CenterPanelPosition);
            }

            bounds.Expand(boundsExpanding);
            
            return bounds.Contains(point);
        }

        /// <summary>
        /// Finds out where the camera should be placed, when it wants to go in a wanted point.
        /// </summary>
        public Vector3 ClosestPointInBounds(Vector3 wantedPoint) {
            cameraZ = wantedPoint.z;

            wantedPoint.z = 0f; // z does not matter

            bounds = new Bounds();

            foreach (WritingPanel panel in WritingPanels) {
                bounds.Encapsulate(panel.CenterPanelPosition);
            }
            bounds.Expand(boundsExpanding);

            closestPoint = bounds.ClosestPoint(wantedPoint);
            return new Vector3(closestPoint.x, closestPoint.y, cameraZ);
        }
    }
}