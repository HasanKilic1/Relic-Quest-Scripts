using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelicSlot : MonoBehaviour
{
    public RelicSO relicSO;
    public RelicData relicData;
    [Header("UI Visual")]
    [SerializeField] Image icon; 
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;

    [Header("Progress")]
    [SerializeField] TextMeshProUGUI progressText;
    [SerializeField] Image progressImage;

    [Header("Theme Sprites")]
    [SerializeField] Image frame;
    [SerializeField] Sprite nature;
    [SerializeField] Sprite fire;
    [SerializeField] Sprite electricty;
    [SerializeField] Sprite ice;
    [SerializeField] Sprite golden;
    [SerializeField] Image bottomGradient;
    [SerializeField] Color natureColor = Color.white;
    [SerializeField] Color fireColor = Color.white;
    [SerializeField] Color electrictyColor = Color.white;
    [SerializeField] Color iceColor = Color.white;
    [SerializeField] Color goldenColor = Color.white;

    public void Setup(RelicSO relicSO)
    {
        this.relicSO = relicSO;
        HandleVisual(relicSO);
    }

    private void HandleVisual(RelicSO relicSO)
    {
        nameText.text = relicSO.Name;
        levelText.text = CardDataManager.Instance.GetRelicDataByID(relicSO.ID).Level.ToString();

        icon.sprite = relicSO.MainMenuIcon;
        switch (relicSO.RelicTheme)
        {
            case RelicTheme.Nature:
                frame.sprite = nature;
                bottomGradient.color = natureColor;
                break;
            case RelicTheme.Fire:
                frame.sprite = fire;
                bottomGradient.color = fireColor;
                break;
            case RelicTheme.Electricty:
                frame.sprite = electricty;
                bottomGradient.color = electrictyColor;
                break;
            case RelicTheme.Ice:
                frame.sprite = ice;
                bottomGradient.color = iceColor;
                break;
            case RelicTheme.Golden:
                frame.sprite = golden;
                bottomGradient.color = goldenColor;
                break;
        }
        UpdateProgressVisual();
    }

    private void UpdateProgressVisual()
    {
        progressText.text = relicData.totalSelected.ToString() + "/" + "10";
        progressImage.fillAmount = Mathf.Min(relicData.totalSelected / 10, 1);
    }

    private void HighLight()
    {

    }

    private void OnLevelUp(RelicSO relicSO) 
    {
        if(relicSO == this.relicSO)
        {
            relicData.totalSelected -= 10;
            relicData.totalSelected = Mathf.Max(0 , relicData.totalSelected);
        }
    }

    public bool ReadyToFuse()
    {
        return this.relicData.totalSelected >= 10;
    }
}
