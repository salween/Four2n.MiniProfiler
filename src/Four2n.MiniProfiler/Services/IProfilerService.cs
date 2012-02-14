using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;

namespace Four2n.Orchard.MiniProfiler.Services {
    public interface IProfilerService : IDependency{

        void StepStart(string key, string message, bool isVerbose = false);
        void StepStop(string key);

    }
}