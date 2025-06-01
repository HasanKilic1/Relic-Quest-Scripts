using System.Collections.Generic;
using UnityEngine;

public class CardDataManager : MonoBehaviour
{
    public static CardDataManager Instance{get; private set;}

    [Header("Hold All Cards Here")]
    [SerializeField] public List<RelicSO> RelicSOs;

    private void Awake()
    {
       Instance = this;
    }

    public RelicSO GetRelicByID(int id)
    {
        return RelicSOs.Find(c => c.ID == id);
    }

    public RelicData GetRelicDataByID(int id)
    {
        return SaveLoadHandler.Instance.GetPlayerData().Relics.Find(r => r.Id == id);
    }
}
