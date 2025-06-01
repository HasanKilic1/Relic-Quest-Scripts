using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CostShower : MonoBehaviour
{
    [SerializeField] ResourceType costType;
    [SerializeField] int cost;
    [SerializeField] TextMeshProUGUI costText;
    void Start()
    { 
    }
    private void Update()
    {
        UpdateCostText();
    }
    private void UpdateCostText()
    {
        switch (costType)
        {
            case ResourceType.Coin:
                costText.text = $"({EconomyManager.Instance.CurrentGold} / {cost})";
                break;
            case ResourceType.Gem:
                costText.text = $"({EconomyManager.Instance.CurrentGem} / {cost})";
                break;
         /*   case ResourceType.Purchase:
                costText.text = (EconomyManager.Instance.currentGold / cost).ToString();
                break;*/
            case ResourceType.SilverKey:
                costText.text = $"({EconomyManager.Instance.CurrentSilverKey} / {cost})";
                break;
            case ResourceType.GoldenKey:
                costText.text = $"({EconomyManager.Instance.CurrentGoldenKey} / {cost})";
                break;
            case ResourceType.EtherealStone:
                costText.text = $"({EconomyManager.Instance.CurrentEtherealStone} / {cost})";
                break;
        }
    }
}
