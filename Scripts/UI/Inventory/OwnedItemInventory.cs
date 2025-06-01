using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OwnedItemInventory : MonoBehaviour
{
    [SerializeField] Button AllItemSorterButton;
    [SerializeField] Button ArmorSorterButton;
    [SerializeField] Button ShoesSorterButton;
    [SerializeField] Button RingAndNecklaceSorterButton;
    [SerializeField] ItemSlot slotPrefab;
    [SerializeField] Transform slotHolder;

    private List<ItemSlot> spawnedSlots;
    [SerializeField] GameObject AllItemsFocus;
    [SerializeField] GameObject ArmorSorterFocus;
    [SerializeField] GameObject ShoesSorterFocus;
    [SerializeField] GameObject RingAndNecklaceSorterFocus;
    GameObject currentFocused;
    private void OnEnable()
    {
        spawnedSlots = new List<ItemSlot>();
        Invoke(nameof(ShowAllItems), 0.1f);
    }
    private void Start()
    {        
        AllItemSorterButton.onClick.AddListener(ShowAllItems);
        ArmorSorterButton.onClick.AddListener(ShowArmors);
        ShoesSorterButton.onClick.AddListener(ShowShoes);
        RingAndNecklaceSorterButton.onClick.AddListener(ShowRingAndNecklaces);
    }
 
    private List<ItemSO> AllOwnedItems()
    {
        return ItemManager.Instance.GetOwnedItemSOs();
    }
  
    private void ShowAllItems()
    {
        var all = AllOwnedItems();
        Debug.Log("all owned item count: " + all.Count);
        SpawnNewItemSlotsSorted(all);
        ShowFocusObject(ItemType.Undefined);
    }
    private void ShowArmors()
    {
        var armorSOs = AllOwnedItems().Where(so => so.ItemType == ItemType.Armor ||
                                                so.ItemType == ItemType.Helmet ||
                                                so.ItemType == ItemType.Gloves).ToList();
        
        SpawnNewItemSlotsSorted(armorSOs);        
        ShowFocusObject(ItemType.Armor);
    }

    private void ShowShoes()
    {
        var shoes = AllOwnedItems().Where(so => so.ItemType == ItemType.Shoes).ToList();
        SpawnNewItemSlotsSorted(shoes);
        ShowFocusObject(ItemType.Shoes);
    }

    private void ShowRingAndNecklaces()
    {
        var rings = AllOwnedItems().Where(so => so.ItemType == ItemType.Ring ||
                                               so.ItemType == ItemType.Necklace).ToList();

        SpawnNewItemSlotsSorted(rings);       
        ShowFocusObject(ItemType.Ring);
    }      
   
    private void SpawnNewItemSlotsSorted(List<ItemSO> itemSOs)
    {
        ClearSpawnedSlots();
        var sorted = itemSOs.OrderByDescending(x => x.UpgradeCostMultiplierPerLevel).ToArray();
        for (int i = 0; i < itemSOs.Count; i++)
        {
            SpawnNewSlotBySO(sorted[i]);
        }
    }
    private void SpawnNewSlotBySO(ItemSO itemSO)
    {
        ItemSlot spawnedSlot = Instantiate(slotPrefab, slotHolder).GetComponent<ItemSlot>();
        spawnedSlot.SetupItemSlot(itemSO);
        spawnedSlots.Add(spawnedSlot);
    }
    private void ClearSpawnedSlots()
    {
        if(spawnedSlots == null)
        {
            spawnedSlots = new List<ItemSlot>();
        }
        
        foreach (var slot in spawnedSlots)
        {
            Destroy(slot.gameObject);
        }
        spawnedSlots.Clear();
    }
    private void ShowFocusObject(ItemType type)
    {
        if (currentFocused != null) currentFocused.gameObject.SetActive(false);

        if (type == ItemType.Undefined)
        {
            AllItemsFocus.SetActive(true);
            currentFocused = AllItemsFocus;
        }
        else if (type == ItemType.Armor || type == ItemType.Helmet || type == ItemType.Gloves)
        {
            ArmorSorterFocus.SetActive(true);
            currentFocused = ArmorSorterFocus;
        }
        else if (type == ItemType.Ring || type == ItemType.Necklace)
        {
            RingAndNecklaceSorterFocus.SetActive(true);
            currentFocused = RingAndNecklaceSorterFocus;
        }
        else if (type == ItemType.Shoes)
        {
            ShoesSorterFocus.SetActive(true);
            currentFocused = ShoesSorterFocus;
        }
    }

    public void UnequipItem(ItemSO itemSO)
    {
        ItemSlot slot = spawnedSlots.Find(slot => slot.GetItemSO() == itemSO);
        if (slot != null) 
        {
            HKDebugger.LogInfo($"Item slot unequipped : {itemSO.Name}");
            slot.Unequip();
        }
    }
}
