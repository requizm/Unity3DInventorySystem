using Demo;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Drop interaction starts from here.
    /// </summary>
    public class DropController : MonoBehaviour, IBinder
    {
        private UIInventoryInteractor uiInventoryInteractor;

        public void Initialize()
        {
            uiInventoryInteractor = ServiceLocator.Current.Get<UIInventoryInteractor>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q) && uiInventoryInteractor.SelectedSlot != null &&
                !uiInventoryInteractor.SelectedSlot.IsEmpty)
            {
                var item = uiInventoryInteractor.SelectedSlot.Items[^1] as IPickable;
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
