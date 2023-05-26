namespace Demo
{
    /// <summary>
    ///  IGameService is an interface for all game services.
    /// </summary>
    public interface IGameService
    {
        /// <summary>
        ///  Initialize the service. This is called from Bootstrapper.
        /// </summary>
        public void Initialize();
    }
}
