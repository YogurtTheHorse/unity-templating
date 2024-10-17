using System;

namespace YogurtTheHorse.Unity.Templating.Editor.Progression
{
    public class LambdaProgress : IProgress
    {
        private readonly Func<float> _progressFunc;
        public string State { get; }
        public float Progress => _progressFunc();

        public LambdaProgress(string state, Func<float> progressFunc)
        {
            State = state;
            _progressFunc = progressFunc;
        }
    }
}