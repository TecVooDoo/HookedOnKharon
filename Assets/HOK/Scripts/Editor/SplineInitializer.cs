using Dreamteck.Splines;
using UnityEditor;
using UnityEngine;

namespace HOK.Editor
{
    /// <summary>
    /// Editor utility to initialize splines with predefined points.
    /// </summary>
    public static class SplineInitializer
    {
        [MenuItem("HOK/Initialize River Spline")]
        public static void InitializeSelectedRiverSpline()
        {
            GameObject selected = Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("[SplineInitializer] No GameObject selected.");
                return;
            }
            
            SplineComputer spline = selected.GetComponent<SplineComputer>();
            if (spline == null)
            {
                Debug.LogWarning("[SplineInitializer] Selected object has no SplineComputer component.");
                return;
            }
            
            InitializeRiverSpline(spline);
            Debug.Log("[SplineInitializer] River spline initialized with default points.");
        }
        
        /// <summary>
        /// Initializes a spline with a simple river path along the Z axis.
        /// </summary>
        public static void InitializeRiverSpline(SplineComputer spline)
        {
            // Create points for a river running along the Z axis
            // Y = 0.3 to keep raft slightly above water surface
            SplinePoint[] points = new SplinePoint[5];
            
            // Start of river (south end)
            points[0] = CreatePoint(new Vector3(0f, 0.3f, -18f), Vector3.forward);
            
            // Slight curve left
            points[1] = CreatePoint(new Vector3(-2f, 0.3f, -8f), Vector3.forward);
            
            // Center - near fishing spot
            points[2] = CreatePoint(new Vector3(0f, 0.3f, 0f), Vector3.forward);
            
            // Slight curve right
            points[3] = CreatePoint(new Vector3(2f, 0.3f, 8f), Vector3.forward);
            
            // End of river (north end, near dock)
            points[4] = CreatePoint(new Vector3(-4f, 0.3f, 16f), Vector3.forward);
            
            spline.SetPoints(points, SplineComputer.Space.World);
            spline.type = Spline.Type.BSpline;
            
            EditorUtility.SetDirty(spline);
        }
        
        private static SplinePoint CreatePoint(Vector3 position, Vector3 normal)
        {
            SplinePoint point = new SplinePoint();
            point.position = position;
            point.normal = normal;
            point.size = 1f;
            point.color = Color.white;
            point.type = SplinePoint.Type.SmoothMirrored;
            return point;
        }
    }
}
