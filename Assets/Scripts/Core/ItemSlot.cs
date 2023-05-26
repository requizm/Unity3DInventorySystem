using Core;
using Demo;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ItemSlot : MonoBehaviour, IBinder
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
}
