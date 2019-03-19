namespace Game.UIAddition {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;

    public class BezierCurveLineRenderer : MonoBehaviour {

        private LineRenderer lineRenderer;
        [SerializeField]
        private float vertexPerUnit = 1.5f;
        [SerializeField]
        private int initialVertexPerUnit = 3;
        private int vertexCount = 12;

        private void Awake() {
            lineRenderer = GetComponent<LineRenderer>();
        }

        public void MovePoints(List<Vector3> splinePoints) {
            vertexCount = initialVertexPerUnit +
                (int)(Vector3.Distance(splinePoints.Last(), splinePoints.First()) * vertexPerUnit);

            if (splinePoints == null || splinePoints.Count <= 0) {
                lineRenderer.positionCount = 0;
                lineRenderer.SetPositions(new Vector3[] { Vector3.zero });
                return;
            }

            List<Vector3> pointList = new List<Vector3>();
            for (float ratio = 0; ratio <= 1; ratio += 1.0f / vertexCount) {
                Vector3 bezierPoint = CalculateBezierPoint(ratio, splinePoints);
                pointList.Add(bezierPoint);
            }

            if (pointList.Count <= 1) {
                pointList.Add(splinePoints.First());
                pointList.Add(splinePoints.Last());
            } else {
                pointList.Add(splinePoints.Last());
            }
            lineRenderer.positionCount = pointList.Count;
            lineRenderer.SetPositions(pointList.ToArray());
        }

        private Vector3 CalculateBezierPoint(float ratio, IEnumerable<Vector3> points) {
            if (points.Count() == 1) {
                return points.First();
            }

            LinkedList<Vector3> subPoints = new LinkedList<Vector3>();
            Vector3? lastPoint = null;
            foreach (var point in points) {
                if (!lastPoint.HasValue) {
                    lastPoint = point;
                    continue;
                } else {
                    subPoints.AddLast(Vector3.Lerp(lastPoint.Value, point, ratio));

                    lastPoint = point;
                }
            }

            return CalculateBezierPoint(ratio, subPoints);
        }
    }
}
