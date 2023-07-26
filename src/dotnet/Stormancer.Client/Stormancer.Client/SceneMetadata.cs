using System.Collections.Generic;

namespace Stormancer
{
    /// <summary>
    /// Metadata of a scene.
    /// </summary>
    public class SceneHostMetadata
    {
        internal SceneHostMetadata(SceneIdentifier id, string template, IEnumerable<SceneRoute> routes, Dictionary<string, string> metadata)
        {
            Id = id;
            Template = template;
            Routes = routes;
            Metadata = metadata;
        }


        /// <summary>
        /// Gets the Id of the scene.
        /// </summary>
        public SceneIdentifier Id { get; }

        /// <summary>
        /// Gets the id of the scene template.
        /// </summary>
        public string Template { get; }

        /// <summary>
        /// Gets the routes declared by the scene host.
        /// </summary>
        public IEnumerable<SceneRoute> Routes { get; }

        /// <summary>
        /// Gets the scene metadata.
        /// </summary>
        public Dictionary<string,string> Metadata { get; }
    }

    /// <summary>
    /// A route in a scene to which data can be sent.
    /// </summary>
    public class SceneRoute
    {
        /// <summary>
        /// Gets the handle of the route.
        /// </summary>
        public ushort Handle { get; set; }

        /// <summary>
        /// Gets the name of the route
        /// </summary>
        public string Name { get; set; } = default!;

      
    }
}