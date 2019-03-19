namespace Game {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;

    [CreateAssetMenu(menuName = "Game/Game Event")]
    public class GameEvent : ScriptableObject {
        [ShowInInspector, ReadOnly]
        private List<GameEventListener> listeners = new List<GameEventListener>();

        public void Raise() {
            for (int i = listeners.Count - 1; i >= 0; i--) {
                listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(GameEventListener eventListener) {
            listeners.Add(eventListener);
        }

        public void UnregisterListener(GameEventListener eventListener) {
            listeners.Remove(eventListener);
        }
    }
}
