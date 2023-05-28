using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    /// <summary>
    ///  ItemAsset is a ScriptableObject that contains all item type data.
    /// </summary>
    public interface IItemAsset
    {
        /// <summary>
        ///  Id is a unique identifier for the item type. This is useful for saving/loading.
        /// </summary>
        public int Id { get; }
        
        /// <summary>
        /// AssetName is the name of the item type. We can show this to the player. Maybe we can use this for localization.
        /// </summary>
        public string AssetName { get; }
        
        /// <summary>
        /// Tags are used to categorize items. We can use this for filtering/searching.
        /// </summary>
        public List<string> Tags { get; }
        
        /// <summary>
        /// StackLimit is the maximum number of items that can be stacked in a single ItemSlot.
        /// </summary>
        public int StackLimit { get; }
        
        /// <summary>
        /// Prefab is the GameObject that will be instantiated when the item is instantiated. Currently we're not using this.
        /// </summary>
        public GameObject Prefab { get; }
        
        /// <summary>
        /// Icon is the Sprite that will be shown in the inventory UI.
        /// </summary>
        public Sprite Icon { get; }
    }
}
