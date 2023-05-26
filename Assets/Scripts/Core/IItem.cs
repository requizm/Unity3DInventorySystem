using Core;

/// <summary>
/// IItem is an interface for all items.
/// </summary>
public interface IItem
{
    /// <summary>
    /// ItemAsset is a ScriptableObject that contains all the data for the type of item.
    /// </summary>
    public ItemAsset ItemAsset { get; }
    
    /// <summary>
    /// Id is a unique identifier for the item instance.
    /// </summary>
    public int Id { get; } // InstanceId

    /// <summary>
    /// OnAdd is called when the item is added to the inventory.
    /// </summary>
    public void OnAdd();
    
    /// <summary>
    /// OnRemove is called when the item is removed from the inventory.
    /// </summary>
    public void OnRemove();
}
