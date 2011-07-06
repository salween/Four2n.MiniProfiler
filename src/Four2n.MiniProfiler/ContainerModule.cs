// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContainerModule.cs" company="Daniel Dabrowski - rod.42n.pl">
//   Copyright (c) 2008 Daniel Dabrowski - 42n. All rights reserved.
// </copyright>
// <summary>
//   Defines the ContainerModule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Four2n.Orchard.MiniProfiler
{
    using Autofac;

    using global::Orchard.Environment;

    using Module = Autofac.Module;

    public class ContainerModule : Module
    {
        private IOrchardHost orchardHost;

        public ContainerModule(IOrchardHost orchardHost)
        {
            this.orchardHost = orchardHost;
        }

        protected override void Load(ContainerBuilder moduleBuilder)
        {
            var currentLogger = ((DefaultOrchardHost)this.orchardHost).Logger;
            if (currentLogger is OrchardHostProxyLogger)
            {
                return;
            }

            ((DefaultOrchardHost)this.orchardHost).Logger = new OrchardHostProxyLogger(currentLogger);
        }
    }
}