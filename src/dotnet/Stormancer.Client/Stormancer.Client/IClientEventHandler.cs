using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Stormancer
{
    /// <summary>
    /// Enables a class to participate in the client lifecycle.
    /// </summary>
    public interface IClientEventHandler
    {
        /// <summary>
        /// Event called when the client is being initialized, after dependency registration.
        /// </summary>
        /// <param name="client"></param>
        void OnInitializingClient(StormancerClient client);

        /// <summary>
        /// Event called when the client is being disposed.
        /// </summary>
        /// <param name="client"></param>
        void OnDisposingClient(StormancerClient client);
    }
}
