namespace Demo
{
    /// <summary>
    /// IBinder is an interface for all binders. <br/>
    /// If this is not a game service but it needs to be initialized, it should implement this interface.
    /// </summary>
    public interface IBinder
    {
        /// <summary>
        /// After all services are initialized, this method is called.
        /// </summary>
        public void Initialize();
    }
}
