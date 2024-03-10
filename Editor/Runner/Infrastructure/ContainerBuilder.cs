using BoDi;
using System;
using System.Reflection;
using UnityFlow.General.Configuration;
using UnityFlow.General.Tracing;
using UnityFlow.General.UnitTestProvider;
using UnityFlow.Runner.Configuration;
using UnityFlow.Runner.UnitTestProvider;

namespace UnityFlow.Infrastructure
{
    public interface IContainerBuilder
    {
        IObjectContainer CreateGlobalContainer(Assembly testAssembly, IRuntimeConfigurationProvider configurationProvider = null);
        IObjectContainer CreateTestThreadContainer(IObjectContainer globalContainer);
        IObjectContainer CreateScenarioContainer(IObjectContainer testThreadContainer, ScenarioInfo scenarioInfo);
        IObjectContainer CreateFeatureContainer(IObjectContainer testThreadContainer, FeatureInfo featureInfo);
    }

    public class ContainerBuilder : IContainerBuilder
    {
        public static IDefaultDependencyProvider DefaultDependencyProvider = new DefaultDependencyProvider();

        private readonly IDefaultDependencyProvider _defaultDependencyProvider;

        public ContainerBuilder(IDefaultDependencyProvider defaultDependencyProvider = null)
        {
            _defaultDependencyProvider = defaultDependencyProvider ?? DefaultDependencyProvider;
        }

        public virtual IObjectContainer CreateGlobalContainer(Assembly testAssembly, IRuntimeConfigurationProvider configurationProvider = null)
        {
            var container = new ObjectContainer();
            container.RegisterInstanceAs<IContainerBuilder>(this);

            RegisterDefaults(container);

            var testAssemblyProvider = container.Resolve<ITestAssemblyProvider>();
            testAssemblyProvider.RegisterTestAssembly(testAssembly);

            if (configurationProvider != null)
                container.RegisterInstanceAs(configurationProvider);
            else
            {
                configurationProvider = container.Resolve<IRuntimeConfigurationProvider>();
            }


            //container.RegisterTypeAs<RuntimePluginEvents, RuntimePluginEvents>(); //NOTE: we need this unnecessary registration, due to a bug in BoDi (does not inherit non-registered objects)
            //var runtimePluginEvents = container.Resolve<RuntimePluginEvents>();

            SpecFlowConfiguration specFlowConfiguration = ConfigurationLoader.GetDefault();
            specFlowConfiguration = configurationProvider.LoadConfiguration(specFlowConfiguration);
            if (specFlowConfiguration.CustomDependencies != null)
            {
                container.RegisterFromConfiguration(specFlowConfiguration.CustomDependencies);
            }

            var unitTestProviderConfiguration = container.Resolve<UnitTestProviderConfiguration>();

            //LoadPlugins(configurationProvider, container, runtimePluginEvents, specFlowConfiguration, unitTestProviderConfiguration, testAssembly);

            //runtimePluginEvents.RaiseConfigurationDefaults(specFlowConfiguration);

            //runtimePluginEvents.RaiseRegisterGlobalDependencies(container);

            container.RegisterInstanceAs(specFlowConfiguration);

            if (unitTestProviderConfiguration != null)
                container.RegisterInstanceAs(container.Resolve<IUnitTestRuntimeProvider>(unitTestProviderConfiguration.UnitTestProvider ?? ConfigDefaults.UnitTestProviderName));

            //runtimePluginEvents.RaiseCustomizeGlobalDependencies(container, specFlowConfiguration);

            container.Resolve<IConfigurationLoader>().TraceConfigSource(container.Resolve<ITraceListener>(), specFlowConfiguration);

            return container;
        }

        public virtual IObjectContainer CreateTestThreadContainer(IObjectContainer globalContainer)
        {
            var testThreadContainer = new ObjectContainer(globalContainer);

            _defaultDependencyProvider.RegisterTestThreadContainerDefaults(testThreadContainer);

            //var runtimePluginEvents = globalContainer.Resolve<RuntimePluginEvents>();
            //runtimePluginEvents.RaiseCustomizeTestThreadDependencies(testThreadContainer);
            testThreadContainer.Resolve<ITestObjectResolver>();
            return testThreadContainer;
        }

