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

        [MenuItem("HOK/Initialize Merchant Branch Spline")]
        public static void InitializeSelectedMerchantBranchSpline()
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

            InitializeMerchantBranchSpline(spline);
            Debug.Log("[SplineInitializer] Merchant branch spline initialized.");
        }

        [MenuItem("HOK/Initialize River Spline (By Name)")]
        public static void InitializeRiverSplineByName()
        {
            GameObject splineObject = GameObject.Find("RiverSpline");
            if (splineObject == null)
            {
                Debug.LogWarning("[SplineInitializer] Could not find 'RiverSpline' in scene.");
                return;
            }

            SplineComputer spline = splineObject.GetComponent<SplineComputer>();
            if (spline == null)
            {
                Debug.LogWarning("[SplineInitializer] RiverSpline has no SplineComputer component.");
                return;
            }

            InitializeRiverSpline(spline);
            Debug.Log("[SplineInitializer] River spline initialized with reversed direction.");
        }

        [MenuItem("HOK/Initialize Merchant Branch Spline (By Name)")]
        public static void InitializeMerchantBranchSplineByName()
        {
            GameObject splineObject = GameObject.Find("MerchantBranchSpline");
            if (splineObject == null)
            {
                Debug.LogWarning("[SplineInitializer] Could not find 'MerchantBranchSpline' in scene.");
                return;
            }

            SplineComputer spline = splineObject.GetComponent<SplineComputer>();
            if (spline == null)
            {
                Debug.LogWarning("[SplineInitializer] MerchantBranchSpline has no SplineComputer component.");
                return;
            }

            InitializeMerchantBranchSpline(spline);
            Debug.Log("[SplineInitializer] Merchant branch spline initialized.");
        }
        
        /// <summary>
        /// Initializes a spline with a simple river path.
        /// CORRECT Unity Coordinate System (camera at -Z looking toward +Z):
        /// +X = screen RIGHT, -X = screen LEFT, +Z = screen TOP, -Z = screen BOTTOM
        /// Spline percent 0 = entrance (+X, right), percent 1 = dock (-X, left)
        /// Left arrow increases percent = toward dock
        /// Right arrow decreases percent = toward entrance
        /// </summary>
        public static void InitializeRiverSpline(SplineComputer spline)
        {
            // River runs along X-axis: entrance at +X (right), dock at -X (left)
            // Y = 0.3 to keep raft slightly above water surface
            // Z = 2 (center of main river channel, between banks at Z=-3 and Z=7)
            SplinePoint[] points = new SplinePoint[5];

            // Point 0: River entrance (right side of screen, +X)
            points[0] = CreatePoint(new Vector3(18f, 0.3f, 2f), Vector3.forward);

            // Point 1: Between entrance and junction
            points[1] = CreatePoint(new Vector3(5f, 0.3f, 2f), Vector3.forward);

            // Point 2: Junction area (where branch forks off, X=-5)
            points[2] = CreatePoint(new Vector3(-5f, 0.3f, 2f), Vector3.forward);

            // Point 3: Past junction, toward dock
            points[3] = CreatePoint(new Vector3(-12f, 0.3f, 2f), Vector3.forward);

            // Point 4: Dock (left side of screen, -X)
            points[4] = CreatePoint(new Vector3(-17f, 0.3f, 2f), Vector3.forward);

            spline.SetPoints(points, SplineComputer.Space.World);
            spline.type = Spline.Type.BSpline;

            EditorUtility.SetDirty(spline);
        }
        
        /// <summary>
        /// Initializes a spline for the merchant branch off Acheron.
        /// CORRECT Unity Coordinate System (camera at -Z looking toward +Z):
        /// +X = screen RIGHT, -X = screen LEFT, +Z = screen TOP, -Z = screen BOTTOM
        /// Branch forks from main river toward upper-left (+Z and -X)
        /// Spline percent 0 = junction (on main river), percent 1 = merchant (dead end at upper-left)
        /// Up/W increases percent = toward merchant
        /// Down/S or auto-return at percent 0 = back to main river
        /// </summary>
        public static void InitializeMerchantBranchSpline(SplineComputer spline)
        {
            // Branch forks from main river at junction (X=-5) toward upper-left (merchant at X=-17, Z=14)
            // Curves through the diagonal channel opening
            SplinePoint[] points = new SplinePoint[4];

            // Point 0: Junction point (connects to main river, start of branch)
            points[0] = CreatePoint(new Vector3(-5f, 0.3f, 5f), Vector3.forward);

            // Point 1: Curving through the diagonal opening toward upper-left
            points[1] = CreatePoint(new Vector3(-10f, 0.3f, 9f), Vector3.forward);

            // Point 2: Continuing toward merchant dock
            points[2] = CreatePoint(new Vector3(-15f, 0.3f, 12f), Vector3.forward);

            // Point 3: Merchant dock (dead end at upper-left, X=-17, Z=14)
            points[3] = CreatePoint(new Vector3(-17f, 0.3f, 14f), Vector3.forward);

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
