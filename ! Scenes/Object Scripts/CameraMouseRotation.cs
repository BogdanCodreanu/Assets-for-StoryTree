namespace Game.SceneObjects {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using DG.Tweening;

    public class CameraMouseRotation : MonoBehaviour {
        [SerializeField, Required]
        private Transform cameraTransform;
        private Camera MainCamera { get { return Camera.main; } }

        [SerializeField, Required]
        private Transform invisibleRotator;

        private Plane plane;
        private Quaternion initialRotationForCurrentPlane;
        private const float PlaneAtDistance = 5f;
        private Ray cameraRay;
        private Vector3 hitPoint;
        private float rayHitAtDistance;

        /// <summary>
        /// Point right in front of the camera.
        /// </summary>
        private Vector3 middleCameraPlanePoint;

        [SerializeField]
        [PropertyTooltip("How fast should the camera look around")]
        private float speed = 1f;
        [SerializeField]
        [PropertyTooltip("How far should the camera rotation be allowed")]
        private float maxRaycastRadius = 5f;
        [SerializeField]
        [PropertyTooltip("How long should it take for the free rotation to be avaliable after it's been" +
            " turned on. The duration of the fading in of the effect.")]
        private float turningOnDuration = 1f;

        /// <summary>
        /// How much the camera rotation should be influenced by the free rotation.
        /// </summary>
        private float currentRotationFactor = 0;
        private Tweener turningOnFreeRotationTweener;
        
        private void Awake() {
            CreatePlane();
        }

        private void Start() {
            TurnOnFreeRotation();
        }

        /// <summary>
        /// Creates the plane for the mouse ray point.
        /// </summary>
        public void CreatePlane() {
            middleCameraPlanePoint = cameraTransform.forward * PlaneAtDistance + cameraTransform.position;
            plane = new Plane(middleCameraPlanePoint,
                middleCameraPlanePoint + cameraTransform.right,
                middleCameraPlanePoint + cameraTransform.up
                );
            initialRotationForCurrentPlane = cameraTransform.rotation;
        }

        private void CalculateRayAndHitPoint() {
            cameraRay = MainCamera.ScreenPointToRay(Input.mousePosition);

            if (!plane.Raycast(cameraRay, out rayHitAtDistance)) {
                Debug.LogError("Camera ray did not hit created plane.");
                return;
            }
            hitPoint = cameraRay.GetPoint(rayHitAtDistance);


            hitPoint = Vector3.ClampMagnitude(hitPoint - middleCameraPlanePoint, maxRaycastRadius) +
                middleCameraPlanePoint;
        }

        private void Update() {
            if (currentRotationFactor == 0)
                return;

            cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation,
                CalculateDesiredRotation(), speed * Time.deltaTime * currentRotationFactor);
        }

        public void TurnOffFreeRotation() {
            if (turningOnFreeRotationTweener != null) {
                turningOnFreeRotationTweener.Kill();
                Debug.Log("turning on stopped");
            }
            currentRotationFactor = 0;
        }

        public void TurnOnFreeRotation() {
            turningOnFreeRotationTweener =
                DOTween.To(delegate (float value) { currentRotationFactor = value; },
                0, 1, turningOnDuration).SetEase(Ease.InOutQuad)
                .OnComplete(delegate { turningOnFreeRotationTweener = null; });
        }

        private Quaternion CalculateDesiredRotation() {
            CalculateRayAndHitPoint();

            invisibleRotator.position = cameraTransform.position;
            invisibleRotator.LookAt(hitPoint);

            return invisibleRotator.rotation;
        }
    }
}