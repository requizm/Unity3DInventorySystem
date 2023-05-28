/// <summary>
///  Base type for all pickable items.
/// </summary>
public interface IPickable
{
    /// <summary>
    /// Picked or not.
    /// </summary>
    public bool IsPicked { get; set; }

    /// <summary>
    /// Called when the player wants to pick up the item.
    /// </summary>
    public void Pick();

    /// <summary>
    /// Called when the player wants to drop the item.
    /// </summary>
    public void Drop();
}
