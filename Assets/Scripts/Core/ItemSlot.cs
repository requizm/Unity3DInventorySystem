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
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image iconImage;
    [SerializeField] private Button button;

    public IItem Item { get; private set; }

    public bool IsEmpty => Item == null;

    private UIInventoryManager uiInventoryManager;

    public void Initialize()
    {
        uiInventoryManager = ServiceLocator.Current.Get<UIInventoryManager>();
    }

    private void Awake()
    {
        Initialize();
    }

    public void SetItem(IItem item)
    {
        nameText.text = item.ItemAsset.name;
        iconImage.sprite = item.ItemAsset.icon;
        Item = item;
    }

    public void Clear()
    {
        nameText.text = "";
        iconImage.sprite = null;
        Item = null;
    }

    public void OnClick()
    {
        if (this == uiInventoryManager.SelectedItem || IsEmpty)
        {
            uiInventoryManager.SelectedItem = null;
        }
        else
        {
            uiInventoryManager.SelectedItem = this;
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

    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsEmpty)
        {
            return;
        }
        uiInventoryManager.DragStartItemSlot = this;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
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
