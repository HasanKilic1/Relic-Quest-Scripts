using UnityEngine;
using UnityEngine.UI;

public class PotionUI : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Image background;
    
    [Header("Red Channel")]
    public Sprite RedChannelIcon;
    public Color RedChannelColor;

    [Header("Blue Channel")]
    public Sprite BlueChannelIcon;
    public Color BlueChannelColor;

    [Header("Green Channel")]
    public Sprite GreenChannelIcon;
    public Color GreenChannelColor;

    public void SetChannel(UIChannel channel)
    {
        switch (channel)
        {
            case UIChannel.Red:
                icon.sprite = RedChannelIcon;
                background.color = RedChannelColor;
                break;

            case UIChannel.Blue:
                icon.sprite = BlueChannelIcon;
                background.color = BlueChannelColor;
                break;

            case UIChannel.Green:
                icon.sprite = GreenChannelIcon;
                background.color = GreenChannelColor;
                break;

            default:
                break;
        }
    }
}
public enum UIChannel
{
    Red,
    Blue,
    Green,
}