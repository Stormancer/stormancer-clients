using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Stormancer
{
    /// <summary>
    /// A transport used by the Stormancer client to communicate with remote peers and servers.
    /// </summary>
    public interface INetworkTransport
    {
        /// <summary>
        /// Type of the transport.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Transport priority when several transports can be used to connect to a remote peer. The higher the more prioritary.
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Tries sending a message to a remote peer.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="context">Context object interpreted </param>
        /// <returns></returns>
        bool TrySend(ReadOnlySpan<byte> message, object context);
    }
}
