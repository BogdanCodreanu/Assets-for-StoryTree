namespace Game.UIAddition {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using DG.Tweening;
    using TMPro;

    public class TextAppear : MonoBehaviour {
        private TMP_Text text;

        [SerializeField, MinValue(0)]
        private float appearSpeed = .5f;
        [SerializeField]
        private Ease appearEase = Ease.OutQuad;
        [SerializeField, MinValue(0)]
        private float disappearSpeed = .5f;
        [SerializeField]
        private Ease disappearEase = Ease.OutQuad;


        private Tweener currentTweener;

        [SerializeField]
        private bool defaultZeroAlpha;

        private void Awake() {
            text = GetComponent<TMP_Text>();
            if (defaultZeroAlpha) {
                SetAlphaZero();
            }
        }

        [Button]
        public void SetAlphaZero() {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        }

        [Button]
        public void Appear() {
            if (currentTweener != null && currentTweener.IsPlaying()) {
                currentTweener.Kill();
            }
            text.DOFade(1, appearSpeed)
                .SetEase(appearEase)
                .OnKill(delegate { currentTweener = null; });
        }

        [Button]
        public void Disappear() {
            if (currentTweener != null && currentTweener.IsPlaying()) {
                currentTweener.Kill();
            }
            text.DOFade(0, disappearSpeed)
                .SetEase(disappearEase)
                .OnKill(delegate { currentTweener = null; });
        }
    }
}
