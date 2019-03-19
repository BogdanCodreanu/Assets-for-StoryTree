namespace RazzielModules.UIHelper {
    using System.Collections;
    using System.Collections.Generic;
    using System;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using DG.Tweening;
    using TMPro;
    using UnityEngine.UI;
#pragma warning disable 0649 // disable default value warning

    public class UIHelperTooltipObject : MonoBehaviour {
        private RectTransform rectTransform { get { return (RectTransform)transform; } }

        [SerializeField, MinValue(0)]
        private float mainOffsetX, mainOffsetY;

        [SerializeField, Required]
        private TMP_Text tooltipText;

        [SerializeField, Required]
        private PanelsQuickAnimations showAnimations;

        private void Awake() {
            MoveToMouseAndOffset(mainOffsetX, -mainOffsetY);
            CheckPivotAndSize();
        }

        private void Update() {
            MoveToMouseAndOffset(mainOffsetX, -mainOffsetY);
            CheckPivotAndSize();
        }

        private void MoveToMouseAndOffset(float offsetX, float offsetY) {
            transform.position = Input.mousePosition;
            transform.position += new Vector3(offsetX, offsetY);
        }

        private void CheckPivotAndSize() {
            rectTransform.pivot = new Vector3(0, 1);

            if (rectTransform.anchoredPosition.x + rectTransform.sizeDelta.x >= Screen.width) {
                MoveToMouseAndOffset(0, 0);
                rectTransform.pivot = new Vector2(1, rectTransform.pivot.y);
            }

            if (rectTransform.anchoredPosition.y - rectTransform.sizeDelta.y <= 0) {
                MoveToMouseAndOffset(0, 0);
                rectTransform.pivot = new Vector2(rectTransform.pivot.x, 0);
            }
        }

        public void Init(string text) {
            tooltipText.text = text;
            showAnimations.PlayAllAnimations();
        }

        public void HideAndKill() {
            showAnimations.PlayAllAnimationsReversed();
            Destroy(gameObject, showAnimations.TotalAnimatingTime);
        }
    }
}
