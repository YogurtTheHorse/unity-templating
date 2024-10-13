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

        public string[] folders = Array.Empty<string>();
        public ScopedRegistry[] scopedRegistries = Array.Empty<ScopedRegistry>();
        public string[] requiredPackages = Array.Empty<string>();
    }
}