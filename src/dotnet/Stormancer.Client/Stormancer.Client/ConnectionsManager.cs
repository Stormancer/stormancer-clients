using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormancer
{
    internal class ConnectionsManager
    {
        private readonly ClusterService _clusterService;
        private readonly IEnumerable<INetworkTransport> _networkTransports;
        private readonly ConnectionsRepository _repository;
        private readonly IEnumerable<string> _supportedTransportTypes;

        private readonly object _syncRoot = new object();
        public ConnectionsManager(ClusterService clusterService, IEnumerable<INetworkTransport> networkTransports, ConnectionsRepository repository)
        {
            _clusterService = clusterService;
            _networkTransports = networkTransports;
            _repository = repository;
            _supportedTransportTypes = _networkTransports.Select(t => t.Type);
        }

        
        private async Task<NetworkConnection> GetOrCreateClusterConnection(string clusterId)
        {
            if(_repository.TryGetClusterConnection(clusterId, out var connectionTask))
            {
                return connectionTask;
            }
            var endpoints = await _clusterService.SelectEndpointsForTransports(clusterId, _supportedTransportTypes);

            lock (_syncRoot)
            {

                foreach (var transport in _networkTransports.OrderByDescending(t => t.Priority))
                {
                    transport.TryConnectAsync()
                }
            }
        }
    }

    internal class ConnectionsRepository
    {
        private readonly object _syncRoot = new object();

        private readonly Dictionary<string, Task<NetworkConnection>> _clusterConnections = new Dictionary<string, Task<NetworkConnection>>();

        private readonly Dictionary<SessionId>
        public Task<NetworkConnection> GetOrCreateClusterConnectionAsync(string clusterId, Func<string, IEnumerable<string>> getEndpoints)
        {

        }
        internal bool TryGetClusterConnection(string clusterId, out Task<NetworkConnection> connectionTask)
        {
            throw new NotImplementedException();
        }
    }
}
