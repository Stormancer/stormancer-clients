using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormancer
{
    /// <summary>
    /// Results of a Get endpoints operation.
    /// </summary>
    public class GetClusterEndpointsResult
    {
        /// <summary>
        /// Contains informations about the transports exposed by a server node.
        /// </summary>
        public class NodeClientTransports
        {
            /// <summary>
            /// Gets the id of the node exposing the transports
            /// </summary>
            public string NodeId { get; set; } = default!;

            /// <summary>
            /// Contains infos about a specific transport endpoint exposed by a node.
            /// </summary>
            public class ClientTransports
            {
                /// <summary>
                /// Gets the name of the transport.
                /// </summary>
                public string Name { get; set; } = default!;

                /// <summary>
                /// Gets the type of the transport
                /// </summary>
                public string Type { get; set; } = default!;

                /// <summary>
                /// Gets a list of public endpoints associated with the transport.
                /// </summary>
                public IEnumerable<string> PublicEndpoints { get; set; } = default!;

                /// <summary>
                /// Gets the weight the client should use when evaluating to choose this transport endpoint.
                /// </summary>
                public float Weight { get; set; } = 1;
            }

            /// <summary>
            /// Gets the list of client transports exposed by the node.
            /// </summary>
            public IEnumerable<ClientTransports> Transports { get; set; } = default!;
        }

        /// <summary>
        /// Gets the list of client transports exposed by the cluster.
        /// </summary>
        public IEnumerable<NodeClientTransports> ClientTransports { get; set; } = default!;
    }
    internal class ClusterService
    {
        private readonly Random _random = new Random();
        private readonly FederationService _federationService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IJsonSerializer _jsonSerializer;

        public ClusterService(FederationService federation, IHttpClientFactory httpClientFactory, IJsonSerializer jsonSerializer)
        {
            _federationService = federation;
            _httpClientFactory = httpClientFactory;
            _jsonSerializer = jsonSerializer;
        }
        private async Task<GetClusterEndpointsResult> GetClusterEndpointsAsync(string clusterId)
        {
            if (_federationService.CurrentFederationUri == null)
            {
                throw new InvalidOperationException("No server endpoint configured. Either set `DefaultClusterUri` in the configuration or call `Client.ConnectToFederationAsync`");
            }

            var federation = await _federationService.GetCurrentFederationAsync();


            var cluster = federation.GetCluster(clusterId);

            if (cluster == null)
            {
                throw new InvalidOperationException($"Cluster '{clusterId} was not found in the current federation '{_federationService.CurrentFederationUri}'.");
            }


            var baseUri = new Uri(cluster.Endpoints[_random.Next(0, cluster.Endpoints.Count)]);

            var client = _httpClientFactory.CreateClient("cluster");

            var payload = await client.GetStringAsync(new Uri(baseUri, "/_cluster/endpoints"));

            return _jsonSerializer.Deserialize<GetClusterEndpointsResult>(payload);
        }


        public async Task<Dictionary<string,IEnumerable<string>>> SelectEndpointsForTransports(string clusterId, IEnumerable<string> transportTypes)
        {
            var endpoints = await GetClusterEndpointsAsync(clusterId);

            var results = new Dictionary<string, IEnumerable<string>>();

            var endpointList = new List<string>();

            var transports = endpoints.ClientTransports.SelectMany(ct => ct.Transports).Where(transport => transportTypes.Contains(transport.Type)).GroupBy(transport=>transport.Type);

            foreach (var group in transports)
            {
                var type = group.Key;
                var filteredTransports = group.ToList();
                double totalWeight = 0;
                foreach (var transport in filteredTransports)
                {
                    totalWeight += transport.Weight;
                }

                var seed = _random.NextDouble();

                double currentWeight = 0;

                foreach(var t in filteredTransports)
                {
                    currentWeight += t.Weight;
                    if(currentWeight/totalWeight > seed)
                    {
                        results[type] = t.PublicEndpoints;
                        break;
                    }
                }

            }

            return results;

        }
    }
}
