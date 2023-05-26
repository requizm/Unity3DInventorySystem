using System.Collections.Generic;
using Core;
using Demo;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// This class is responsible for item slot in UI. It can be used for inventory, equipment, etc. <br/>
/// It can be used both for single item and stackable items.
/// </summary>
public class ItemSlot : MonoBehaviour, IBinder, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI stackText;

    private Color EmptyIconColor { get; set; }
    private Color FilledIconColor { get; set; }
    public Color NotFilledIconColor { get; private set; }

    public List<IItem> Items { get; private set; } = new List<IItem>();

    public bool IsEmpty => Items.Count == 0;

    private UIInventoryManager uiInventoryManager;

    public void Initialize()
    {
        uiInventoryManager = ServiceLocator.Current.Get<UIInventoryManager>();
    }

    private void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// Updates item slot
    /// </summary>
    /// <param name="item">Count should be greater than 0</param>
    public void SetItem(List<IItem> item)
    {
        nameText.text = item[0].ItemAsset.assetName;
        iconImage.sprite = item[0].ItemAsset.icon;
        stackText.text = item.Count.ToString();
        Items = item;
        
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
            uiInventoryManager.SelectedSlot = null;
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
            uiInventoryManager.SelectedSlot = null;
            return;
        }

        Items.RemoveRange(Items.Count - number, number);
        stackText.text = Items.Count.ToString();
    }

    public void OnClick()
    {
        if (this == uiInventoryManager.SelectedSlot || IsEmpty)
        {
            uiInventoryManager.SelectedSlot = null;
        }
        else
        {
            uiInventoryManager.SelectedSlot = this;
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
        uiInventoryManager.DragStartItemSlot = this;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging)
        {
            return;
        }

        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);
        var found = false;
        foreach (var result in results)
        {
            var itemSlot = result.gameObject.GetComponent<ItemSlot>();
            if (itemSlot != null)
            {
                uiInventoryManager.DragEndItemSlot = itemSlot;
                found = true;
                break;
            }
        }

        if (!found)
        {
            uiInventoryManager.DragStartItemSlot = null;
        }

        isDragging = false;
    }

    public void OnDragStart()
    {
    }

    public void OnDragEnd(bool success)
    {
        if (success && uiInventoryManager.DragStartItemSlot != uiInventoryManager.DragEndItemSlot)
        {
            uiInventoryManager.SwapTwoItems(uiInventoryManager.DragStartItemSlot, uiInventoryManager.DragEndItemSlot);
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
