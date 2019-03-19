namespace Game.UIAddition {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using Settings;
    using Writing;
    using UnityEngine.EventSystems;
    using TMPro;

    public class DragWritingPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {
        [SerializeField, Required]
        private WritingPanel writingPanel;

        private Vector2 originalLocalPointerPosition;
        private Vector3 originalPanelLocalPosition;
        private RectTransform panelRectTransform;
        private RectTransform parentRectTransform;

        private List<WritingPanel> childrenPanels;
        private Vector3[] originalChildrenPanelsPosition;
        int i;

        private bool isBeingDragged;

        void Awake() {
            panelRectTransform = transform.parent as RectTransform;
            parentRectTransform = panelRectTransform.parent as RectTransform;
        }

        public void OnPointerDown(PointerEventData data) {
            if (data.button != PointerEventData.InputButton.Left) {
                return;
            }

            isBeingDragged = true;

            SaveChildrenInfo();

            originalPanelLocalPosition = panelRectTransform.localPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, data.position, data.pressEventCamera, out originalLocalPointerPosition);
        }

        private void SaveChildrenInfo() {
            childrenPanels = new List<WritingPanel>();
            writingPanel.GetAllLinkedChildren(ref childrenPanels);
            originalChildrenPanelsPosition = new Vector3[childrenPanels.Count];
            i = 0;
            foreach (WritingPanel panel in childrenPanels) {
                originalChildrenPanelsPosition[i] = ((RectTransform)panel.transform).localPosition;
                i++;
            }
        }

        public void OnDrag(PointerEventData data) {
            if (panelRectTransform == null || parentRectTransform == null ||
                data.button != PointerEventData.InputButton.Left)
                return;

            Vector2 localPointerPosition;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, data.position, data.pressEventCamera, out localPointerPosition)) {
                Vector3 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;
                panelRectTransform.localPosition = originalPanelLocalPosition + offsetToOriginal;
                MoveChildren(offsetToOriginal);
            }

            ClampToWindow();
        }

        private void MoveChildren(Vector3 offsetToOriginal) {
            if (childrenPanels == null) {
                Debug.LogError("Trying to move children panels of a node, but they are null");
                return;
            }
            i = 0;
            foreach (WritingPanel panel in childrenPanels) {
                ((RectTransform)panel.transform).localPosition = originalChildrenPanelsPosition[i] + offsetToOriginal;
                i++;
            }

        }

        // Clamp panel to area of parent
        void ClampToWindow() {
            Vector3 pos = panelRectTransform.localPosition;

            Vector3 minPosition = parentRectTransform.rect.min - panelRectTransform.rect.min;
            Vector3 maxPosition = parentRectTransform.rect.max - panelRectTransform.rect.max;

            pos.x = Mathf.Clamp(panelRectTransform.localPosition.x, minPosition.x, maxPosition.x);
            pos.y = Mathf.Clamp(panelRectTransform.localPosition.y, minPosition.y, maxPosition.y);

            panelRectTransform.localPosition = pos;
        }

        public void OnPointerUp(PointerEventData eventData) {
            if (!isBeingDragged) {
                return;
            }
            RegisterActions.Instance.RegisterNewState();
            isBeingDragged = false;
        }
    }
}