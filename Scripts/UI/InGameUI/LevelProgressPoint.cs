using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgressPoint : MonoBehaviour
{
    [SerializeField] Image levelIcon;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Slider slider;
    [SerializeField] float progressValue;

    public void SetupUI(Sprite icon , int level)
    {
        levelIcon.sprite = icon;
        levelText.text = level.ToString();
    }
    public void RefreshLevel(int level)
    {

    }
}
