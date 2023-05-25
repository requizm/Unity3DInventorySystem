using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
    public class ItemAsset : ScriptableObject
    {
        [SerializeField] public int id;
        [SerializeField] public string name;
        [SerializeField] public GameObject prefab;
        [SerializeField] public Sprite icon;
    }
}
