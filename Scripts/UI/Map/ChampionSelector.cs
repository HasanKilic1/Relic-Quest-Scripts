using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChampionSelector : MonoBehaviour
{
    [SerializeField] private Button NextChampionButton;

    [Header("UI")]
    [SerializeField] private Image championTypeImage;
    [SerializeField] private TextMeshProUGUI championNameText;
    [SerializeField] private TextMeshProUGUI championLevelText;
    [Header("Role Sprites")]
    [SerializeField] private Sprite archerIcon;
    [SerializeField] private Sprite wizardIcon;
    [SerializeField] private Sprite assasinIcon;
    [SerializeField] private Sprite warriorIcon;
    private int index;

    [SerializeField] MMF_Player selectFeedbacks;
    private MMF_TMPTextReveal nameReveal;

    private void Start()
    {
        nameReveal = selectFeedbacks.GetFeedbackOfType<MMF_TMPTextReveal>();            
        ShowAlreadySelected();
        NextChampionButton.onClick.AddListener(SelectNextChampion);
    }

    private void ShowAlreadySelected()
    {
        index = ChampionManager.Instance.GetOwnedChampionSOs().IndexOf(ChampionManager.Instance.GetSelectedChampionSO());
        ShowAtIndex(index);
    }

    private void SelectNextChampion()
    {
        index++;
        if (index > ChampionManager.Instance.GetOwnedChampionSOs().Count - 1)
        {
            index = 0;
        }        
        ShowAtIndex(index);
    }
    private void ShowAtIndex(int index)
    {
        ChampionSO championSO = ChampionManager.Instance.GetOwnedChampionSOs()[index];
        RefreshUI(championSO);
        MarkAsSelected(championSO);
    }

    private void MarkAsSelected(ChampionSO championSO)
    {
        foreach (var champ in SaveLoadHandler.Instance.GetPlayerData().OwnedChampions)
        {
            champ.isSelected = false;
        }

        Champion champion = ChampionManager.Instance.GetOwnedChampionByID(championSO.ID);
        champion.isSelected = true;
        SaveLoadHandler.Instance.GetPlayerData().selectedChampionID = championSO.ID;
        SaveLoadHandler.Instance.SaveData();
    }

    private void RefreshUI(ChampionSO championSO)
    {
        nameReveal.NewText = championSO.Name;
        selectFeedbacks.PlayFeedbacks();
        championLevelText.text = ChampionManager.Instance.GetOwnedChampionByID(championSO.ID).Level.ToString() + " Lvl.";
        switch (championSO.Role)
        {
            case ChampionRole.Archer:
                championTypeImage.sprite = archerIcon;
                break;
            case ChampionRole.Wizard:
                championTypeImage.sprite = wizardIcon;
                break;
            case ChampionRole.Assasin:
                championTypeImage.sprite = assasinIcon;
                break;
            case ChampionRole.Warrior:
                championTypeImage.sprite = warriorIcon;
                break;
        }
    }

}
