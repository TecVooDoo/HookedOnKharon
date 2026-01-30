using HOK.Core;
using UnityEngine;

namespace HOK.Ferry
{
    /// <summary>
    /// Marks a river entrance/exit point in the Central Hub.
    /// When the raft enters the trigger, it will transition to the target river scene.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class RiverEntrance : MonoBehaviour
    {
        [Header("Target River")]
        [Tooltip("Name of the scene to load when entering this river.")]
        [SerializeField] private string targetSceneName;

        [Tooltip("Which river this entrance leads to.")]
        [SerializeField] private RiverType riverType;

        [Header("Spawn Settings")]
        [Tooltip("Position on the target spline where the raft will spawn (0-1). 0 = river entrance, 1 = dock.")]
        [SerializeField] [Range(0f, 1f)] private float spawnPercent = 0f;

        [Header("Visual Indicator")]
        [Tooltip("Optional GameObject to show when the raft is near.")]
        [SerializeField] private GameObject indicatorObject;

        [Header("State")]
        [SerializeField] private bool isUnlocked = true;

        /// <summary>
        /// The river type this entrance leads to.
        /// </summary>
        public RiverType River => riverType;

        /// <summary>
        /// Whether this entrance is currently usable.
        /// </summary>
        public bool IsUnlocked
        {
            get => isUnlocked;
            set
            {
                isUnlocked = value;
                UpdateIndicator(false);
            }
        }

        /// <summary>
        /// The scene name to load when using this entrance.
        /// </summary>
        public string TargetSceneName => targetSceneName;

        /// <summary>
        /// Where on the target spline the raft should spawn.
        /// </summary>
        public float SpawnPercent => spawnPercent;

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
            if (!isUnlocked)
            {
                return;
            }

            // Check if it's the raft (has PlayerMovementController)
            if (other.GetComponent<PlayerMovementController>() != null)
            {
                // Check if SceneTransitionManager exists
                if (SceneTransitionManager.Instance == null)
                {
                    Debug.LogError("[RiverEntrance] SceneTransitionManager not found. Make sure it exists in the scene or Bootstrap.");
                    return;
                }

                // Trigger transition to river
                Debug.Log($"[RiverEntrance] Raft entered {riverType} entrance. Loading: {targetSceneName}");
                SceneTransitionManager.Instance.TransitionToRiver(riverType, targetSceneName, spawnPercent);
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
                indicatorObject.SetActive(isUnlocked && showActive);
            }
        }

        private void OnDrawGizmos()
        {
            // Draw entrance marker in scene view
            Gizmos.color = isUnlocked ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, 1f);

            // Draw direction arrow
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, transform.forward * 2f);
        }
    }

    /// <summary>
    /// The five rivers of the Underworld.
    /// </summary>
    public enum RiverType
    {
        Acheron,    // Pain/Woe - Beginner
        Styx,       // Hate - Early
        Lethe,      // Forgetfulness - Mid
        Phlegethon, // Fire - Advanced
        Cocytus     // Lamentation - Endgame
    }
}
