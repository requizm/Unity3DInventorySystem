using Demo;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Checks if the item can be dropped
    /// </summary>
    public class DropController : MonoBehaviour, IBinder
    {
        private UIInventoryManager uiInventoryManager;

        public void Initialize()
        {
            uiInventoryManager = ServiceLocator.Current.Get<UIInventoryManager>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q) && uiInventoryManager.SelectedSlot != null &&
                !uiInventoryManager.SelectedSlot.IsEmpty)
            {
                var item = uiInventoryManager.SelectedSlot.Items[^1] as IPickable;
                if (item == null)
                {
                    Debug.LogError("Item is not pickable");
                    return;
                }

                item.Drop();
            }
        }
    }
}
