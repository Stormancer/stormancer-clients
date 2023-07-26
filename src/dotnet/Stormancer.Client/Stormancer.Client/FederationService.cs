using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormancer
{
    /// <summary>
    /// Cluster in a federation.
    /// </summary>
    public class FederationCluster
    {
        /// <summary>
        /// Gets or sets the id of the cluster.
        /// </summary>
        public string Id { get; set; } = default!;

        /// <summary>
        /// Gets or sets a list of all the HTTP endoints that can be used to send requests to this cluster.
        /// </summary>
        public List<string> Endpoints { get; set; } = new List<string>();

        /// <summary>
        /// Gets et sets tags associated with this cluster.
        /// </summary>
        public IEnumerable<string> Tags { get; set; } = default!;
    }

    /// <summary>
    /// Federation.
    /// </summary>
    public class Federation
    {
        /// <summary>
        /// Gets or sets a <see cref="FederationCluster"/> object representing the cluster in the federation that was called to get federation informations.
        /// </summary>
        public FederationCluster Current { get; set; } = default!; 

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<FederationCluster> Clusters { get; set; } = default!;

        /// <summary>
        /// Get the cluster from its Id
        /// </summary>
        /// <param name="id">the cluster id</param>
        /// <returns>The FederationCluster with the given id</returns>
        public FederationCluster GetCluster(string id)
        {
            if (Current.Id == id)
            {
                return Current;
            }
            else
            {
                foreach (var cluster in Clusters)
                {
                    if (cluster.Id == id)
                    {
                        return cluster;
                    }
                }
            }
            throw new ArgumentOutOfRangeException($"Cluster {id} not found in federation.");
        }

    }


    internal class FederationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IJsonSerializer _jsonSerializer;

        public FederationService(IHttpClientFactory httpClientFactory, IJsonSerializer jsonSerializer)
        {
            _httpClientFactory = httpClientFactory;
            _jsonSerializer = jsonSerializer;
        }
        public Task<Federation> GetFederationAsync(Uri clusterUri)
        {
            var client = _httpClientFactory.CreateClient("cluster");

            var json = client.GetStringAsync(new Uri(clusterUri, "/_federation"));

        }
    }
}
