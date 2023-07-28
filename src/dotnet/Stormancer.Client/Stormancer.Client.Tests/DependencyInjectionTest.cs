using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormancer.Tests
{
    internal class DependencyInjectionTest
    {
        private StormancerClient? _client;
        [SetUp]
        public void Setup()
        {
            _client = StormancerClient.Create(b => b.DefaultCluster(new Uri("https://karma-1.stormancer.com/")).DefaultApplication("karmazoo", "wip"));
        }

        [Test]
        public async Task SingleInstance()
        {
            Debug.Assert(_client != null);
            var singleInstanceDependency = _client.Get<IHttpClientFactory>();
            var singleInstanceDependeny2 = _client.Get<IHttpClientFactory>();

            Assert.IsTrue(singleInstanceDependency == singleInstanceDependeny2);
        }

    }
}
