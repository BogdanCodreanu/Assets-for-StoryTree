namespace RazzielModules.UIHelper {
    using System.Collections;
    using System.Collections.Generic;
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

#pragma warning disable 0649 // disable default value warning

    /// <summary>
    /// Drop this script on an UI element to create a tooltip while mouse is over
    /// </summary>
    public class UIElementTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        [SerializeField, TextArea]
        private string tooltipMessage;

        private UIHelperTooltipObject tooltipObject;

        public void OnPointerEnter(PointerEventData eventData) {
            tooltipObject = UIHelper.Instance.CreateTooltip(tooltipMessage);
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (tooltipObject == null)
                return;

            tooltipObject.HideAndKill();
            tooltipObject = null;
        }
    }
}
