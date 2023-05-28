using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    /// <summary>
    ///  Base type for all item types.
    /// </summary>
    public interface IItemAsset
    {
        /// <summary>
        /// Represents a unique identifier for the item instance. It is useful for saving/loading.
        /// </summary>
        public int Id { get; }
        
        /// <summary>
        /// Represents the name of the item. It is useful for searching/filtering.
        /// </summary>
        public string AssetName { get; }
        
        /// <summary>
        /// Used to categorize items. It is useful for searching/filtering.
        /// </summary>
        public List<string> Tags { get; }
        
        /// <summary>
        /// Maximum number of items that can be stacked in a single slot.
        /// </summary>
        public int StackLimit { get; }
        
        /// <summary>
        /// Represents the prefab that will be instantiated when the item is used. Currently it isn't used.
        /// </summary>
        public GameObject Prefab { get; }
        
        /// <summary>
        /// Represents the icon of the item. It is useful for UI.
        /// </summary>
        public Sprite Icon { get; }
    }
}
