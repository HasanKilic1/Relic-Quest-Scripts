using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpPrizePanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] RewardSlot rewardSlot;
    [SerializeField] Transform slotHolder;
    [SerializeField] Button continueButton;
    [SerializeField] List<RewardSO> rewards;

    private void Start()
    {
        continueButton.onClick.AddListener(GiveRewardsAndClose);
        MainMenu.Instance.CloseEconomyUI();
    }

    public void ShowRewards(List<RewardSO> rewardSOs)
    {
        rewards = new List<RewardSO>(rewardSOs);
        
        StartCoroutine(RewardSpawn(rewardSOs));
        StartCoroutine(LevelTextRefresh());
    } 
    
    private void GiveRewardsAndClose()
    {
        rewards.ForEach(r => r.GiveReward(EconomyManager.Instance));
        MainMenu.Instance.OpenEconomyUI();
        Destroy(gameObject);
    }

    private IEnumerator RewardSpawn(List<RewardSO> rewardSOs)
    {
        foreach (var reward in rewardSOs)
        {
            RewardSlot slot = Instantiate(rewardSlot, slotHolder);
            slot.SetReward(reward);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator LevelTextRefresh()
    {
        levelText.text = (SaveLoadHandler.Instance.GetPlayerData().User.Level - 1).ToString();
        yield return new WaitForSeconds(1);
        levelText.text = SaveLoadHandler.Instance.GetPlayerData().User.Level.ToString();
    }
}
