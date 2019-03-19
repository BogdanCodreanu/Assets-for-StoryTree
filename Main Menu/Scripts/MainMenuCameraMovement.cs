namespace Game.MainMenu {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using DG.Tweening;
    using SceneObjects;

    public class MainMenuCameraMovement : MonoBehaviour {
        [SerializeField]
        private MovePosition toMainButtons, toReading, toWriting;

        [SerializeField]
        private float animDuration;
        [SerializeField]
        private Ease animEaseUsed = Ease.OutQuad;
        [SerializeField, Required]
        private CameraMouseRotation cameraRotation;

        public void GoToReading() {
            toReading.MoveToThisPoint(transform, animDuration, animEaseUsed, cameraRotation);
        }
        public void GoToWriting() {
            toWriting.MoveToThisPoint(transform, animDuration, animEaseUsed, cameraRotation);
        }

        public void GoToMainButtons() {
            toMainButtons.MoveToThisPoint(transform, animDuration, animEaseUsed, cameraRotation);
        }

        [System.Serializable]
        public class MovePosition {
            public Transform copyTransform;

            public void MoveToThisPoint(Transform movedTrans, float duration, Ease ease,
                CameraMouseRotation cameraMouseRotation) {
                // also stops mouse rotation and turns it back on
                cameraMouseRotation.TurnOffFreeRotation();

                movedTrans.DOMove(copyTransform.position, duration).SetEase(ease);
                movedTrans.DORotate(copyTransform.eulerAngles, duration).SetEase(ease)
                    .OnComplete(delegate {
                        cameraMouseRotation.CreatePlane();
                        cameraMouseRotation.TurnOnFreeRotation();
                    });
            }
        }

#if UNITY_EDITOR
        [Button, HorizontalGroup(GroupID = "1")]
        private void SnapCameraToReading() {
            InstantCopyTrans(transform, toReading.copyTransform);
        }
        [Button, HorizontalGroup(GroupID = "1")]
        private void SnapReadingToCamera() {
            InstantCopyTrans(toReading.copyTransform, transform);
        }

        [Button, HorizontalGroup(GroupID = "2")]
        private void SnapCameraToWriting() {
            InstantCopyTrans(transform, toWriting.copyTransform);
        }
        [Button, HorizontalGroup(GroupID = "2")]
        private void SnapWritingToCamera() {
            InstantCopyTrans(toWriting.copyTransform, transform);
        }


        [Button, HorizontalGroup(GroupID = "3")]
        private void SnapCameraToMain() {
            InstantCopyTrans(transform, toMainButtons.copyTransform);
        }
        [Button, HorizontalGroup(GroupID = "3")]
        private void SnapMainToCamera() {
            InstantCopyTrans(toMainButtons.copyTransform, transform);
        }

        private void InstantCopyTrans(Transform snapped, Transform to) {
            snapped.position = to.position;
            snapped.rotation = to.rotation;
        }
#endif
    }
}
