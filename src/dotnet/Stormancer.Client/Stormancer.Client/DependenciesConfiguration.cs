using System;
using System.Collections.Generic;
using System.Text;

namespace Stormancer
{
    internal static class DependenciesConfiguration
    {
        public static void ConfigureDependencies(DependencyBuilder builder, StormancerClient client,StormancerClientConfiguration configuration)
        {
            builder.Register(client);
            builder.Register(configuration);
            builder.Register(() => new JsonNetSerializer()).As<IJsonSerializer>();
            builder.Register(() => new DefaultHttpClientFactory()).As<IHttpClientFactory>().SingleInstance();
            builder.Register((ctx) => new FederationService(ctx.Resolve<IHttpClientFactory>(), ctx.Resolve<IJsonSerializer>(),ctx.Resolve<StormancerClientConfiguration>())).SingleInstance();
        }
    }
}
