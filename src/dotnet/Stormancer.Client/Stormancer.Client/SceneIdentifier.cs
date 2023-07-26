using System;
using System.Collections.Generic;
using System.Text;

namespace Stormancer
{
    /// <summary>
    /// Identifies a scene
    /// </summary>
    public class SceneIdentifier
    {
        /// <summary>
        /// Creates an instance of <see cref="SceneIdentifier"/> from
        /// </summary>
        /// <param name="application"></param>
        /// <param name="scene"></param>
        public SceneIdentifier(ApplicationIdentifier application,string scene)
        {
            Application = application;
            Scene = scene;
        }

       
        /// <summary>
        /// Gets the id of the scene in the application.
        /// </summary>
        public string Scene { get; }

        /// <summary>
        /// Gets the <see cref="ApplicationIdentifier"/> object containing the scene.
        /// </summary>
        public ApplicationIdentifier? Application { get; }

        public override string ToString()
        {
            return $"{Application}/{Scene}";
            
        }
    }
}
