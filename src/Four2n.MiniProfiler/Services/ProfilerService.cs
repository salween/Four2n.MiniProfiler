using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Concurrent;

namespace Four2n.Orchard.MiniProfiler.Services {

    using MvcMiniProfiler;

    public class ProfilerService : IProfilerService  {

        private MiniProfiler _profiler;
        protected MiniProfiler profiler {
            get {
                // The event bus starts in a different scope where there's no MiniProfiler.Current, set it now
                if (_profiler == null) {
                    _profiler = MiniProfiler.Current;
                }
                return _profiler;
            }
        }
        private readonly ConcurrentDictionary<string, ConcurrentStack<IDisposable>> _steps = new ConcurrentDictionary<string, ConcurrentStack<IDisposable>>();

        public ProfilerService() {
            _profiler = MiniProfiler.Current;
        }

        public void StepStart(string key, string message, bool isVerbose = false)
        {
            if (profiler == null) return;
            var stack = _steps.GetOrAdd(key, (k) => {
                return new ConcurrentStack<IDisposable>();
            });
            var step = profiler.Step(message, isVerbose ? ProfileLevel.Verbose : ProfileLevel.Info);
            stack.Push(step);
        }

        public void StepStop(string key)
        {
            if (profiler == null) return;
            IDisposable step;
            if (_steps[key].TryPop(out step)) {
                step.Dispose();
            }
        }

        public void StopAll() {

            // Dispose any orphaned steps
            foreach (var stack in _steps.Values) {

                IDisposable step;
                while (stack.TryPop(out step)) {
                    step.Dispose();
                }
            }

        }
    }
}