        public virtual IObjectContainer CreateScenarioContainer(IObjectContainer testThreadContainer, ScenarioInfo scenarioInfo)
        {
            if (testThreadContainer == null)
                throw new ArgumentNullException(nameof(testThreadContainer));

            var scenarioContainer = new ObjectContainer(testThreadContainer);
            scenarioContainer.RegisterInstanceAs(scenarioInfo);
            _defaultDependencyProvider.RegisterScenarioContainerDefaults(scenarioContainer);

            scenarioContainer.ObjectCreated += obj =>
            {
                if (obj is IContainerDependentObject containerDependentObject)
                {
                    containerDependentObject.SetObjectContainer(scenarioContainer);
                }
            };

            //var runtimePluginEvents = testThreadContainer.Resolve<RuntimePluginEvents>();
            //runtimePluginEvents.RaiseCustomizeScenarioDependencies(scenarioContainer);

            return scenarioContainer;
        }

        public IObjectContainer CreateFeatureContainer(IObjectContainer testThreadContainer, FeatureInfo featureInfo)
        {
            if (testThreadContainer == null)
                throw new ArgumentNullException(nameof(testThreadContainer));

            var featureContainer = new ObjectContainer(testThreadContainer);
            featureContainer.RegisterInstanceAs(featureInfo);

            featureContainer.ObjectCreated += obj =>
            {
                if (obj is IContainerDependentObject containerDependentObject)
                {
                    containerDependentObject.SetObjectContainer(featureContainer);
                }
            };

            //var runtimePluginEvents = testThreadContainer.Resolve<RuntimePluginEvents>();
            //runtimePluginEvents.RaiseCustomizeFeatureDependencies(featureContainer);

            return featureContainer;
        }

        //protected virtual void LoadPlugins(IRuntimeConfigurationProvider configurationProvider, ObjectContainer container, RuntimePluginEvents runtimePluginEvents,
        //    SpecFlowConfiguration specFlowConfiguration, UnitTestProviderConfiguration unitTestProviderConfigration, Assembly testAssembly)
        //{
        //    // initialize plugins that were registered from code
        //    foreach (var runtimePlugin in container.Resolve<IDictionary<string, IRuntimePlugin>>().Values)
        //    {
        //        // these plugins cannot have parameters
        //        runtimePlugin.Initialize(runtimePluginEvents, new RuntimePluginParameters(), unitTestProviderConfigration);
        //    }

        //    // load & initalize parameters from configuration
        //    var pluginLocator = container.Resolve<IRuntimePluginLocator>();
        //    var pluginLoader = container.Resolve<IRuntimePluginLoader>();
        //    var traceListener = container.Resolve<ITraceListener>();
        //    foreach (var pluginPath in pluginLocator.GetAllRuntimePlugins())
        //    {
        //        // Should not log error if TestAssembly does not have a RuntimePlugin attribute
        //        var traceMissingPluginAttribute = !testAssembly.Location.Equals(pluginPath);
        //        LoadPlugin(pluginPath, pluginLoader, runtimePluginEvents, unitTestProviderConfigration, traceListener, traceMissingPluginAttribute);
        //    }
        //}

        //protected virtual void LoadPlugin(
        //    string pluginPath,
        //    IRuntimePluginLoader pluginLoader,
        //    RuntimePluginEvents runtimePluginEvents,
        //    UnitTestProviderConfiguration unitTestProviderConfigration,
        //    ITraceListener traceListener,
        //    bool traceMissingPluginAttribute)
        //{
        //    traceListener.WriteToolOutput($"Loading plugin {pluginPath}");

        //    var plugin = pluginLoader.LoadPlugin(pluginPath, traceListener, traceMissingPluginAttribute);
        //    var runtimePluginParameters = new RuntimePluginParameters();

        //    plugin?.Initialize(runtimePluginEvents, runtimePluginParameters, unitTestProviderConfigration);
        //}

        protected virtual void RegisterDefaults(ObjectContainer container)
        {
            _defaultDependencyProvider.RegisterGlobalContainerDefaults(container);

            container.RegisterTypeAs<UTFRuntimeProvider, IUnitTestRuntimeProvider>("utf");
        }
    }
}