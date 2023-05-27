using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// ItemAsset is a ScriptableObject that contains all item data.
    /// </summary>
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
    public class SOItemAsset : ScriptableObject, IItemAsset
    {
        [SerializeField] private int id;
        [SerializeField] private string assetName;
        [SerializeField] private List<string> tags;
        [SerializeField] private int stackLimit;
        [SerializeField] private GameObject prefab;
        [SerializeField] private Sprite icon;

        public int Id => id;
        public string AssetName => assetName;
        public List<string> Tags => tags;
        public int StackLimit => stackLimit;
        public GameObject Prefab => prefab;
        public Sprite Icon => icon;
    }
}
