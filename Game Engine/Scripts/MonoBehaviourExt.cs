namespace Game {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public static class MonoBehaviourExt {
        public static void ExecuteWithDelay(this MonoBehaviour mono, float delay, UnityAction action) {
            if (delay <= 0) {
                action();
            } else {
                mono.StartCoroutine(Executing(delay, action));
            }
        }

        private static IEnumerator Executing(float delay, UnityAction action) {
            yield return new WaitForSeconds(delay);
            action();
        }
    }

    public static class GameObjectExt {
        /// <summary>
        /// Creates a duplicate object of this game object, sets it as active and inherits the same
        /// transform data, including the parent
        /// </summary>
        /// <returns>The spawned object</returns>
        public static GameObject CreateObjectFromPreset(this GameObject presetObject) {
            GameObject spawn = GameObject.Instantiate(presetObject.gameObject,
                presetObject.transform.position, presetObject.transform.rotation,
                presetObject.transform.parent);

            spawn.gameObject.SetActive(true);
            return spawn;
        }
    }
}
