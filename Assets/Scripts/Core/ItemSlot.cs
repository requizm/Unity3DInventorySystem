using Core;
using Demo;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ItemSlot : MonoBehaviour, IBinder, ISelectable
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image iconImage;
    private Button button;

    public IItem Item { get; private set; }

    public bool IsEmpty => Item == null;

    private UIInventoryManager uiInventoryManager;

    public void Initialize()
    {
        uiInventoryManager = ServiceLocator.Current.Get<UIInventoryManager>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
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

    private void OnClick()
    {
        uiInventoryManager.ClickItem(this);
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
