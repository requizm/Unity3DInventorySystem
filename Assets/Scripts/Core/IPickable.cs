using Core;

public interface IPickable
{
    public bool IsPicked { get; set; }

    public void Pick();
    public void Drop();
}
