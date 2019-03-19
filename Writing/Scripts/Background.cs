namespace Game.Writing {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using UnityEngine.Events;
    using DG.Tweening;

    public class Background : MonoBehaviour, IPointerDownHandler {
        [SerializeField, Required]
        private PanelsControllerAndLinker panelsController;

        [SerializeField, Required, AssetsOnly]
        private GameEvent rightQuickClickEvent, leftQuickClickEvent;

        //private Image image;
        private MouseButtonCounter rightMouseClick = new MouseButtonCounter(1, PointerEventData.InputButton.Right);
        private MouseButtonCounter leftMouseClick = new MouseButtonCounter(0, PointerEventData.InputButton.Left);

        [SerializeField, PropertyTooltip("How many seconds does it take to be considered a hold, not a " +
            "click")]
        private float clickPressHoldNeed = .5f;

        [SerializeField]
        private List<GridScaleImage> gridScales = new List<GridScaleImage>();

        [System.Serializable]
        public struct GridScaleImage {
            [SerializeField, Required]
            private Image gridImage;

            [SerializeField]
            private Vector2 opacityEnableFromTo;

            public void SetOpacity(float currentCameraOrth) {
                gridImage.color = new Color(gridImage.color.r, gridImage.color.g, gridImage.color.b,
                    Mathf.Clamp01((currentCameraOrth - opacityEnableFromTo.x) / (
                    opacityEnableFromTo.y - opacityEnableFromTo.x)));
            }
        }

        private void Awake() {
            //image = GetComponent<Image>();
            rightMouseClick.OnQuickClick.AddListener(rightQuickClickEvent.Raise);
            leftMouseClick.OnQuickClick.AddListener(leftQuickClickEvent.Raise);
        }


        public void OnPointerDown(PointerEventData eventData) {
            rightMouseClick.StartPressIfNeeded(eventData);
            leftMouseClick.StartPressIfNeeded(eventData);
        }

        private void Update() {
            rightMouseClick.CountPress(clickPressHoldNeed);
            leftMouseClick.CountPress(clickPressHoldNeed);
        }

        public class MouseButtonCounter {
            private int mouseButtonIndex;
            private PointerEventData.InputButton eventButton;

            public UnityEvent OnQuickClick { get; private set; }
            public UnityEvent OnStartHold { get; private set; }

            private bool isPressed;
            private float pressDuration;

            public MouseButtonCounter(int mouseButtonIndex, PointerEventData.InputButton inputButton) {
                this.mouseButtonIndex = mouseButtonIndex;
                this.eventButton = inputButton;
                OnQuickClick = new UnityEvent();
                OnStartHold = new UnityEvent();
            }

            /// <summary>
            /// Should be called on an event pointer down
            /// </summary>
            public void StartPressIfNeeded(PointerEventData eventData) {
                if (eventData.button == eventButton) {
                    StartedPress();
                }
            }

            private void StartedPress() {
                isPressed = true;
                pressDuration = 0f;
            }

            /// <summary>
            /// Should be called on Update
            /// </summary>
            public void CountPress(float durationConsideredHold) {
                if (isPressed) {
                    pressDuration += Time.deltaTime;
                    if (Input.GetMouseButtonUp(mouseButtonIndex)) {
                        ClickReleased(durationConsideredHold);
                    }
                }
            }

            private void ClickReleased(float durationConsideredHold) {
                isPressed = false;
                if (pressDuration >= durationConsideredHold) {
                    OnStartHold.Invoke();
                } else {
                    OnQuickClick.Invoke();
                }
            }
        }


        /// <summary>
        /// Change images grids opacity according to the new camera ortho size.
        /// </summary>
        /// <param name="currentCameraOrtho"></param>
        public void ChangeGrids(float currentCameraOrtho) {
            foreach (GridScaleImage image in gridScales) {
                image.SetOpacity(currentCameraOrtho);
            }
        }

    }
}