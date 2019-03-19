namespace Game.Writing {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using UnityEngine.UI;
    using DG.Tweening;
    using TMPro;
#pragma warning disable 0649 // disable default value warning

    public class PropertiesPanel : MonoBehaviour {
        private RectTransform rectTransform { get { return (RectTransform)transform; } }

        [SerializeField, BoxGroup("Animation")]
        private float openingDuration = .5f;
        [SerializeField, BoxGroup("Animation")]
        private Ease openingEase = Ease.OutQuad;

        [SerializeField, Required]
        private Transform contentHolder, offScreenContent;
        [SerializeField, Required]
        private TMP_Text propertiesTitle;

        public enum AppearanceState { OffScreen, AnimatingOnScreen, OnScreen, AnimatingOffScreen }

        /// <summary>
        /// In what appearance state is the panel?
        /// </summary>
        private AppearanceState appState = AppearanceState.OffScreen;

        /// <summary>
        /// Current shown properties. Is null if no propery is currently on screen.
        /// This changes when the panel is off screen, and deletes previous properties.
        /// </summary>
        private GameObject currentProperties;

        private GameObject previousPrefabGiven = null;

        public void ClosePanel() {
            PlayCloseAnimation();
        }


        private void PlayOpenAnimation() {
            if (appState == AppearanceState.OnScreen || appState == AppearanceState.AnimatingOnScreen)
                return;

            DOTween.Kill(rectTransform);
            appState = AppearanceState.AnimatingOnScreen;
            rectTransform.DOAnchorPosX(-rectTransform.sizeDelta.x, openingDuration)
                .SetEase(openingEase).OnComplete(delegate { appState = AppearanceState.OnScreen; });
        }

        private void PlayCloseAnimation() {
            if (appState == AppearanceState.OffScreen || appState == AppearanceState.AnimatingOffScreen)
                return;

            DOTween.Kill(rectTransform);
            appState = AppearanceState.AnimatingOffScreen;
            rectTransform.DOAnchorPosX(0, openingDuration)
                .SetEase(openingEase).OnComplete(delegate {
                    appState = AppearanceState.OffScreen;
                    if (currentProperties) {
                        Destroy(currentProperties);
                    }
                    previousPrefabGiven = null;
                });
        }

        /// <summary>
        /// Spawns and gets the properties object.
        /// </summary>
        /// <param name="propertiesPrefab">The properties prefab to spawn</param>
        /// <param name="propertiesTitle">What title should the panel have?</param>
        /// <returns>Returns null if it was an invalid call. Meaning that the panel will not spawn
        /// the given object. Othewise will return the spawned object.</returns>
        public GameObject SpawnAndShowProperties(GameObject propertiesPrefab, string propertiesTitle) {
            if (propertiesPrefab == previousPrefabGiven) {
                return null;
            }

            GameObject spawn = Instantiate(propertiesPrefab, offScreenContent);
            previousPrefabGiven = propertiesPrefab;
            StartCoroutine(ChangeAndShowProperties(spawn, propertiesTitle));
            return spawn;
        }

        private IEnumerator ChangeAndShowProperties(GameObject newProperties, string newTitle) {
            PlayCloseAnimation();
            yield return new WaitUntil(delegate { return appState == AppearanceState.OffScreen; });
            if (currentProperties) {
                Destroy(currentProperties);
            }
            currentProperties = newProperties;
            currentProperties.transform.SetParent(contentHolder, false);
            propertiesTitle.text = newTitle;

            PlayOpenAnimation();
        }
    }
}