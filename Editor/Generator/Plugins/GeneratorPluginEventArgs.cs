﻿using System;
using BoDi;
using UnityFlow.Generator.Configuration;
using UnityFlow.General.Plugins;

namespace UnityFlow.Generator.Plugins
{
    public class RegisterDependenciesEventArgs : ObjectContainerEventArgs
    {
        public RegisterDependenciesEventArgs(ObjectContainer objectContainer) : base(objectContainer)
        {
        }
    }

    public class CustomizeDependenciesEventArgs : ObjectContainerEventArgs
    {
        public CustomizeDependenciesEventArgs(ObjectContainer objectContainer, SpecFlowProjectConfiguration specFlowProjectConfiguration)
            : base(objectContainer)
        {
            this.SpecFlowProjectConfiguration = specFlowProjectConfiguration;
        }

        public SpecFlowProjectConfiguration SpecFlowProjectConfiguration { get; private set; }
    }

    public class ConfigurationDefaultsEventArgs : EventArgs
    {
        public ConfigurationDefaultsEventArgs(SpecFlowProjectConfiguration specFlowProjectConfiguration)
        {
            this.SpecFlowProjectConfiguration = specFlowProjectConfiguration;
        }

        public SpecFlowProjectConfiguration SpecFlowProjectConfiguration { get; private set; }
    }
}
