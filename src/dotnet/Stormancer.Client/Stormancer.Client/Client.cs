using System;
using System.Threading.Tasks;

namespace Stormancer
{
    /// <summary>
    /// Provides APIs to interact with a Stormancer cluster as a peer.
    /// </summary>
    public class StormancerClient : IDisposable
    {
     
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

        }

        /// <summary>
        /// Sends disconnect signals to all connected clusters and free resources.
        /// </summary>
        public void Dispose()
        {
           
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
        public async Task ConnectToFederation(Uri? clusterUri = default)
        {
            
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
        /// <param name="sceneName"></param>
        /// <param name="configurator"></param>
        /// <returns></returns>
        public Scene GetSceneReference(ApplicationId application, string sceneName, Action<SceneConfigurationBuilder>? configurator = default)
        {
            throw new NotImplementedException();
        }
    }
}
