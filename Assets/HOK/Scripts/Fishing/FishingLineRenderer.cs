using Obvious.Soap;
using UnityEngine;

namespace HOK.Fishing
{
    /// <summary>
    /// Renders the fishing line from rod tip to hook position.
    /// Updates the HookDepth SOAP variable based on hook Y position for camera tracking.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class FishingLineRenderer : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Transform at the tip of the fishing rod.")]
        [SerializeField] private Transform rodTipTransform;
        [Tooltip("Transform representing the hook/lure position.")]
        [SerializeField] private Transform hookTransform;

        [Header("SOAP Variables")]
        [Tooltip("Current fishing state - line only visible when fishing.")]
        [SerializeField] private IntVariable currentFishingStateVariable;
        [Tooltip("Hook depth variable to update for camera tracking.")]
        [SerializeField] private FloatVariable hookDepthVariable;

        [Header("Water Settings")]
        [Tooltip("Y position of the water surface.")]
        [SerializeField] private float waterSurfaceY = 0.3f;
        [Tooltip("Maximum depth below water for hook depth calculation.")]
        [SerializeField] private float maxDepth = 10f;

        [Header("Line Settings")]
        [Tooltip("Number of points in the line (more = smoother curve).")]
        [SerializeField] private int lineSegments = 20;
        [Tooltip("Amount of sag/curve in the line.")]
        [SerializeField] private float lineSag = 0.5f;
        [Tooltip("Width of the line at the rod end.")]
        [SerializeField] private float lineWidthStart = 0.02f;
        [Tooltip("Width of the line at the hook end.")]
        [SerializeField] private float lineWidthEnd = 0.01f;

        private LineRenderer lineRenderer;
        private Vector3[] linePoints;
        private bool isLineVisible;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            linePoints = new Vector3[lineSegments];

            // Configure line renderer
            lineRenderer.positionCount = lineSegments;
            lineRenderer.startWidth = lineWidthStart;
            lineRenderer.endWidth = lineWidthEnd;
            lineRenderer.useWorldSpace = true;

            // Start hidden
            lineRenderer.enabled = false;
            isLineVisible = false;
        }

        private void Update()
        {
            UpdateLineVisibility();

            if (isLineVisible)
            {
                UpdateLinePositions();
                UpdateHookDepth();
            }
        }

        private void UpdateLineVisibility()
        {
            if (currentFishingStateVariable == null)
            {
                return;
            }

            FishingState state = (FishingState)currentFishingStateVariable.Value;

            // Line is visible during casting, line in water, fish biting, and hooked states
            bool shouldBeVisible = state == FishingState.Casting ||
                                   state == FishingState.LineInWater ||
                                   state == FishingState.FishBiting ||
                                   state == FishingState.Hooked;

            if (shouldBeVisible != isLineVisible)
            {
                isLineVisible = shouldBeVisible;
                lineRenderer.enabled = isLineVisible;
            }
        }

        private void UpdateLinePositions()
        {
            if (rodTipTransform == null || hookTransform == null)
            {
                return;
            }

            Vector3 startPos = rodTipTransform.position;
            Vector3 endPos = hookTransform.position;

            // Calculate line with sag (catenary-like curve)
            for (int i = 0; i < lineSegments; i++)
            {
                float t = (float)i / (lineSegments - 1);

                // Linear interpolation
                Vector3 point = Vector3.Lerp(startPos, endPos, t);

                // Add sag (parabolic curve peaking at the middle)
                float sagAmount = lineSag * 4f * t * (1f - t);
                point.y -= sagAmount;

                linePoints[i] = point;
            }

            lineRenderer.SetPositions(linePoints);
        }

        private void UpdateHookDepth()
        {
            if (hookDepthVariable == null || hookTransform == null)
            {
                return;
            }

            // Calculate depth as normalized value (0 = at surface, 1 = at max depth)
            float hookY = hookTransform.position.y;
            float depthBelowSurface = waterSurfaceY - hookY;

            // Clamp and normalize
            float normalizedDepth = Mathf.Clamp01(depthBelowSurface / maxDepth);
            hookDepthVariable.Value = normalizedDepth;
        }

        /// <summary>
        /// Sets the water surface Y position.
        /// </summary>
        public void SetWaterSurface(float y)
        {
            waterSurfaceY = y;
        }

        /// <summary>
        /// Sets the line sag amount.
        /// </summary>
        public void SetLineSag(float sag)
        {
            lineSag = sag;
        }

        /// <summary>
        /// Forces the line to show/hide (for testing).
        /// </summary>
        public void SetLineVisible(bool visible)
        {
            isLineVisible = visible;
            lineRenderer.enabled = visible;
        }
    }
}
