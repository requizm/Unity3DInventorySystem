using System.Collections.Generic;
using Demo;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core
{
    /// <summary>
    /// Responsible for item slot in UI. It can be used for inventory, equipment, etc.
    /// </summary>
    public class ItemSlot : MonoBehaviour, IBinder, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Image iconImage;
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI stackText;

        /// <summary>
        /// If there is no item in the slot, icon color will be this color.
        /// </summary>
        private Color EmptyIconColor { get; set; }

        /// <summary>
        /// If search is not active or item is in the search result, icon color will be this color.
        /// </summary>
        private Color FilledIconColor { get; set; }

        /// <summary>
        /// If search is active and item is not in the search result, icon color will be this color.
        /// </summary>
        public Color NotFilledIconColor { get; private set; }

        public List<IItem> Items { get; private set; } = new List<IItem>();

        public bool IsEmpty => Items.Count == 0;

        private UIInventoryInteractor uiInventoryInteractor;

        public void Initialize()
        {
            uiInventoryInteractor = ServiceLocator.Current.Get<UIInventoryInteractor>();
        }

        /// <summary>
        /// Updates item slot
        /// </summary>
        /// <param name="item">Count should be greater than 0</param>
        public void SetItem(List<IItem> item)
        {
            if (item.Count == 0)
            {
                Clear();
                return;
            }
            nameText.text = item[0].ItemAsset.AssetName;
            iconImage.sprite = item[0].ItemAsset.Icon;
            stackText.text = item.Count.ToString();
            Items = new List<IItem>(item);

            EmptyIconColor = new Color(iconImage.color.r, iconImage.color.g, iconImage.color.b, 0f);
            FilledIconColor = new Color(iconImage.color.r, iconImage.color.g, iconImage.color.b, 1f);
            NotFilledIconColor = new Color(iconImage.color.r, iconImage.color.g, iconImage.color.b, 0.5f);

            iconImage.color = IsEmpty ? EmptyIconColor : FilledIconColor;
        }

        /// <summary>
        /// Clears item slot
        /// </summary>
        public void Clear()
        {
            nameText.text = "";
            iconImage.sprite = null;
            stackText.text = "";
            Items.Clear();

            EmptyIconColor = new Color(iconImage.color.r, iconImage.color.g, iconImage.color.b, 0f);
            NotFilledIconColor = new Color(iconImage.color.r, iconImage.color.g, iconImage.color.b, 0.5f);

            iconImage.color = EmptyIconColor;
        }

        /// <summary>
        /// Removes item from slot
        /// </summary>
        /// <param name="item"></param>
        public void Remove(IItem item)
        {
            Items.Remove(item);
            stackText.text = Items.Count.ToString();
            if (Items.Count == 0)
            {
                Clear();
                uiInventoryInteractor.SelectedSlot = null;
            }
        }

        /// <summary>
        /// Remove n items from slot <br/>
        /// Starting from the end of the list
        /// </summary>
        /// <param name="number"></param>
        public void Decrease(int number)
        {
            if (number > Items.Count)
            {
                Debug.LogError($"Not enough items in slot");
                return;
            }

            if (number == 0)
            {
                Debug.LogError($"Number of items to decrease cannot be 0");
                return;
            }

            if (number == Items.Count)
            {
                Clear();
                uiInventoryInteractor.SelectedSlot = null;
                return;
            }

            Items.RemoveRange(Items.Count - number, number);
            stackText.text = Items.Count.ToString();
        }

        public void OnClick()
        {
            if (this == uiInventoryInteractor.SelectedSlot || IsEmpty)
            {
                uiInventoryInteractor.SelectedSlot = null;
            }
            else
            {
                uiInventoryInteractor.SelectedSlot = this;
            }
        }

        /// <summary>
        /// Called when slot is selected
        /// </summary>
        public void OnSelect()
        {
            button.targetGraphic.color = Color.green;
        }

        /// <summary>
        /// Called when slot is deselected
        /// </summary>
        public void OnDeselect()
        {
            button.targetGraphic.color = Color.white;
        }

        private bool isDragging;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (IsEmpty)
            {
                return;
            }

            isDragging = true;
            uiInventoryInteractor.DragStartItemSlot = this;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isDragging)
            {
                return;
            }

            var pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);
            var found = false;
            foreach (var result in results)
            {
                var itemSlot = result.gameObject.GetComponent<ItemSlot>();
                if (itemSlot != null)
                {
                    uiInventoryInteractor.DragEndItemSlot = itemSlot;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                uiInventoryInteractor.DragStartItemSlot = null;
            }

            isDragging = false;
        }

        public void OnDragStart()
        {
        }

        public void OnDragEnd(bool success)
        {
            if (success && uiInventoryInteractor.DragStartItemSlot != uiInventoryInteractor.DragEndItemSlot)
            {
                uiInventoryInteractor.SwapTwoItems(uiInventoryInteractor.DragStartItemSlot, uiInventoryInteractor.DragEndItemSlot);
                return;
            }
        }

        public void SetColor(Color color)
        {
            iconImage.color = color;
        }

        public void ResetColor()
        {
            iconImage.color = IsEmpty ? EmptyIconColor : FilledIconColor;
        }
    }
}
