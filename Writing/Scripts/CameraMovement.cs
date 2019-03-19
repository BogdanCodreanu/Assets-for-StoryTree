namespace Game.Writing {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using DG.Tweening;
    using UnityEngine.UI;
    using Sirenix.Serialization;

    public class CameraMovement : MonoBehaviour {
        private Vector3 lastMousePos;
        private Vector3 currentMousePos;
        /// <summary>
        /// Position used to calculate where should be the next position
        /// </summary>
        private Vector3 deltaMousePos;

        private new Camera camera;

        private const int MovementMouseButtonIndex = 1;

        /// <summary>
        /// Frame that the movement mouse button has been pressed
        /// </summary>
        private bool MovementButtonDown {
            get { return Input.GetMouseButtonDown(MovementMouseButtonIndex); }
        }
        /// <summary>
        /// Is the mouse movement button pressed?
        /// </summary>
        private bool MovementButton {
            get { return Input.GetMouseButton(MovementMouseButtonIndex); }
        }

        private float ScrollWheelValue {
            get { return Input.GetAxis("Mouse ScrollWheel"); }
        }

        [SerializeField, PropertyTooltip("How long will it take to complete a scroll step")]
        private float scrollSpeed = .3f;
        [SerializeField, PropertyTooltip("How much will the camera scroll with each step")]
        private float scrollTermen = .5f;
        [SerializeField]
        private Ease scrollingEase = Ease.InOutSine;
        [SerializeField, PropertyTooltip("Limits of scrolling")]
        private Vector2 scrollingLimits;

        public float CurrentOthoSize { get { return camera.orthographicSize; } }

        [SerializeField, PropertyTooltip("Scales the termen of zooming with this curve," +
            " from 0 to 1. 0 representing the lowest limit of the scroll, and 1 the highest")]
        private AnimationCurve scrollingFactor;

        private Tweener scrollingTweener;
        /// <summary>
        /// Next orthographic size
        /// </summary>
        private float futureOrthSize;

        [SerializeField, Required]
        private Background background;

        [SerializeField, Required]
        private CameraMovementLimits movementLimits;

        private void Awake() {
            camera = GetComponent<Camera>();
            futureOrthSize = camera.orthographicSize;
        }

        private void Start() {
            ChangeBackgroundGrids();
        }

        private void CalculateMovementDeltaFromInput() {
            if (MovementButtonDown) {
                lastMousePos = Input.mousePosition;
            }
            if (MovementButton) {
                currentMousePos = Input.mousePosition;
                deltaMousePos = MouseToWorldConversion(lastMousePos) -
                    MouseToWorldConversion(currentMousePos);

                lastMousePos = currentMousePos;
            }
        }

        public static Vector3 MouseToWorldConversion(Vector3 inputMousePosition) {
            return new Vector3(Camera.main.ScreenToWorldPoint(inputMousePosition).x,
                Camera.main.ScreenToWorldPoint(inputMousePosition).y, 0);
        }

        private void Update() {
            CalculateMovementDeltaFromInput();

            if (MovementButton) {
                transform.position = movementLimits
                    .ClosestPointInBounds(transform.position + deltaMousePos);
            }
            ZoomIfNeeded();
        }

        private void ZoomIfNeeded() {
            if (ScrollWheelValue != 0) {
                if (scrollingTweener != null && scrollingTweener.IsPlaying()) {
                    scrollingTweener.Kill();
                }

                futureOrthSize -= scrollTermen * Mathf.Sign(ScrollWheelValue) *
                    scrollingFactor.Evaluate((futureOrthSize - scrollingLimits.x) /
                    (scrollingLimits.y - scrollingLimits.x));

                futureOrthSize = Mathf.Clamp(futureOrthSize, scrollingLimits.x, scrollingLimits.y);

                scrollingTweener =
                    camera.DOOrthoSize(futureOrthSize, scrollSpeed)
                    .SetEase(scrollingEase)
                    .OnKill(delegate { scrollingTweener = null; })
                    .OnUpdate(ChangeBackgroundGrids);
            }
        }

        public void JumpToPosition(Vector3 newPosition) {
            deltaMousePos = Vector3.zero;
            newPosition = new Vector3(newPosition.x, newPosition.y, transform.position.z);
            transform.DOMove(newPosition, .3f).SetEase(Ease.InOutQuad);
        }

        /// <summary>
        /// Change opacity of background grids.
        /// </summary>
        private void ChangeBackgroundGrids() {
            background.ChangeGrids(CurrentOthoSize);
        }


    }
}