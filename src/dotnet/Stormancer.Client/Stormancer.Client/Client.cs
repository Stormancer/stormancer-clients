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

        public async Task ConnectToFederation(Uri clusterUri)
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
