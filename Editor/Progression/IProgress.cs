namespace YogurtTheHorse.Unity.Templating.Editor.Progression
{
    public interface IProgress
    {
        string State { get; }
        
        float Progress { get; }

        bool IsCompleted => Progress >= 1;
    }
}