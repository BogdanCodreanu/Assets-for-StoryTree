namespace Game.UIAddition {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using DG.Tweening;
    using TMPro;
    using UnityEngine.EventSystems;

    public class CustomInputModule : StandaloneInputModule {
        [SerializeField]
        private int leftMouseButtonPointerIndex = 1;

        public PointerEventData GetPointerData() {
            return m_PointerData[leftMouseButtonPointerIndex];
        }
    }
}
