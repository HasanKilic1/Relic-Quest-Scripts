using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    ItemEquipAndUpgrader itemEquipAndUpgrader;
    Button slotButton;
    [SerializeField] private ItemSO itemSO;
    private bool isStatic = false;
    [SerializeField] public ItemType staticItemType;

    [SerializeField] Image backGround;
    [SerializeField] Image frame;
    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemLevel;
    [SerializeField] GameObject equippedPanel;

    [Header("General")] // itemTypeIcon
    [SerializeField] Sprite armorIcon;
    [SerializeField] Sprite helmetIcon;
    [SerializeField] Sprite ringIcon;
    [SerializeField] Sprite necklaceIcon;
    [SerializeField] Sprite glovesIcon;
    [SerializeField] Sprite shoesIcon;

    [Header("BackGround Colors")]
    [SerializeField] Color common;
    [SerializeField] Color uncommon;
    [SerializeField] Color rare;
    [SerializeField] Color epic;
    [SerializeField] Color legendary;
    [Header("Frame Colors")]
    [SerializeField] Color fcommon;
    [SerializeField] Color funcommon;
    [SerializeField] Color frare;
    [SerializeField] Color fepic;
    [SerializeField] Color flegendary;

    private void OnEnable()
    {
        ItemEquipAndUpgrader.OnAnyItemUpgraded += CheckItemLevel;
        ItemEquipAndUpgrader.OnAnyItemEquipped += CheckIsEquipped;
    }  

    private void OnDisable()
    {
        ItemEquipAndUpgrader.OnAnyItemUpgraded -= CheckItemLevel;
        ItemEquipAndUpgrader.OnAnyItemEquipped -= CheckIsEquipped;
    }

    private void Awake()
    {
        slotButton = GetComponent<Button>();
        slotButton.interactable = false;
        equippedPanel.SetActive(false);
    }

    private void Start()
    {        
        itemEquipAndUpgrader = GameObject.FindGameObjectWithTag("Item Upgrader").GetComponent<ItemEquipAndUpgrader>();        
        slotButton.onClick.AddListener(OpenUpgrader);
        CheckIsEquipped(0);
    }

    public void SetupItemSlot(ItemSO itemSO)
    {
        if(isStatic && staticItemType != itemSO.ItemType)
        {
            Debug.LogError("Do not try to insert a static item slot to different item type!");
            return;
        }
        this.itemSO = itemSO;
        itemIcon.gameObject.SetActive(true);
        itemIcon.sprite = itemSO.Icon;
        slotButton.interactable = true;
        UpdateLevelText();
        SetBackGroundColorByRarity(itemSO.Rarity);
    }

    public ItemSO GetItemSO()
    {
        return this.itemSO;
    }

    private void SetBackGroundColorByRarity(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:
                backGround.color = common;
                frame.color = fcommon;
                break;
            case Rarity.Uncommon:
                backGround.color = uncommon;
                frame.color = funcommon;
                break;
            case Rarity.Rare:
                backGround.color = rare;
                frame.color = frare;
                break;
            case Rarity.Epic:
                backGround.color = epic;
                frame.color = fepic;
                break;
            case Rarity.Legendary:
                backGround.color = legendary;
                frame.color = flegendary;
                break;
        }
    }
    private void OpenUpgrader()
    {
        itemEquipAndUpgrader.gameObject.SetActive(true);
        itemEquipAndUpgrader.SetupItemUpgradePanel(itemSO);
    }

    private void CheckItemLevel(int id)
    {
        if(id == this.itemSO.Id)
        {
            UpdateLevelText();
        }        
    }

    private void CheckIsEquipped(int id)
    {  
        if (IsEquippedItem) Equip();
        else CloseEquippedPanel();
    }

    private void UpdateLevelText()
    {
        int level = ItemManager.Instance.GetOwnedItemDataByID(itemSO.Id).level;
        itemLevel.text = level.ToString();
    }

    private void Equip()
    {        
        equippedPanel.gameObject.SetActive(true && !isStatic);
    }

    private void CloseEquippedPanel()
    {
        equippedPanel.gameObject.SetActive(false);
    }

    public void Unequip() => equippedPanel.SetActive(false);
    private bool IsEquippedItem => ItemManager.Instance.GetEquippedItemSOs().Contains(itemSO);
    public void MakeStatic()
    {
        itemIcon.gameObject.SetActive(false);
        isStatic = true;
    }

    public GameObject GetItemLevelText() => itemLevel.gameObject;
}
