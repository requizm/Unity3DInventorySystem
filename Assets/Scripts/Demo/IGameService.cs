namespace Demo
{
    /// <summary>
    ///  Similar to Singleton pattern, but it is not a singleton. <br/>
    /// It is used to access game services.
    /// </summary>
    public interface IGameService
    {
        /// <summary>
        ///  Initialize the service. This is called from Bootstrapper.
        /// </summary>
        public void Initialize();
    }
}
