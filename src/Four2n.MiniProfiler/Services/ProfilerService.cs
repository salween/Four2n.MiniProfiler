using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Concurrent;

namespace Four2n.Orchard.MiniProfiler.Services {

    using MvcMiniProfiler;

    public class ProfilerService : IProfilerService, IDisposable  {

        private readonly MiniProfiler _profiler;
        private readonly ConcurrentDictionary<string, ConcurrentStack<IDisposable>> _steps = new ConcurrentDictionary<string, ConcurrentStack<IDisposable>>();

        public ProfilerService() {
            _profiler = MiniProfiler.Current;
        }

        public void StepStart(string key, string message, bool isVerbose = false)
        {
            var step = _profiler.Step(message, isVerbose?ProfileLevel.Verbose:ProfileLevel.Info);
            var stack = _steps.GetOrAdd(key, (k) => {
                return new ConcurrentStack<IDisposable>();
            });
            stack.Push(step);
        }

        public void StepStop(string key)
        {
            IDisposable step;
            if (_steps[key].TryPop(out step)) {
                step.Dispose();
            }
        }

        public void Dispose() {

            // Dispose any orphaned steps
            foreach (var stack in _steps.Values) {

                foreach (var step in stack) {
                    step.Dispose();
                }

            }

        }
    }
}