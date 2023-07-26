namespace Stormancer
{
    /// <summary>
    /// Extends the Stormancer client.
    /// </summary>
    public interface IStormancerClientPlugin
    {
        /// <summary>
        /// Adds dependency registrations to the client scope
        /// </summary>
        /// <param name="builder"></param>
        void OnClientDependenciesRegistration(DependencyBuilder builder);

        /// <summary>
        /// Adds dependency registrations to the scene scope when a new scene reference is created. 
        /// </summary>
        /// <param name="sceneMetadata"></param>
        /// <param name="builder"></param>
        void OnSceneDependenciesRegistration(SceneHostMetadata sceneMetadata, DependencyBuilder builder);
    }
}