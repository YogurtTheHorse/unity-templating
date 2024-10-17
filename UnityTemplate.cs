using System;
using UnityEngine;

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
        public struct FileToCopy
        {
            public string source;
            public string destination;
        }

        [Header("Files")] public string[] folders = Array.Empty<string>();
        public bool createGitKeepFiles = true;

        [Space] public FileToCopy[] files = Array.Empty<FileToCopy>();
        public bool overwriteFiles = true;

        [Header("Packages")] public ScopedRegistry[] scopedRegistries = Array.Empty<ScopedRegistry>();
        public string[] requiredPackages = Array.Empty<string>();
    }
}