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
        public async Task Test1()
        {
            _client.GetSceneReference("authenticator");

            Assert.Pass();
        }
    }
}