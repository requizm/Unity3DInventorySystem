using Core;

public interface IItem
{
    public ItemAsset ItemAsset { get; }
    public int Id { get; }

    public void OnAdd();
    public void OnRemove();
}
