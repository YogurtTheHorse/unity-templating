using System.Collections.Generic;
using System.Linq;

namespace YogurtTheHorse.Unity.Templating.Editor.Progression
{
    public class ParallelProgress : IProgress
    {
        private readonly string _name;
        private readonly IProgress[] _progresses;

        public string State => $"{_name} {_progresses.Count(p => p.IsCompleted)}/{_progresses.Length}"; 
        public float Progress => _progresses.Average(p => p.Progress);

        public ParallelProgress(string name, IEnumerable<IProgress> progresses)
        {
            _name = name;
            _progresses = progresses.ToArray();
        }
    }
}