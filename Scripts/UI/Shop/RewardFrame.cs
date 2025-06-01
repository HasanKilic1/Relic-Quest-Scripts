using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardFrame : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI rewardAmount;
    [SerializeField] Image icon;
    [SerializeField] Image frame;
    [SerializeField] Image bg;
    [SerializeField] Color common;
    [SerializeField] Color uncommon;
    [SerializeField] Color rare;
    [SerializeField] Color epic;
    [SerializeField] Color legendary;
    public void SetRewardAmount(string amount)
    {
        rewardAmount.text = amount;
    }
    public void SetSO(ItemSO ýtemSO)
    {
        icon.sprite = ýtemSO.Icon;
        switch(ýtemSO.Rarity)
        {
            case Rarity.Common:
                frame.color = common;
                bg.color = common;
                break;
            case Rarity.Uncommon:
                frame.color = uncommon;
                bg.color = uncommon;
                break;
            case Rarity.Rare:
                frame.color = rare;
                bg.color = rare;
                break;
            case Rarity.Epic:
                frame.color = epic;
                bg.color = epic;
                break;
            case Rarity.Legendary:
                frame.color = legendary;
                bg.color = legendary;
                break;
        }
    }

}
