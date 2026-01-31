using UnityEngine;

namespace HOK.Data
{
    /// <summary>
    /// Defines a fish species with all its properties for fishing gameplay. 
    /// </summary>
    [CreateAssetMenu(fileName = "Fish_", menuName = "HOK/Definitions/Fish Definition")]
    public class FishDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string fishName;
        [SerializeField] [TextArea(2, 4)] private string description;
        [SerializeField] private Sprite icon;
        [SerializeField] private GameObject prefab;

        [Header("Rarity")]
        [SerializeField] private FishRarity rarity;

        [Header("Fishing Behavior")]
        [SerializeField] [Range(0f, 1f)] private float baseCatchDifficulty;
        [SerializeField] [Range(0.5f, 5f)] private float tensionMultiplier = 1f;
        [SerializeField] [Range(0.1f, 5f)] private float escapeStrength = 1f;
        [SerializeField] [Range(0.1f, 1f)] private float hookWindowSize = 0.5f;

        [Header("Bite Timing")]
        [SerializeField] [Range(1f, 30f)] private float minBiteTime = 3f;
        [SerializeField] [Range(1f, 60f)] private float maxBiteTime = 10f;
        [SerializeField] [Range(0.3f, 3f)] private float biteWindowDuration = 1f;

        [Header("Spawn Conditions")]
        [SerializeField] [Range(0f, 1f)] private float minScorchProximity;
        [SerializeField] private string requiredRiver;

        [Header("Rewards")]
        [SerializeField] [Range(1, 1000)] private int baseObols = 10;
        [SerializeField] private bool isCollectible = true;

        // Public accessors
        public string FishName => fishName;
        public string Description => description;
        public Sprite Icon => icon;
        public GameObject Prefab => prefab;
        public FishRarity Rarity => rarity;
        public float BaseCatchDifficulty => baseCatchDifficulty;
        public float TensionMultiplier => tensionMultiplier;
        public float EscapeStrength => escapeStrength;
        public float HookWindowSize => hookWindowSize;
        public float MinBiteTime => minBiteTime;
        public float MaxBiteTime => maxBiteTime;
        public float BiteWindowDuration => biteWindowDuration;
        public float MinScorchProximity => minScorchProximity;
        public string RequiredRiver => requiredRiver;
        public int BaseObols => baseObols;
        public bool IsCollectible => isCollectible;

        /// <summary>
        /// Gets a random bite time within the configured range.
        /// </summary>
        public float GetRandomBiteTime()
        {
            return Random.Range(minBiteTime, maxBiteTime);
        }

        /// <summary>
        /// Checks if this fish can spawn given the current Scorch proximity.
        /// </summary>
        public bool CanSpawnWithScorchProximity(float currentProximity)
        {
            return currentProximity >= minScorchProximity;
        }

        /// <summary>
        /// Checks if this fish can spawn in the given river.
        /// </summary>
        public bool CanSpawnInRiver(string riverName)
        {
            if (string.IsNullOrEmpty(requiredRiver))
            {
                return true;
            }
            return requiredRiver.Equals(riverName, System.StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// Fish rarity tiers affecting spawn rates and rewards.
    /// </summary>
    public enum FishRarity
    {
        Common = 0,
        Uncommon = 1,
        Rare = 2,
        Legendary = 3
    }
}
