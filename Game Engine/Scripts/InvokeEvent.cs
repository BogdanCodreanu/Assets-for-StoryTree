namespace Game {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using Sirenix.OdinInspector;

    public class InvokeEvent : MonoBehaviour {
        public enum InvokeTiming { OnAwake, OnStart, OnStartWithDelay, OnCustomCall }
        [SerializeField, PropertyTooltip("When should the event be called")]
        private InvokeTiming timing;

        [SerializeField, ShowIf("timing", InvokeTiming.OnStartWithDelay)]
        private float delay;

        [SerializeField, DrawWithUnity]
        private UnityEvent unityEvent;

        private void Awake() {
            if (timing == InvokeTiming.OnAwake) {
                unityEvent.Invoke();
            }
        }

        private void Start() {
            if (timing == InvokeTiming.OnStart) {
                unityEvent.Invoke();
            } else if (timing == InvokeTiming.OnStartWithDelay) {
                StartCoroutine(CallWithDelay());
            }
        }

        private IEnumerator CallWithDelay() {
            yield return new WaitForSecondsRealtime(delay);
            unityEvent.Invoke();
        }

        /// <summary>
        /// Use this to call at a desired time. When you don't want automated time.
        /// </summary>
        [Button("Custom Call", ButtonSizes.Medium)]
        public void CustomCall() {
            if (timing != InvokeTiming.OnCustomCall) {
                return;
            }
            unityEvent.Invoke();
        }
    }
}
