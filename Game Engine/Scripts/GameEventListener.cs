namespace Game {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using Sirenix.OdinInspector;

    public class GameEventListener : MonoBehaviour {
        [SerializeField] private GameEvent gameEvent;
        [SerializeField, DrawWithUnity] private UnityEvent response;

        public void OnEventRaised() {
            response.Invoke();
        }

        private void OnEnable() {
            if (gameEvent) {
                gameEvent.RegisterListener(this);
            }
        }

        private void OnDisable() {
            if (gameEvent) {
                gameEvent.UnregisterListener(this);
            }
        }
    }
}
