using System.Diagnostics;

namespace Stormancer.Tests
{
    public class Tests
    {
        private StormancerClient? _client;
        [SetUp]
        public void Setup()
        {
            _client = StormancerClient.Create(b => b.DefaultCluster(new Uri("https://karma-1.stormancer.com/")).DefaultApplication("karmazoo","wip"));
        }

        [Test]
        public async Task ConnectToFederation()
        {
            Debug.Assert(_client != null);
            await _client.ConnectToFederationAsync();
        }

        [Test]
        public async Task Connect()
        {
            Debug.Assert(_client != null);

            using var scene = _client.GetSceneReference("authenticator");

            await scene.WhenConnectedAsync();
            Assert.Pass();
        }
    }
}