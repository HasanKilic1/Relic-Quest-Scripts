using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RouletteItem : MonoBehaviour
{/*
    RouletteController rouletteController;
    [Header("Logic")]
    [SerializeField] RewardSO rewardSO;
    [SerializeField] float startAngle;    
    public string itemname;
    [SerializeField] RewardData rewardData;
    private int rewardAmount;

    [Header("UI")]
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject focus;

    private void OnEnable()
    {
        RouletteController.OnSpinFinished += CheckReward;
    }

    private void OnDisable()
    {
        RouletteController.OnSpinFinished -= CheckReward;
    }

    private void Start()
    {
        rouletteController = FindFirstObjectByType<RouletteController>();
        InitializeRandomly();
    }
    public void InitializeRandomly()
    {
        rewardData = rewardSO.GetRandomRewardData();        

        rewardAmount = UnityEngine.Random.Range(rewardData.rewardAmountMin * GetMultiplierRelativeToTotalSpin(), 
                                                rewardData.rewardAmountMax * GetMultiplierRelativeToTotalSpin());

        icon.sprite = rewardSO.GetSpriteForReward(rewardData);
        text.text = rewardAmount.ToString();
    }

    private int GetMultiplierRelativeToTotalSpin()
    {
        int pow = Mathf.Max(rouletteController.TotalRouletteSpinned, 1);
        int multiplier = (int)Mathf.Pow(2, pow);
        return multiplier;
    }

    private void CheckReward(int angle)
    {
        if (angle == Convert.ToInt32(-startAngle))
        {
            Focus();
            GiveReward();
        }        
    }
    
    private void Focus()
    {
        focus.SetActive(true);
    }

    public void GiveReward()
    {
        EconomyManager.Instance.AddResource(rewardData.resourceType, rewardAmount);
    }*/
}
    

