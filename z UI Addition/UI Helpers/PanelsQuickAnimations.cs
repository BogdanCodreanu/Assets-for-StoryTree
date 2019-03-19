namespace RazzielModules.UIHelper {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using DG.Tweening;
    using UnityEngine.UI;

    public class PanelsQuickAnimations : MonoBehaviour {

        [SerializeField]
        private List<AnimationForElements> animations = new List<AnimationForElements>();

        /// <summary>
        /// How long does it take for animations to complete?
        /// </summary>
        public float TotalAnimatingTime {
            get {
                float maxAnimTime = 0f;
                foreach (AnimationForElements anim in animations) {
                    maxAnimTime = Mathf.Max(maxAnimTime, anim.AnimationTime);
                }
                return maxAnimTime;
            }
        }

        private void Awake() {
            foreach (AnimationForElements anims in animations) {
                anims.Awake();
            }
        }

        [Button("Play All", ButtonSizes.Medium)]
        public void PlayAllAnimations() {
            foreach (AnimationForElements anims in animations) {
                anims.KillPreviousAnimations();
                anims.StartAnimations(false);
            }
        }
        /// <summary>
        /// Plays the same animation, but it reverses FROM value.
        /// </summary>
        public void PlayAllAnimationsReversed() {
            foreach (AnimationForElements anims in animations) {
                anims.KillPreviousAnimations();
                anims.StartAnimations(true);
            }
        }

        [System.Serializable]
        public class AnimationForElements {
            [SerializeField]
            private Type type;
#if UNITY_EDITOR
            [SerializeField, GUIColor(.8f, .8f, 1f)]
            private string comment;
#endif

            [SerializeField]
            private Animation appliedAnimation;

            /// <summary>
            /// How long does it take for this animation to complete?
            /// </summary>
            public float AnimationTime {
                get { return appliedAnimation.delay + appliedAnimation.duration; }
            }
            
            public enum Type { Fade, Resize }

            [SerializeField, ShowIf("type", Type.Fade)]
            private List<MaskableGraphic> graphicsToFade;
            private Dictionary<MaskableGraphic, float> initialAlphas
                = new Dictionary<MaskableGraphic, float>();

            [SerializeField, ShowIf("type", Type.Resize)]
            private List<RectTransform> rectsToResize;

            public void Awake() {
                foreach (MaskableGraphic graphic in graphicsToFade) {
                    initialAlphas.Add(graphic, graphic.color.a);
                }
            }

            public void KillPreviousAnimations() {
                if (type == Type.Fade) {
                    foreach (MaskableGraphic graphic in graphicsToFade) {
                        DOTween.Kill(graphic, true);
                    }
                } else {
                    foreach (RectTransform rect in rectsToResize) {
                        DOTween.Kill(rect, true);
                    }
                }
            }

            /// <summary>
            /// Starts animating the current elements.
            /// </summary>
            /// <param name="reverse">If true, it will play animations with FROM, or without
            /// if FROM was true</param>
            public void StartAnimations(bool reverse) {
                if (type == Type.Fade) {
                    foreach (MaskableGraphic graphic in graphicsToFade) {
                        if (graphic == null)
                            Debug.LogError("Item assign for animation is null. Remove from list.");
                        appliedAnimation.ApplyAnimation(graphic, initialAlphas[graphic],
                            null, reverse, type);
                    }
                } else {
                    foreach (RectTransform rect in rectsToResize) {
                        if (rect == null)
                            Debug.LogError("Item assign for animation is null. Remove from list");
                        appliedAnimation.ApplyAnimation(null, 0, rect, reverse, type);
                    }
                }
            }
        }
        
        [System.Serializable]
        public class Animation {
            public float duration = 1;
            public float delay;

            [TabGroup("Fade")]
            public float fadeValue;

            [TabGroup("Resize")]
            [PropertyTooltip("If true, will use exact numerical input. Otherwise, will use " +
                "the same rect.sizeDelta multiplied by the given value.")]
            public bool useExactScaleValue = false;
            [TabGroup("Resize"), HideIf("useExactScaleValue")]
            public float scaleValueUniform;
            [TabGroup("Resize"), ShowIf("useExactScaleValue")]
            public Vector2 scaleValue;
            [TabGroup("Resize"), ShowIf("useExactScaleValue")]
            public bool keepObjectX, keepObjectY;

            public Ease ease = Ease.OutQuad;
            [PropertyTooltip("If true, then animation will be FROM, otherwise TO")]
            public bool from;


            public void ApplyAnimation(MaskableGraphic graphic, float initialAlpha,
                RectTransform rect, bool reverseFrom,
                AnimationForElements.Type type) {

                bool fromUsed = this.from;
                Tweener currentTweener;

                if (type == AnimationForElements.Type.Resize) {
                    currentTweener =
                        rect.DOSizeDelta(!useExactScaleValue ? rect.sizeDelta * scaleValueUniform : 
                            new Vector2(keepObjectX ? rect.sizeDelta.x : scaleValue.x,
                            keepObjectY ? rect.sizeDelta.y : scaleValue.y),
                        duration);
                } else {
                    currentTweener = graphic.DOFade(fadeValue, duration);
                }

                currentTweener.SetDelay(delay).SetEase(ease);

                if (reverseFrom) {
                    fromUsed = !fromUsed;
                }
                if (fromUsed) {
                    if (type == AnimationForElements.Type.Fade) {
                        graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b,
                            initialAlpha);
                    }

                    currentTweener.From();
                }
            }
        }
    }
}
