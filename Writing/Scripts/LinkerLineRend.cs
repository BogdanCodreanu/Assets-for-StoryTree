namespace Game.Writing {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using UnityEngine.UI;
    using System.Linq;
    using DG.Tweening;
    using UIAddition;

    [RequireComponent(typeof(IWritingLinker))]
    public class LinkerLineRend : MonoBehaviour {
        private PanelsControllerAndLinker PanelsControllerAndLinker
            { get { return linker.ContainingPanel.PanelsController; } }
        [SerializeField, Required]
        private LineRenderer lineRend;
        [SerializeField, Required]
        private BezierCurveLineRenderer bezierLineRend;
        private Vector3[] bezierPoints = new Vector3[6];

        private IWritingLinker linker;

        private bool lineRendVisible = false;
        
        private void Awake() {
            linker = GetComponent<IWritingLinker>();
        }


        private void Update() {
            if (linker.ShouldDisplayLineRend) {
                MoveLineRend();
            } else {
                HideLineRend();
            }
        }

        private void HideLineRend() {
            if (lineRendVisible) {
                DoLineRendWidth(0).OnKill(delegate { lineRend.positionCount = 0; })
                    .SetEase(Ease.OutQuad);
            }
            lineRendVisible = false;
        }

        private void ShowLineRend() {
            if (!lineRendVisible) {
                DoLineRendWidth(1)
                    .SetEase(Ease.OutQuad);
            }
            lineRendVisible = true;
        }

        private Tweener DoLineRendWidth(float to) {
            return DOTween.To(delegate (float value) { lineRend.widthMultiplier = value; },
                lineRend.widthMultiplier, to, .1f);
        }

        private void MoveLineRend() {
            if (!lineRendVisible) {
                ShowLineRend();
            }

            CreateBezierPoints();

            bezierLineRend.MovePoints(bezierPoints.ToList());
        }

        private void CreateBezierPoints() {
            bezierPoints[0] = (linker.DisplayLineRendFromPoint);

            bezierPoints[1] = bezierPoints[2] = linker.DisplayLineRendFromPoint +
                Vector3.down * Mathf.Abs(linker.DisplayLineRendToPoint.y - linker.DisplayLineRendFromPoint.y)
                * .25f;

            bezierPoints[3] = bezierPoints[4] = linker.DisplayLineRendToPoint +
                Vector3.up * Mathf.Abs(-linker.DisplayLineRendToPoint.y + linker.DisplayLineRendFromPoint.y)
                * .25f;

            bezierPoints[5] = (linker.DisplayLineRendToPoint);
        }
    }
}