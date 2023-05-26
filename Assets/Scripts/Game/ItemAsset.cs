using UnityEngine;

namespace Core
{
    /// <summary>
    /// ItemAsset is a ScriptableObject that contains all item data.
    /// </summary>
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
    public class ItemAsset : ScriptableObject
    {
        [SerializeField] public int id;
        [SerializeField] public string name;
        [SerializeField] public int stackLimit;
        [SerializeField] public GameObject prefab;
        [SerializeField] public Sprite icon;
    }
}
