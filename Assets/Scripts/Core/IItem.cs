namespace Core
{
    /// <summary>
    /// Base type for all items.
    /// </summary>
    public interface IItem
    {
        /// <summary>
        /// Represents the data for a specific type of item.
        /// </summary>
        public IItemAsset ItemAsset { get; }

        /// <summary>
        /// Represents a unique identifier for the item instance. It is useful for saving/loading.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Called when the item is added to the inventory.
        /// </summary>
        public void OnAdd();

        /// <summary>
        /// Called when the item is removed from the inventory.
        /// </summary>
        public void OnRemove();
    }
}
