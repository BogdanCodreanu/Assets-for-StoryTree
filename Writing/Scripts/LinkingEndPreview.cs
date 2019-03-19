namespace Game.Writing {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using DG.Tweening;

    public class LinkingEndPreview : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler {
        private RectTransform rectTransform { get { return (RectTransform)transform; } }

        private float initialHeight;
        private Image image;

        private Tweener turningOnTweener;
        private Color initialColor;
        private Color MouseOverColor { get { return PanelsController.LinkingColor; } }

        private PanelsControllerAndLinker PanelsController { get { return writingPanel.PanelsController; } }

        [SerializeField, Required]
        private WritingPanel writingPanel;

        private void Awake() {
            image = GetComponent<Image>();
            initialColor = image.color;
            initialHeight = rectTransform.sizeDelta.y;
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 0);
            ShowOrHideImage(false);

        }

        private void ShowOrHideImage(bool show) {
            image.color = new Color(image.color.r, image.color.g, image.color.b, show ? initialColor.a : 0);
        }

        /// <summary>
        /// Turns preview on or off.
        /// </summary>
        /// <param name="isOn">True means the preview will be shown.</param>
        public void ToggleState(bool isOn) {
            if (turningOnTweener != null && turningOnTweener.IsPlaying()) {
                turningOnTweener.Kill();
            }

            image.raycastTarget = isOn;

            turningOnTweener =
                rectTransform.DOSizeDelta(new Vector2(rectTransform.sizeDelta.x, isOn ? initialHeight : 0),
                .5f).SetEase(Ease.OutQuad)
                .OnKill(delegate { turningOnTweener = null; });

            if (isOn) {
                turningOnTweener.OnStart(delegate { ShowOrHideImage(true); });
            } else {
                turningOnTweener.OnComplete(delegate { ShowOrHideImage(false); });
            }
        }

        private void GiveAccordingColor(bool isMouseOver) {
            if (isMouseOver) {
                image.DOColor(MouseOverColor, .1f).SetEase(Ease.OutQuad);
            } else {
                image.DOColor(initialColor, .1f).SetEase(Ease.OutQuad);
            }
        }

        public void OnPointerEnter(PointerEventData eventData) {
            GiveAccordingColor(true);
        }

        public void OnPointerExit(PointerEventData eventData) {
            GiveAccordingColor(false);
        }

        public void OnPointerDown(PointerEventData eventData) {
            if (eventData.button == PointerEventData.InputButton.Left)
                PanelsController.SelectedLinkEnd(writingPanel);
        }
    }
}