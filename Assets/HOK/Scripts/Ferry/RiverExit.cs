using HOK.Core;
using UnityEngine;

namespace HOK.Ferry
{
    /// <summary>
    /// Marks a river exit point (at the river entrance/mouth, NOT the dock).
    /// Place near spline percent 0 where players exit back to the hub.
    /// When the raft enters the trigger, it transitions to the central hub.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class RiverExit : MonoBehaviour
    {
        [Header("River Info")]
        [Tooltip("Which river this exit is in.")]
        [SerializeField] private RiverType riverType;

        [Header("Hub Spawn Settings")]
        [Tooltip("Where to spawn in the hub (X, Z coordinates). Y is ignored.")]
        [SerializeField] private Vector3 hubSpawnPosition;

        [Tooltip("Facing direction when spawning in hub (Y rotation in degrees).")]
        [SerializeField] private float hubSpawnRotationY;

        [Header("Visual Indicator")]
        [Tooltip("Optional GameObject to show when the raft is near.")]
        [SerializeField] private GameObject indicatorObject;

        /// <summary>
        /// The river this exit belongs to.
        /// </summary>
        public RiverType River => riverType;

        private void Awake()
        {
            // Ensure collider is set as trigger
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
            }

            UpdateIndicator(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            // Check if it's the raft (has PlayerMovementController)
            PlayerMovementController movement = other.GetComponent<PlayerMovementController>();
            if (movement == null)
            {
                return;
            }

            // Check if SceneTransitionManager exists
            if (SceneTransitionManager.Instance == null)
            {
                Debug.LogError("[RiverExit] SceneTransitionManager not found. Make sure it exists in the scene or Bootstrap.");
                return;
            }

            // Trigger transition to hub
            Debug.Log($"[RiverExit] Raft reached {riverType} exit. Transitioning to Hub.");
            SceneTransitionManager.Instance.TransitionToHub(riverType, hubSpawnPosition, hubSpawnRotationY);
        }

        private void OnTriggerStay(Collider other)
        {
            // Show indicator while raft is in range
            if (other.GetComponent<PlayerMovementController>() != null)
            {
                UpdateIndicator(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<PlayerMovementController>() != null)
            {
                UpdateIndicator(false);
            }
        }

        /// <summary>
        /// Shows or hides the visual indicator.
        /// </summary>
        public void UpdateIndicator(bool showActive)
        {
            if (indicatorObject != null)
            {
                indicatorObject.SetActive(showActive);
            }
        }

        private void OnDrawGizmos()
        {
            // Draw exit marker in scene view
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 1f);

            // Draw direction arrow (points toward hub spawn direction)
            Gizmos.color = Color.yellow;
            Vector3 direction = Quaternion.Euler(0f, hubSpawnRotationY, 0f) * Vector3.forward;
            Gizmos.DrawRay(transform.position, direction * 2f);

            // Draw line to hub spawn position
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, hubSpawnPosition);
            Gizmos.DrawWireSphere(hubSpawnPosition, 0.5f);
        }
    }
}
