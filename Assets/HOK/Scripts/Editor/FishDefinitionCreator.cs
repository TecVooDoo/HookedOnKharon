using HOK.Data;
using UnityEditor;
using UnityEngine;

namespace HOK.Editor
{
    /// <summary>
    /// Editor utility to create the 10 MVP fish definitions for Acheron.
    /// Run via menu: HOK/Create Acheron Fish Definitions
    /// </summary>
    public static class FishDefinitionCreator
    {
        private const string FishFolder = "Assets/HOK/Data/Definitions/Fish";

        [MenuItem("HOK/Create Acheron Fish Definitions")]
        public static void CreateAcheronFish()
        {
            // Ensure folders exist
            EnsureFolderExists("Assets/HOK/Data/Definitions");
            EnsureFolderExists(FishFolder);
            EnsureFolderExists(FishFolder + "/Common");
            EnsureFolderExists(FishFolder + "/Uncommon");
            EnsureFolderExists(FishFolder + "/Rare");

            // Create Common fish (6)
            CreateFish("WoePerch", "Common", FishRarity.Common,
                "A dull gray fish that feeds on tears of sorrow. Common throughout the Acheron.",
                difficulty: 0.2f, tension: 1.0f, escape: 0.8f, hookWindow: 0.6f,
                minBite: 3f, maxBite: 8f, biteWindow: 1.2f,
                scorchProx: 0f, obols: 5);

            CreateFish("PainCarp", "Common", FishRarity.Common,
                "A bottom-dweller that thrives in murky waters. Its scales shimmer with faint agony.",
                difficulty: 0.25f, tension: 1.1f, escape: 0.9f, hookWindow: 0.55f,
                minBite: 4f, maxBite: 10f, biteWindow: 1.1f,
                scorchProx: 0f, obols: 6);

            CreateFish("SorrowTrout", "Common", FishRarity.Common,
                "Silver-scaled and melancholic. Often seen swimming in slow, mournful circles.",
                difficulty: 0.3f, tension: 1.0f, escape: 1.0f, hookWindow: 0.5f,
                minBite: 3f, maxBite: 9f, biteWindow: 1.0f,
                scorchProx: 0f, obols: 7);

            CreateFish("LamentBass", "Common", FishRarity.Common,
                "A larger fish whose low rumbles echo through the river. Decent fight when hooked.",
                difficulty: 0.35f, tension: 1.2f, escape: 1.1f, hookWindow: 0.5f,
                minBite: 5f, maxBite: 12f, biteWindow: 1.0f,
                scorchProx: 0f, obols: 8);

            CreateFish("AcheronEel", "Common", FishRarity.Common,
                "A slippery serpentine fish that weaves through the riverbed. Quick but weak.",
                difficulty: 0.2f, tension: 0.8f, escape: 1.2f, hookWindow: 0.45f,
                minBite: 2f, maxBite: 7f, biteWindow: 0.9f,
                scorchProx: 0f, obols: 6);

            CreateFish("GloomGuppy", "Common", FishRarity.Common,
                "Tiny but numerous. These dark little fish travel in schools of despair.",
                difficulty: 0.15f, tension: 0.7f, escape: 0.6f, hookWindow: 0.7f,
                minBite: 2f, maxBite: 5f, biteWindow: 1.3f,
                scorchProx: 0f, obols: 3);

            // Create Uncommon fish (3)
            CreateFish("GriefSalmon", "Uncommon", FishRarity.Uncommon,
                "A powerful swimmer that fights upstream against the current of woe. Prized catch.",
                difficulty: 0.5f, tension: 1.4f, escape: 1.5f, hookWindow: 0.4f,
                minBite: 8f, maxBite: 18f, biteWindow: 0.8f,
                scorchProx: 0f, obols: 25);

            CreateFish("RegretPike", "Uncommon", FishRarity.Uncommon,
                "Long and aggressive. Known to snap at anything that reminds it of past mistakes.",
                difficulty: 0.55f, tension: 1.5f, escape: 1.8f, hookWindow: 0.35f,
                minBite: 10f, maxBite: 20f, biteWindow: 0.7f,
                scorchProx: 0f, obols: 30);

            CreateFish("MourningSturgeon", "Uncommon", FishRarity.Uncommon,
                "An ancient fish that remembers every soul that passed. Heavy and stubborn.",
                difficulty: 0.6f, tension: 1.6f, escape: 1.3f, hookWindow: 0.4f,
                minBite: 12f, maxBite: 25f, biteWindow: 0.9f,
                scorchProx: 0f, obols: 35);

            // Create Rare fish (1) - requires Scorch proximity
            CreateFish("EmberfinPhantom", "Rare", FishRarity.Rare,
                "A ghostly fish that only appears near hellfire. Its fins glow with spectral flame. Scorch can sense it.",
                difficulty: 0.75f, tension: 2.0f, escape: 2.2f, hookWindow: 0.3f,
                minBite: 15f, maxBite: 35f, biteWindow: 0.6f,
                scorchProx: 0.7f, obols: 100);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[FishDefinitionCreator] Created 10 Acheron fish definitions!");
        }

        private static void CreateFish(
            string fishName,
            string subfolder,
            FishRarity rarity,
            string description,
            float difficulty,
            float tension,
            float escape,
            float hookWindow,
            float minBite,
            float maxBite,
            float biteWindow,
            float scorchProx,
            int obols)
        {
            string path = $"{FishFolder}/{subfolder}/Fish_{fishName}.asset";

            // Check if already exists
            FishDefinition existing = AssetDatabase.LoadAssetAtPath<FishDefinition>(path);
            if (existing != null)
            {
                Debug.Log($"[FishDefinitionCreator] {fishName} already exists, skipping.");
                return;
            }

            FishDefinition fish = ScriptableObject.CreateInstance<FishDefinition>();

            // Use SerializedObject to set private fields
            SerializedObject so = new SerializedObject(fish);

            so.FindProperty("fishName").stringValue = fishName;
            so.FindProperty("description").stringValue = description;
            so.FindProperty("rarity").enumValueIndex = (int)rarity;
            so.FindProperty("baseCatchDifficulty").floatValue = difficulty;
            so.FindProperty("tensionMultiplier").floatValue = tension;
            so.FindProperty("escapeStrength").floatValue = escape;
            so.FindProperty("hookWindowSize").floatValue = hookWindow;
            so.FindProperty("minBiteTime").floatValue = minBite;
            so.FindProperty("maxBiteTime").floatValue = maxBite;
            so.FindProperty("biteWindowDuration").floatValue = biteWindow;
            so.FindProperty("minScorchProximity").floatValue = scorchProx;
            so.FindProperty("requiredRiver").stringValue = "Acheron";
            so.FindProperty("baseObols").intValue = obols;
            so.FindProperty("isCollectible").boolValue = true;

            so.ApplyModifiedPropertiesWithoutUndo();

            AssetDatabase.CreateAsset(fish, path);
            Debug.Log($"[FishDefinitionCreator] Created: {path}");
        }

        private static void EnsureFolderExists(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string parent = System.IO.Path.GetDirectoryName(path).Replace("\\", "/");
                string folderName = System.IO.Path.GetFileName(path);
                AssetDatabase.CreateFolder(parent, folderName);
            }
        }
    }
}
