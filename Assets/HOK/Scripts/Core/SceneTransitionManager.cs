using System.Collections;
using HOK.Ferry;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace HOK.Core
{
    /// <summary>
    /// Manages scene transitions between rivers and the hub.
    /// Persists across scene loads and coordinates raft spawning.
    /// </summary>
    public class SceneTransitionManager : MonoBehaviour
    {
        public static SceneTransitionManager Instance { get; private set; }

        [Header("Scene Names")]
        [Tooltip("Name of the central hub scene.")]
        [SerializeField] private string hubSceneName = "Central_Hub";

        [Header("Transition Settings")]
        [Tooltip("Delay before loading new scene (allows for fade out).")]
        [SerializeField] private float transitionDelay = 0.2f;

        [Header("Input Action Maps")]
        [Tooltip("Name of the action map used in river scenes.")]
        [SerializeField] private string ferryActionMap = "Ferry";

        [Tooltip("Name of the action map used in the hub.")]
        [SerializeField] private string hubActionMap = "Hub";

        // Pending transition data (set before scene load, consumed after)
        private bool hasPendingTransition;
        private RiverType pendingRiverType;
        private float pendingSpawnPercent;
        private Vector3 pendingHubSpawnPosition;
        private float pendingHubSpawnRotationY;
        private bool pendingIsHubDestination;

        private bool isTransitioning;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        /// Transitions from a river scene to the central hub.
        /// </summary>
        /// <param name="exitRiver">Which river we're exiting from.</param>
        /// <param name="spawnPosition">Where to spawn in the hub (near the river entrance).</param>
        /// <param name="spawnRotationY">Facing direction in the hub.</param>
        public void TransitionToHub(RiverType exitRiver, Vector3 spawnPosition, float spawnRotationY)
        {
            if (isTransitioning)
            {
                Debug.LogWarning("[SceneTransitionManager] Transition already in progress.");
                return;
            }

            Debug.Log($"[SceneTransitionManager] Transitioning to Hub from {exitRiver}");

            hasPendingTransition = true;
            pendingIsHubDestination = true;
            pendingRiverType = exitRiver;
            pendingHubSpawnPosition = spawnPosition;
            pendingHubSpawnRotationY = spawnRotationY;

            StartCoroutine(LoadSceneCoroutine(hubSceneName));
        }

        /// <summary>
        /// Transitions from the hub to a river scene.
        /// </summary>
        /// <param name="targetRiver">Which river to enter.</param>
        /// <param name="sceneName">The scene name to load.</param>
        /// <param name="spawnPercent">Where on the river spline to spawn (0-1).</param>
        public void TransitionToRiver(RiverType targetRiver, string sceneName, float spawnPercent)
        {
            if (isTransitioning)
            {
                Debug.LogWarning("[SceneTransitionManager] Transition already in progress.");
                return;
            }

            Debug.Log($"[SceneTransitionManager] Transitioning to {targetRiver} at {spawnPercent:F2}");

            hasPendingTransition = true;
            pendingIsHubDestination = false;
            pendingRiverType = targetRiver;
            pendingSpawnPercent = spawnPercent;

            StartCoroutine(LoadSceneCoroutine(sceneName));
        }

        private IEnumerator LoadSceneCoroutine(string sceneName)
        {
            isTransitioning = true;

            // Brief delay for potential fade effect
            if (transitionDelay > 0f)
            {
                yield return new WaitForSeconds(transitionDelay);
            }

            // Load the scene
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            if (asyncLoad == null)
            {
                Debug.LogError($"[SceneTransitionManager] Failed to load scene: {sceneName}");
                isTransitioning = false;
                hasPendingTransition = false;
                yield break;
            }

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            // Scene loaded - OnSceneLoaded callback will handle setup
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            isTransitioning = false;

            // Switch input action map based on scene type
            SwitchInputActionMap(scene.name);

            // Apply pending transition data if any
            if (hasPendingTransition)
            {
                ApplyPendingTransition();
                hasPendingTransition = false;
            }
        }

        private void SwitchInputActionMap(string sceneName)
        {
            PlayerInput playerInput = FindAnyObjectByType<PlayerInput>();
            if (playerInput == null)
            {
                Debug.LogWarning("[SceneTransitionManager] No PlayerInput found in scene.");
                return;
            }

            // Determine which action map to use
            string targetMap = sceneName == hubSceneName ? hubActionMap : ferryActionMap;

            if (playerInput.currentActionMap?.name != targetMap)
            {
                playerInput.SwitchCurrentActionMap(targetMap);
                Debug.Log($"[SceneTransitionManager] Switched to action map: {targetMap}");
            }
        }

        private void ApplyPendingTransition()
        {
            if (pendingIsHubDestination)
            {
                ApplyHubSpawn();
            }
            else
            {
                ApplyRiverSpawn();
            }
        }

        private void ApplyHubSpawn()
        {
            // Find the raft with PlayerMovementController
            PlayerMovementController movement = FindAnyObjectByType<PlayerMovementController>();
            if (movement == null)
            {
                Debug.LogError("[SceneTransitionManager] No PlayerMovementController found in Hub scene.");
                return;
            }

            // Switch to free movement mode and set position
            movement.SetFreeMode();
            movement.SetPosition(pendingHubSpawnPosition);
            movement.transform.rotation = Quaternion.Euler(0f, pendingHubSpawnRotationY, 0f);

            Debug.Log($"[SceneTransitionManager] Spawned raft in Hub at {pendingHubSpawnPosition}, facing {pendingHubSpawnRotationY}");
        }

        private void ApplyRiverSpawn()
        {
            // Find the raft with PlayerMovementController
            PlayerMovementController movement = FindAnyObjectByType<PlayerMovementController>();
            if (movement == null)
            {
                Debug.LogError("[SceneTransitionManager] No PlayerMovementController found in River scene.");
                return;
            }

            // Find the main river spline (prioritize by name or tag)
            Dreamteck.Splines.SplineComputer spline = FindMainRiverSpline();
            if (spline == null)
            {
                Debug.LogError("[SceneTransitionManager] No SplineComputer found in River scene.");
                return;
            }

            // Switch to spline mode with the found spline
            movement.SetSplineMode(spline, pendingSpawnPercent);

            Debug.Log($"[SceneTransitionManager] Spawned raft on river spline '{spline.name}' at {pendingSpawnPercent:F2}");
        }

        /// <summary>
        /// Finds the main river spline, prioritizing by tag "MainRiver" or name containing "River".
        /// </summary>
        private Dreamteck.Splines.SplineComputer FindMainRiverSpline()
        {
            Dreamteck.Splines.SplineComputer[] allSplines = FindObjectsByType<Dreamteck.Splines.SplineComputer>(FindObjectsSortMode.None);
            if (allSplines == null || allSplines.Length == 0) return null;

            // First pass: look for spline with "MainRiver" tag
            for (int i = 0; i < allSplines.Length; i++)
            {
                if (allSplines[i].CompareTag("MainRiver"))
                {
                    return allSplines[i];
                }
            }

            // Second pass: look for spline with "River" in name (case-insensitive)
            for (int i = 0; i < allSplines.Length; i++)
            {
                if (allSplines[i].name.ToLower().Contains("river"))
                {
                    return allSplines[i];
                }
            }

            // Fallback: return first spline found
            return allSplines[0];
        }

        /// <summary>
        /// Gets the hub scene name.
        /// </summary>
        public string HubSceneName => hubSceneName;

        /// <summary>
        /// Whether a transition is currently in progress.
        /// </summary>
        public bool IsTransitioning => isTransitioning;
    }
}
