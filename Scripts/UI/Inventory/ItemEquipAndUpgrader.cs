using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemEquipAndUpgrader : MonoBehaviour
{
    public static event Action<int> OnAnyItemUpgraded;
    public static event Action<int> OnAnyItemEquipped;
    EquipmentInventory equipmentInventory;
    ItemSO currentItemSO;
    ItemData currentItemData;
    [SerializeField] GameObject ItemInfoFrame;
    [Header("Info")]
    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemLevel;
    [SerializeField] TextMeshProUGUI itemRarity;
    [SerializeField] TextMeshProUGUI etherealStoneText;
 //   [SerializeField] TextMeshProUGUI itemDeclaration;
    [SerializeField] TextMeshProUGUI[] attributeTypeTexts;
    [SerializeField] TextMeshProUGUI[] attributeValueTexts;

    [SerializeField] Button closeButton;
    [SerializeField] Button equipButton;
    [SerializeField] Button upgradeButton;
    [SerializeField] TextMeshProUGUI upgradeCost;

    private void Start()
    {
        equipmentInventory = FindAnyObjectByType<EquipmentInventory>();
        closeButton.onClick.AddListener(Close);
        equipButton.onClick.AddListener(EquipItem);
        upgradeButton.onClick.AddListener(UpgradeItem);
        UpdateUpgradeCostText();
        ItemInfoFrame.SetActive(false);
    }

    public void SetupItemUpgradePanel(ItemSO itemSO)
    {
        ItemInfoFrame.SetActive(true);
        currentItemSO = itemSO;
        itemIcon.sprite = itemSO.Icon;
        itemName.text = itemSO.Name;
        foreach (var itemData in ItemManager.Instance.GetOwnedItems())
        {
            if (itemSO.Id == itemData.id)
            {
                currentItemData = itemData;
                itemLevel.text = itemData.level.ToString();
                break;
            }            
        }
        itemRarity.text = itemSO.Rarity.ToString();
    //    itemDeclaration.text = itemSO.Description;
        UpdateItemLevelText();
        UpdateUpgradeCostText();
        UpdateItemAttributes();
        RefreshUpgradeButtonStatus();
    }

    private void RefreshUpgradeButtonStatus() => upgradeButton.interactable = CanUpgrade();

    private void EquipItem()
    {
        equipmentInventory.EquipItem(currentItemSO);
        OnAnyItemEquipped?.Invoke(currentItemSO.Id);
    }

    private void UpgradeItem()
    {
        if(currentItemData != null && CanUpgrade())
        {
            currentItemData.level++;
            EconomyManager.Instance.SpendCoin(MoneyCost);
            OnAnyItemUpgraded?.Invoke(currentItemSO.Id);

            UpdateItemAttributes();
            UpdateUpgradeCostText();
            UpdateItemLevelText();
            SpendResource();
            RefreshUpgradeButtonStatus();
            SaveLoadHandler.Instance.SaveData();    
        }
    }

    private void SpendResource()
    {
        //Spend Ethereal Stone
        EconomyManager.Instance.TrySpendResource(ResourceType.EtherealStone, EtheralStoneCost, out _);
        EconomyManager.Instance.TrySpendResource(ResourceType.Coin, MoneyCost, out _);
    }

    private void UpdateItemAttributes()
    {
        foreach (var text in attributeTypeTexts)
        {
            text.gameObject.SetActive(false);
        }
        foreach (var text in attributeValueTexts)
        {
            text.gameObject.SetActive(false);
        }


        int attributeCount = currentItemSO.AttributeInfluencers.Length;
        for (int i = 0; i < attributeCount; i++)
        {
            attributeTypeTexts[i].gameObject?.SetActive(true);
            attributeValueTexts[i].gameObject?.SetActive(true);

            attributeTypeTexts[i].text = currentItemSO.AttributeInfluencers[i].attributeType.ToString(); //Attribute type : Attribute effect
            attributeValueTexts[i].text = (currentItemSO.AttributeInfluencers[i].effectOnAttributePerLevel * currentItemData.level).ToString("0.00");

            attributeValueTexts[i].color = currentItemSO.AttributeInfluencers[i].effectOnAttributePerLevel > 0f ? Color.green : Color.red;
        }
    }
    private void UpdateUpgradeCostText()
    {
        if(currentItemSO != null && currentItemData != null)
        {
            upgradeCost.text = NumberFormatter.GetDisplay(MoneyCost);
            etherealStoneText.text = EconomyManager.Instance.CurrentEtherealStone + " / " + (currentItemData.level + 1).ToString();
        }
    }

    private void UpdateItemLevelText()
    {
        itemLevel.text = currentItemData?.level.ToString();
    }

    private void Close()
    {
        ItemInfoFrame.SetActive(false);
    }
    private int MoneyCost => currentItemSO.UpgradeCostMultiplierPerLevel * currentItemData.level;
    private int EtheralStoneCost => currentItemData.level + 1;
    private bool CanUpgrade()
    {
        bool itemLevelValid = ItemManager.Instance.GetOwnedItemDataByID(currentItemData.id).level < 10;
        bool hasEnoughEtherealStone = EconomyManager.Instance.HasEnoughEtherealStone(EtheralStoneCost);
        bool hasEnoughCoin = EconomyManager.Instance.HasEnoughCoin(MoneyCost);
        return hasEnoughCoin && itemLevelValid && hasEnoughEtherealStone;
                             
    }
}
