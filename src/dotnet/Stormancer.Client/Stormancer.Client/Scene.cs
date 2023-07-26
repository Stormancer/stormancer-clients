using System;
using System.Threading;
using System.Threading.Tasks;

namespace Stormancer
{
    /// <summary>
    /// Reference to a scene
    /// </summary>
    public class Scene : IDisposable
    {
        /// <summary>
        /// Disposes the scene and triggers disconnection.
        /// </summary>
        public void Dispose()
        {
          
        }

        /// <summary>
        /// Wait for the scene to be connected.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task WhenConnectedAsync(CancellationToken cancellationToken = default)
        {

        }
    }
}