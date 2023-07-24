using System;
using System.Collections.Generic;
using System.Text;

namespace Stormancer
{
    /// <summary>
    /// Object representing an unique application.
    /// </summary>
    public class ApplicationIdentifier
    {
        /// <summary>
        /// Creates a new instance of <see cref="ApplicationIdentifier"/>
        /// </summary>
        /// <param name="cluster"></param>
        /// <param name="directory"></param>
        /// <param name="applicationName"></param>
        public ApplicationIdentifier(string? cluster, string directory, string applicationName)
        {
            Cluster = cluster;
            Directory = directory;
            ApplicationName = applicationName;
        }

        /// <summary>
        /// Gets an uri to the cluster containing the application.
        /// </summary>
        /// <remarks>
        /// If null, the default cluster the client connects to.
        /// </remarks>
        public string? Cluster { get; }

        /// <summary>
        /// Gets the id of directory containing the application.
        /// </summary>
        public string Directory { get; }

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        public string ApplicationName { get; }

    }
}
