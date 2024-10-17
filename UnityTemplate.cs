using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace YogurtTheHorse.Unity.Templating
{
    [CreateAssetMenu(fileName = "UnityTemplate", menuName = "Unity/UnityTemplate")]
    public class UnityTemplate : ScriptableObject
    {
        [Serializable]
        public struct ScopedRegistry
        {
            public string name;
            public string url;
            public string[] scopes;
        }

        [Serializable]
        public struct AssetsToCopy
        {
            public enum AssetType
            {
                UnityAsset = 0,
                RelativeFile = 1,
            }

            public AssetType type;
            public string source;
            public string destination;
        }

        [Header("Files")] public string[] folders = Array.Empty<string>();
        public bool createGitKeepFiles = true;

        [Space] public AssetsToCopy[] assetsToCopy = Array.Empty<AssetsToCopy>();

        [Header("Packages")] public ScopedRegistry[] scopedRegistries = Array.Empty<ScopedRegistry>();
        public string[] requiredPackages = Array.Empty<string>();
    }
}