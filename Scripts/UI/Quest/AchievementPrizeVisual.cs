using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementPrizeVisual : MonoBehaviour
{
    [SerializeField] Image Icon;
    [SerializeField] TextMeshProUGUI valueText;

    [Header("Resource Icons")]
    [SerializeField] Sprite coin;
    [SerializeField] Sprite gem;
    [SerializeField] Sprite silverKey;
    [SerializeField] Sprite goldenKey;
    [SerializeField] Sprite etherealStone;

    public void Show(ResourceData resourceData)
    {
        switch (resourceData.ResourceType)
        {
            case ResourceType.Coin:
                Icon.sprite = coin;
                break;
            case ResourceType.Gem:
                Icon.sprite = gem;
                break;
            case ResourceType.SilverKey:
                Icon.sprite = silverKey;
                break;
            case ResourceType.GoldenKey:
                Icon.sprite = goldenKey;
                break;
            case ResourceType.EtherealStone:
                Icon.sprite = etherealStone;
                break;
        }
        valueText.text = resourceData.Quantity.ToString();
    }
}
