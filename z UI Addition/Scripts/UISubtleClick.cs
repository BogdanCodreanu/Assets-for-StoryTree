namespace Game.UIAddition {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using UnityEngine.Events;

    public class UISubtleClick : MonoBehaviour, IPointerClickHandler {
        [SerializeField]
        private UnityEvent actionOnClick = new UnityEvent();
        public UnityEvent ActionOnClick { get { return actionOnClick; } }

        public void OnPointerClick(PointerEventData eventData) {
            if (eventData.button != PointerEventData.InputButton.Left) {
                return;
            }
            actionOnClick.Invoke();
        }
    }
}