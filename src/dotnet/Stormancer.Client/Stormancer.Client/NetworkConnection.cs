using System;
using System.Collections.Generic;
using System.Text;

namespace Stormancer
{
    public class NetworkConnection
    {
        internal NetworkConnection(INetworkTransport transport, object networkContext)
        {
            Transport = transport;
            _networkContext = networkContext;
        }
        private object _networkContext;
        public INetworkTransport Transport { get; }


        /// <summary>
        /// Tries sending a binary message through the network connection.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool TrySend(ReadOnlySpan<byte> message)
        {
            return Transport.TrySend(message, _networkContext);
        }
    }
    public class ClusterNetworkConnection : NetworkConnection
    {
        public ClusterNetworkConnection(INetworkTransport transport, string clusterId, object context): base(transport,context)
        {
            ClusterId = clusterId;
        }

        public string ClusterId { get; }

    }

    public class DirectPeerNetworkConnection
    {
        public DirectPeerNetworkConnection(INetworkTransport transport, SessionId peerSessionId, object context)
        {
            Transport = transport;
            RemotePeerSessionId = peerSessionId;
            _context = context;
        }
        public SessionId RemotePeerSessionId { get; }

        bool TrySend(ReadOnlySpan<byte> message)
        {

        }
    }
}
