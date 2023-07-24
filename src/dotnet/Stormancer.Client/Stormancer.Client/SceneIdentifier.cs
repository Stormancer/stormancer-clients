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
        /// <param name="scene"></param>
        public SceneIdentifier(string scene)
        {
            SceneId = scene;
        }

        public SceneIdentifier(ApplicationIdentifier application)
        {
            Application = application;
        }

        public string Scene { get; }

        /// <summary>
        /// Gets the <see cref="ApplicationIdentifier"/> object containing the scene.
        /// </summary>
        public ApplicationIdentifier? Application { get; }
    }
}
