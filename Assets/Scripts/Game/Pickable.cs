using Demo;
using UnityEngine;

namespace Core
{
    public class Pickable : MonoBehaviour, IItem, IPickable, IBinder
    {
        [SerializeField] private SOItemAsset soItemAsset;
        [SerializeField] private float distanceToPick = 2f;

        [SerializeField] private GameObject visual;

        public IItemAsset ItemAsset => soItemAsset;
        public int Id => GetInstanceID();

        public bool IsPicked { get; set; } = false;


        private InventoryManager inventoryManager;
        private Player player;

        public void Initialize()
        {
            inventoryManager = ServiceLocator.Current.Get<InventoryManager>();
            player = ServiceLocator.Current.Get<Player>();
        }

        public void OnAdd()
        {
            visual.SetActive(false);
            Debug.Log($"Internal {soItemAsset.AssetName} added");
        }

        public void OnRemove()
        {
            visual.transform.position = player.transform.position + player.transform.forward;
            visual.SetActive(true);
            Debug.Log($"Internal {soItemAsset.AssetName} removed");
        }

        /// <summary>
        /// If player is near and press E, pick item
        /// </summary>
        private void Update()
        {
            if (!IsPicked && Input.GetKeyDown(KeyCode.E))
            {
                var distance = Mathf.Abs(Vector3.Distance(visual.transform.position, player.transform.position));
                if (distance <= distanceToPick)
                {
                    Pick();
                }
            }
        }

        public void Pick()
        {
            if (IsPicked)
            {
                Debug.LogError($"{soItemAsset.AssetName}:{Id} already picked");
                return;
            }

            IsPicked = true;
            inventoryManager.AddItem(this);
        }

        public void Drop()
        {
            if (!IsPicked)
            {
                Debug.LogError($"{soItemAsset.AssetName}:{Id} is not picked");
                return;
            }

            IsPicked = false;
            inventoryManager.RemoveItem(this);
        }
    }
}
