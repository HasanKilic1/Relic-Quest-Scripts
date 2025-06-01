using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class RewardShower : MonoBehaviour
{
    [SerializeField] Transform rewardHolder;
    [Header("All Rewards")]
    [SerializeField] RewardFrame goldReward;
    [SerializeField] RewardFrame gemReward;
    [SerializeField] RewardFrame energyReward;
    [SerializeField] RewardFrame silverKeyReward;
    [SerializeField] RewardFrame goldenKeyReward;
    [SerializeField] RewardFrame etherealStoneReward;
    [SerializeField] RewardFrame itemReward;
    private List<RewardFrame> givenRewardFrames;
    private void Awake()
    {
        givenRewardFrames = new List<RewardFrame>();
    }
   
    public void ShowReward(RewardType rewardType , int amount , ItemSO itemSO= null)
    {
        switch (rewardType)
        {
            case RewardType.Gold:
                SpawnFrame(goldReward,amount);
                break;
            case RewardType.Gem:
                SpawnFrame(gemReward, amount);
                break;
            case RewardType.Energy:
                SpawnFrame(energyReward, amount);
                break;
            case RewardType.SilverKey:
                SpawnFrame(silverKeyReward, amount);
                break;
            case RewardType.GoldenKey:
                SpawnFrame(goldenKeyReward, amount);
                break;
            case RewardType.EtherealStone:
                SpawnFrame(etherealStoneReward, amount);
                break;
            case RewardType.Item:
                if(itemSO != null)
                {
                    ShowItemReward(itemSO);
                }
                break;
        }
    }
    private void ShowItemReward(ItemSO itemSO)
    {
        RewardFrame itemFrame = Instantiate(itemReward, rewardHolder).GetComponent<RewardFrame>(); ;
        itemFrame.SetSO(itemSO);
        itemFrame.SetRewardAmount(new BigInteger(1).ToString());
        givenRewardFrames.Add(itemFrame);
    }
    private RewardFrame SpawnFrame(RewardFrame frame,int amount)
    {
        if(givenRewardFrames == null) givenRewardFrames = new List<RewardFrame>();
        RewardFrame spawnedFrame = Instantiate(frame, rewardHolder).GetComponent<RewardFrame>();
        spawnedFrame.SetRewardAmount(amount.ToString());
        givenRewardFrames.Add(spawnedFrame);
        return spawnedFrame;
    }

    public void DestroyThis()
    {
        Destroy(gameObject);
    }
}
public enum RewardType
{
    Gold,
    Gem,
    Energy,
    SilverKey,
    GoldenKey,
    EtherealStone,
    Item
}
