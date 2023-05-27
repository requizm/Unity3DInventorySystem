using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public interface IItemAsset
    {
        public int Id { get; }
        public string AssetName { get; }
        public List<string> Tags { get; }
        public int StackLimit { get; }
        public GameObject Prefab { get; }
        public Sprite Icon { get; }
    }
}
