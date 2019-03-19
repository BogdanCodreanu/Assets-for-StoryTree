namespace Game.UIAddition {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using Sirenix.OdinInspector;
    using DG.Tweening;

    public class GraphicQuickFade : MonoBehaviour {
        //private MaskableGraphic maskableGraphic;
        private MaskableGraphic[] maskableGraphics;
        private GraphicFaded[] graphicsFaded;

        [SerializeField]
        private FadeType fadingIn;
        [SerializeField]
        private FadeType fadingOut;
        [SerializeField, PropertyTooltip("If checked, then all children and the object " +
            "will be faded. Otherwise, only this object")]
        private bool multipleGraphics;
        
        [SerializeField, PropertyTooltip("Will awake graphics with alpha = 0")]
        private bool startWithZeroAlpha;

        private void Awake() {
            CalculateChildren();
        }

        private void CalculateChildren() {
            if (multipleGraphics) {
                maskableGraphics = GetComponentsInChildren<MaskableGraphic>();
            } else {
                maskableGraphics = GetComponents<MaskableGraphic>();
            }
            graphicsFaded = new GraphicFaded[maskableGraphics.Length];
        }

        private void SaveAlphas() {
            for (int i = 0; i < maskableGraphics.Length; i++) {
                graphicsFaded[i] = new GraphicFaded(maskableGraphics[i], startWithZeroAlpha);
            }
        }

        private void Start() {
            SaveAlphas();
        }

        public void RecalculateChildren() {
            CalculateChildren();
            SaveAlphas();
        }

        /// <summary>
        /// From 0 to the existing value
        /// </summary>
        public void FadeIn() {
            foreach (GraphicFaded graphic in graphicsFaded) {
                graphic.FadeIn(fadingIn);
            }
        }

        /// <summary>
        /// From existing value to 0
        /// </summary>
        public void FadeOut() {
            foreach (GraphicFaded graphic in graphicsFaded) {
                graphic.FadeOut(fadingOut);
            }
        }


        public bool HasFinishedFadingOut() {
            foreach (GraphicFaded graphic in graphicsFaded) {
                if (!graphic.HasFinishedFadingOut()) {
                    return false;
                }
            }
            return true;
        }

        [System.Serializable]
        public struct FadeType {
            [SerializeField, MinValue(0)]
            private float defaultTime;
            [SerializeField]
            private Ease easeType;

            public Tweener Fade(MaskableGraphic graphic, float endValue) {
                return graphic.DOFade(endValue, defaultTime).SetEase(easeType).SetUpdate(true);
            }
        }

        public class GraphicFaded {
            private MaskableGraphic maskableGraphic;
            private Tweener currentTweener;
            private float initialAlphaValue;

            public GraphicFaded(MaskableGraphic graphic, bool awakeWithZeroAlpha) {
                maskableGraphic = graphic;
                initialAlphaValue = graphic.color.a;

                if (awakeWithZeroAlpha) {
                    graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 0);
                }
            }

            private void ResetAlpha() {
                maskableGraphic.color = new Color(maskableGraphic.color.r,
                    maskableGraphic.color.g, maskableGraphic.color.b, initialAlphaValue);
            }

            /// <summary>
            /// From 0 to the existing value
            /// </summary>
            public void FadeIn(FadeType fadingIn) {
                if (currentTweener != null && currentTweener.IsPlaying()) {
                    currentTweener.Kill();
                }
                ResetAlpha();
                currentTweener = fadingIn.Fade(maskableGraphic, 0).From()
                    .OnKill(delegate { currentTweener = null; });
            }

            /// <summary>
            /// From existing value to 0
            /// </summary>
            public void FadeOut(FadeType fadingOut) {
                if (currentTweener != null && currentTweener.IsPlaying()) {
                    currentTweener.Kill();
                }
                currentTweener = fadingOut.Fade(maskableGraphic, 0)
                    .OnKill(delegate { currentTweener = null; });
            }

            public bool HasFinishedFadingOut() {
                return !currentTweener.IsPlaying();
            }
        }
    }
}
