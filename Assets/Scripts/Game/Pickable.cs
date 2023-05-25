using Demo;
using UnityEngine;

namespace Core
{
    public class Pickable : MonoBehaviour, IItem, IPickable, IBinder
    {
        [SerializeField] private ItemAsset itemAsset;
        [SerializeField] private float distanceToPick = 2f;

        [SerializeField] private GameObject visual;

        public ItemAsset ItemAsset => itemAsset;
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
            Debug.Log($"Internal {itemAsset.name} added");
        }

        public void OnRemove()
        {
            visual.transform.position = player.transform.position + player.transform.forward;
            visual.SetActive(true);
            Debug.Log($"Internal {itemAsset.name} removed");
        }

        private void Update()
        {
            if (!IsPicked && Input.GetKeyDown(KeyCode.E))
            {
                var distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance <= distanceToPick)
                {
                    Pick();
                }
            }
        }

        public void Pick()
        {
            IsPicked = true;
            inventoryManager.AddItem(this);
        }

        public void Drop()
        {
            IsPicked = false;
            inventoryManager.RemoveItem(this);
        }
    }
}
