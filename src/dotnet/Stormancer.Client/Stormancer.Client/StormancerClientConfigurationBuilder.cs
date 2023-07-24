using System;
using System.Collections.Generic;
using System.Text;

namespace Stormancer
{
    /// <summary>
    /// Provides methods to configure a Stormancer client.
    /// </summary>
    public class StormancerClientConfigurationBuilder
    {
        /// <summary>
        /// Sets the uri of the cluster the 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public StormancerClientConfigurationBuilder DefaultCluster(Uri uri)
        {
            DefaultClusterUri = uri;
            return this;
        }

        /// <summary>
        /// sets the default application the client should connect to on the cluster
        /// </summary>
        /// <param name="cluster"></param>
        /// <param name="directory"></param>
        /// <param name="applicationName"></param>
        /// <returns></returns>
        public StormancerClientConfigurationBuilder DefaultApplication(string? cluster, string directory, string applicationName)
        {
            DefaultApplicationIdentifier = new ApplicationIdentifier(cluster, directory, applicationName);
            return this;
        }

        /// <summary>
        /// sets the default application the client should connect to on the cluster
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="applicationName"></param>
        /// <returns></returns>
        public StormancerClientConfigurationBuilder DefaultApplication(string directory, string applicationName)
        {
            DefaultApplicationIdentifier = new ApplicationIdentifier(null, directory, applicationName);
            return this;
        }


        /// <summary>
        /// Adds a plugin to the client.
        /// </summary>
        /// <param name="plugin"></param>
        /// <returns></returns>
        public StormancerClientConfigurationBuilder AddPlugin(IStormancerClientPlugin plugin)
        {
            Plugins.Add(plugin);
            return this;
        }

        internal ApplicationIdentifier? DefaultApplicationIdentifier { get; set; }

        internal List<IStormancerClientPlugin> Plugins { get; } = new List<IStormancerClientPlugin>();
        internal Uri? DefaultClusterUri { get; set; }
    }
}
