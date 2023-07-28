using System;
using System.Collections.Generic;
using System.Text;

namespace Stormancer
{
    internal interface IClusterNetworkConnection
    {
        string ClusterId { get; }

        void Send(ReadOnlySpan<byte> message);


    }

    internal interface IDirectPeerNetworkConnection
    {
        SessionId RemotePeerSessionId { get; }

        void Send(ReadOnlySpan<byte> message);
    }
}
