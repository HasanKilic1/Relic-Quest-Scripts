using MoreMountains.Feedbacks;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementUI : MonoBehaviour
{
    private Achievement quest;    
    [Header("General Visual")]
    [SerializeField] Image Icon;
    [SerializeField] Image prizeIcon;
    [SerializeField] TextMeshProUGUI prizeAmountText;
    [SerializeField] public AchievementSO achievementSO;
    [SerializeField] private Button claimButton;
    [SerializeField] Slider progressBar;
    [SerializeField] TextMeshProUGUI declarationText;
    [SerializeField] TextMeshProUGUI progressText;
    [SerializeField] GameObject[] unclaimedNotifyObjects; // a warning for quest is completed but prize did not taken
    [SerializeField] GameObject[] claimedNotifyObjects;

    [Header("Achievement Icons")]
    [SerializeField] Sprite destroyEnemiesSprite;
    [SerializeField] Sprite completeLevelSprite;
    [SerializeField] Sprite regenerateHealthSprite;
    [SerializeField] Sprite totalTakenDamageSprite;
    [SerializeField] Sprite totalDeathSprite;
    [SerializeField] Sprite spendGoldSprite;
    [SerializeField] Sprite makeSkillUpgradesSprite;

    [Header("Resource Icons")]
    [SerializeField] Sprite gold;
    [SerializeField] Sprite gem;
    [SerializeField] Sprite silverKey;
    [SerializeField] Sprite goldenKey;
    [SerializeField] Sprite etherealStone;

    [SerializeField] private MMF_Player claimFeedbacks;
    private void Start()
    {
        claimButton.onClick.AddListener(TryGivePrize);
    }
    
    public void SetAchievement(AchievementSO achievementSO)
    {
        this.achievementSO = achievementSO;
        this.quest = AchievementManager.Instance.GetQuestByID(achievementSO.id);
        CheckQuestStatus();
        SetupQuestUI(achievementSO);
        ShowPrizeIcon();
    }
    
    private void CheckQuestStatus()
    {        
        progressBar.maxValue = achievementSO.targetedProgress;
        progressBar.value = quest.progress;
        progressText.text = quest.progress.ToString() + " / " + achievementSO.targetedProgress.ToString();

        claimButton.interactable = IsCollectable;
        foreach (var unclaimedNotify in unclaimedNotifyObjects)
        {
            unclaimedNotify.SetActive(IsCollectable);
        }
        foreach (var claimed in claimedNotifyObjects)
        {
            claimed.SetActive(CompletedAndPrizeTaken);
        }
        prizeIcon.gameObject.SetActive(!quest.isCompleted || IsCollectable);
    }
    public bool IsCollectable => quest.isCompleted && !quest.isPrizeTaken;
    private bool CompletedAndPrizeTaken => quest.isCompleted && quest.isPrizeTaken;

    public void TryGivePrize()
    {
        if(IsCollectable)
        {
            DeliverPrizes();
            quest.isPrizeTaken = true;
            CheckQuestStatus();
            SaveLoadHandler.Instance.SaveData();
            HKDebugger.LogSuccess("Achievement prize given : " + achievementSO.name + " / " + achievementSO.Prize.prizeType);
            claimFeedbacks?.PlayFeedbacks();
        }        
    }

    private void DeliverPrizes()
    {
        EconomyManager.Instance.AddResource(achievementSO.Prize.prizeType, achievementSO.Prize.prizeAmount);
    }

    private void ShowPrizeIcon()
    {
        switch (achievementSO.Prize.prizeType)
        {
            case ResourceType.Coin:
                prizeIcon.sprite = gold;
                break;
            case ResourceType.Gem:
                prizeIcon.sprite = gem;
                break;
            case ResourceType.SilverKey:
                prizeIcon.sprite = silverKey;
                break;
            case ResourceType.GoldenKey:
                prizeIcon.sprite = goldenKey;
                break;
            case ResourceType.EtherealStone:
                prizeIcon.sprite = etherealStone;
                break;
        }
        prizeAmountText.text = achievementSO.Prize.prizeAmount.ToString();
    }
    private void SetupQuestUI(AchievementSO questSO)
    {
        switch (questSO.QuestType)
        {
            case QuestType.CompleteLevel:
                Icon.sprite = completeLevelSprite;
                declarationText.text = "Complete " + questSO.targetedProgress.ToString() + " level";
                break;
            case QuestType.DestroyEnemies:
                Icon.sprite = destroyEnemiesSprite;
                declarationText.text = "Destroy " + questSO.targetedProgress.ToString() + " enemies";
                break;
            case QuestType.RegenerateHealth:
                Icon.sprite = regenerateHealthSprite;
                declarationText.text = "Regenerate " + questSO.targetedProgress.ToString() + " health";
                break;
            case QuestType.TakenDamageFromEnemies:
                Icon.sprite = totalTakenDamageSprite;
                declarationText.text = "Take " + questSO.targetedProgress.ToString() + " damage";
                break;
            case QuestType.TotalDeathCount:
                Icon.sprite = totalDeathSprite;
                declarationText.text = "Die " + questSO.targetedProgress.ToString() + " times";
                break;
            case QuestType.SpendCoin:
                Icon.sprite = spendGoldSprite;
                declarationText.text = "Spend " + questSO.targetedProgress.ToString() + " coin";
                break;
            case QuestType.MakeSkillUpgrades:
                Icon.sprite = makeSkillUpgradesSprite;
                declarationText.text = "Make " + questSO.targetedProgress.ToString() + " upgrades";
                break;
        }
    }


}