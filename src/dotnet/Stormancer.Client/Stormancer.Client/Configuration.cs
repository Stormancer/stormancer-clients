using System;
using System.Collections.Generic;
using System.Text;

namespace Stormancer
{
    /// <summary>
    /// Configuration of the Stormancer client.
    /// </summary>
    public class StormancerClientConfiguration
    {
        internal StormancerClientConfiguration(StormancerClientConfigurationBuilder builder)
        {
            DefaultClusterUri = builder.DefaultClusterUri;
            DefaultApplicationIdentifier = builder.DefaultApplicationIdentifier;
            Plugins = builder.Plugins;
        }

        /// <summary>
        /// Gets the uri of the cluster that should be used by default.
        /// </summary>
        public Uri? DefaultClusterUri { get; }

        /// <summary>
        /// Gets the identifier of the application that should be used by default.
        /// </summary>
        public ApplicationIdentifier? DefaultApplicationIdentifier { get; }

        /// <summary>
        /// Plugins installed in the client.
        /// </summary>
        public List<IStormancerClientPlugin> Plugins { get; }

        /// <summary>
        /// Gets the time span before cluster federation metadata become invalid.
        /// </summary>
        public TimeSpan ClusterFederationRefreshInterval { get; } = TimeSpan.FromSeconds(30);
    }
}
