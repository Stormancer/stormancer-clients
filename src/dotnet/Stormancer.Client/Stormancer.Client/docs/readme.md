# Stormancer dotnet client library

The Stormancer client library enables programs to connect and interact with applications hosted on Stormancer clusters.

# Usage

   using var client = Stormancer.Client.Create(c=>c.DefaultApplication("http://localhost","account","test"));

   using var scene = client.GetSceneReference("test-scene",b=>b);

   await scene.WhenConnectedAsync();

   var span = new byte[0]().AsSpan();
   scene.Send("route",span);