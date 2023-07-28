using System;
using System.Collections.Generic;
using System.Text;

namespace Stormancer
{
    /// <summary>
    /// Dependencies implementing this interface 
    /// </summary>
    public interface IConnectionStateUpdatesSubscriber
    {
    }

    internal class ConnectionStateUpdatesChannel
    {
        private readonly ConnectionsRepository _repository;
        private readonly Lazy<IEnumerable<IConnectionStateUpdatesSubscriber>> _subscriptions;

        public ConnectionStateUpdatesChannel(ConnectionsRepository repository, Lazy<IEnumerable<IConnectionStateUpdatesSubscriber>> subscriptions)
        {
            _repository = repository;
            _subscriptions = subscriptions;
        }


        public void Notify
    }
}
