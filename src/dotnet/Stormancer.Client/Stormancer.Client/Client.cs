using System;
using System.Threading.Tasks;

namespace Stormancer
{
    /// <summary>
    /// Provides APIs to interact with a Stormancer cluster as a peer.
    /// </summary>
    public class StormancerClient : IDisposable
    {

        private readonly StormancerClientConfiguration _configuration;

        private readonly DependencyScope _scope;
        /// <summary>
        /// Creates a <see cref="StormancerClient"/> instance
        /// </summary>
        /// <param name="configurator"></param>
        /// <returns></returns>
        public static StormancerClient Create(Action<StormancerClientConfigurationBuilder> configurator)
        {
            var config = new StormancerClientConfigurationBuilder();

            configurator(config);

            return new StormancerClient(config);
        }

        private StormancerClient(StormancerClientConfigurationBuilder builder)
        {
           _configuration = new StormancerClientConfiguration(builder);

            var container = new Container(BuildContainer);
            _scope = container.CreateRootScope();

            foreach(var eventHandlers in _scope.ResolveAll<IClientEventHandler>())
            {
                eventHandlers.OnInitializingClient(this);
            }
        }

        private void BuildContainer(DependencyBuilder dependencyBuilder)
        {
            DependenciesConfiguration.ConfigureDependencies(dependencyBuilder, this,_configuration);

            foreach(var plugin in _configuration.Plugins)
            {
                plugin.OnClientDependenciesRegistration(dependencyBuilder);
            }
        }

        /// <summary>
        /// Sends disconnect signals to all connected clusters and free resources.
        /// </summary>
        public void Dispose()
        {
            foreach (var eventHandlers in _scope.ResolveAll<IClientEventHandler>())
            {
                eventHandlers.OnDisposingClient(this);
            }
            _scope.Dispose();
        }

        /// <summary>
        /// Default Application the Client connects to.
        /// </summary>
        public ApplicationId? DefaultApplication { get; }

        /// <summary>
        /// Connects the client to a cluster federation.
        /// </summary>
        /// <param name="clusterUri"></param>
        /// <returns></returns>
        public async Task<Federation> ConnectToFederationAsync(Uri? clusterUri = default)
        {

            var federationService = _scope.Resolve<FederationService>();
            return await federationService.ConnectToFederationAsync(clusterUri);
        }

        /// <summary>
        /// Gets the metadata of the current federation.
        /// </summary>
        /// <returns></returns>
        public Task<Federation> GetFederationMetadataAsync()
        {
            var federationService = _scope.Resolve<FederationService>();
            return federationService.GetCurrentFederationAsync();
        }

        /// <summary>
        /// Connects to a Stormancer server application.
        /// </summary>
        /// <param name="applicationIdentifier"></param>
        /// <returns></returns>
        /// <remarks>The client must be connected to a federation before calling this method.</remarks>
        public async Task ConnectToServerApplication(ApplicationIdentifier? applicationIdentifier)
        {

        }

        /// <summary>
        /// Connects to a Stormancer server application.
        /// </summary>
     
        /// <returns></returns>
        public async Task ConnectToServerApplication()
        {

        }

        /// <summary>
        /// Connects to a Stormancer server application.
        /// </summary>
        /// <param name="applicationIdentifier"></param>
        /// <returns></returns>
        /// <remarks>The client must be connected to a federation before calling this method.</remarks>
        public async Task ConnectToServerApplication(Uri clusterUri, ApplicationIdentifier applicationIdentifier)
        {

        }


        /// <summary>
        /// Gets a reference to a scene
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="configurator"></param>
        /// <returns></returns>
        public Scene GetSceneReference(string sceneName, Action<SceneConfigurationBuilder>? configurator = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a reference to a scene
        /// </summary>
        /// <param name="application"></param>
        /// <param name="sceneName"></param>
        /// <param name="configurator"></param>
        /// <returns></returns>
        public Scene GetSceneReference(ApplicationId application, string sceneName, Action<SceneConfigurationBuilder>? configurator = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resolves a dependency from the client dependency resolver.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>() where T: class
        {
            return _scope.Resolve<T>();
        }
    }
}
