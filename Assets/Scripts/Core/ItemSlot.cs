using System.Collections.Generic;
using Core;
using Demo;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ItemSlot : MonoBehaviour, IBinder, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI stackText;

    public List<IItem> Item { get; private set; } = new List<IItem>();

    public bool IsEmpty => Item.Count == 0;

    private UIInventoryManager uiInventoryManager;

    public void Initialize()
    {
        uiInventoryManager = ServiceLocator.Current.Get<UIInventoryManager>();
    }

    private void Awake()
    {
        Initialize();
    }

    public void SetItem(List<IItem> item)
    {
        nameText.text = item[0].ItemAsset.name;
        iconImage.sprite = item[0].ItemAsset.icon;
        stackText.text = item.Count.ToString();
        Item = item;

        if (IsEmpty)
        {
            iconImage.color = new Color(iconImage.color.r, iconImage.color.g, iconImage.color.b, 0f);
        }
        else
        {
            iconImage.color = new Color(iconImage.color.r, iconImage.color.g, iconImage.color.b, 1f);
        }
    }

    public void Clear()
    {
        nameText.text = "";
        iconImage.sprite = null;
        stackText.text = "";
        Item.Clear();
        
        iconImage.color = new Color(iconImage.color.r, iconImage.color.g, iconImage.color.b, 0f);
    }
    
    public void Decrease(IItem item)
    {
        Item.Remove(item);
        stackText.text = Item.Count.ToString();
        if (Item.Count == 0)
        {
            Clear();
            uiInventoryManager.SelectedSlot = null;
        }
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

    public void OnSelect()
    {
        button.targetGraphic.color = Color.green;
    }

    public void OnDeselect()
    {
        button.targetGraphic.color = Color.white;
    }

    private bool isDragging = false;
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
        }
    }
}
