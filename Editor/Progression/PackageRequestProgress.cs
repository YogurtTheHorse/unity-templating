using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

namespace YogurtTheHorse.Unity.Templating.Editor.Progression
{
    public class PackageRequestProgress<T> : IProgress where T : Request
    {
        protected readonly string Name;
        protected readonly T Request;

        public virtual string State => $"{Name} " + Request.Status switch
        {
            StatusCode.InProgress => "In progress",
            StatusCode.Failure => "Failed",
            StatusCode.Success => "Success",
            _ => "Unknown"
        };

        public virtual float Progress => Request.IsCompleted ? 1 : 0;

        public PackageRequestProgress(string name, T request)
        {
            Name = name;
            Request = request;
        }
    }
}