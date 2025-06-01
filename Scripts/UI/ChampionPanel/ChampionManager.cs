using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChampionManager : MonoBehaviour
{
    public static ChampionManager Instance {  get; private set; }

    [field:SerializeField] public List<ChampionSO> Champions;
    [SerializeField] private int UpgradeCostPerLevel = 500;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else Instance = this;
    }
    public ChampionSO GetSelectedChampionSO()
    {
        foreach (var championSO in Champions)
        {
            if(SaveLoadHandler.Instance.GetPlayerData().OwnedChampions.Any(championData => championData.isSelected && championData.ID == championSO.ID))
            {
                return championSO;
            }
        }
        return null;
    }
    public bool HasAnyBuyable()
    {
        List<ChampionSO> lockedChampionSOs = new List<ChampionSO>();                

        int currentGem = EconomyManager.Instance.CurrentGem;
        return lockedChampionSOs.Any(cSO => cSO.BuyPrice < currentGem);
    }

    public bool HasAnyUpgradable()
    {
        int currentGem = EconomyManager.Instance.CurrentGem;

        return SaveLoadHandler.Instance.GetPlayerData().OwnedChampions.Any(champion => champion.Level * UpgradeCostPerLevel > currentGem);
    }

    public List<int> GetOwnedChampionIDs()
    {
        List<int> ownedIDs = new List<int>();
        SaveLoadHandler.Instance.GetPlayerData().OwnedChampions.ForEach(champ => ownedIDs.Add(champ.ID));
        return ownedIDs;
    }

    public List<ChampionSO> GetOwnedChampionSOs()
    {
        return Champions.FindAll(championSO => GetOwnedChampionIDs().Contains(championSO.ID));
    }

    public List<ChampionSO> GetLockedChampionSOs()
    {
        return Champions.FindAll(championSO => !GetOwnedChampionIDs().Contains(championSO.ID));
    }

    public bool IsOwnedChampion(ChampionSO championSO) => GetOwnedChampionSOs().Contains(championSO);

    public Champion GetOwnedChampionByID(int id) => SaveLoadHandler.Instance.GetPlayerData().OwnedChampions.Find(c => c.ID == id);
}
