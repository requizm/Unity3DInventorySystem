namespace Demo
{
    /// <summary>
    /// If object is not a game service but it needs to be access services, it should implement this interface.
    /// </summary>
    public interface IBinder
    {
        /// <summary>
        /// After all services are initialized, this method is called.
        /// </summary>
        public void Initialize();
        public void Cleanup() {}
    }
}
