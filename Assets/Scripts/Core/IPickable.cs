/// <summary>
///  IPickable is an interface for all pickable items.
/// </summary>
public interface IPickable
{
    /// <summary>
    /// IsPicked is true if the item is picked up by the player.
    /// </summary>
    public bool IsPicked { get; set; }

    /// <summary>
    /// It be called when the player wants to pick up the item.
    /// </summary>
    public void Pick();

    /// <summary>
    /// It can be called when the player wants to drop the item.
    /// </summary>
    public void Drop();
}
