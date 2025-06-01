using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUpgradeController : MonoBehaviour
{
    private UpgradeFeedbacks upgradeFeedbacks;
    [Header("Upgrade Info")]
    [SerializeField] Button UpgradeButton;
    [SerializeField] TextMeshProUGUI upgradeCostText;
    [SerializeField] Slider upgradeSlider;
    [SerializeField] TextMeshProUGUI forgeText;
    [SerializeField] PlayerAttributeUpgrader[] upgraders;
    [SerializeField] RectTransform info;

    [Header("Close")]
    [SerializeField] Button closeButton;
    [SerializeField] GameObject panelParent;
    bool canSlide = false;
    public int totalUpgrades;
    public int upgradeCost;
    private int jumpCountBeforeSelection = 7;
    private float jumpTimeInterval = 0.3f;

    private void Awake()
    {
        upgradeFeedbacks = GetComponent<UpgradeFeedbacks>();
    }
    private void Start()
    {
        UpgradeButton.onClick.AddListener(UpgradeRandomAttribute);
        upgradeSlider.gameObject.SetActive(false);
        closeButton.onClick.AddListener(() =>
        {
            MainMenu.Instance.gameObject.SetActive(true);
            Destroy(panelParent);
        });

        totalUpgrades = SaveLoadHandler.Instance.GetPlayerData().totalUpgrades;
        ResetUpgradeNotification();
        UpdateUpgradeCostText();
        CheckButtonInteraction();
    }

    private void ResetUpgradeNotification()
    {
        NotificationSystem.Instance.ResetNotificationTrigger(NotificationSystem.Upgrade_Trigger);
    }

    private void Update()
    {
        if (canSlide)
        {
            float slidePerSec = 1f /(jumpCountBeforeSelection * jumpTimeInterval);
            upgradeSlider.value += slidePerSec * Time.deltaTime;
        }
    }    
    private void UpgradeRandomAttribute()
    {
        UpgradeButton.interactable = false;
        StartCoroutine(SelectionRoutine());
    }

    private IEnumerator SelectionRoutine()
    {
        upgradeFeedbacks.PlayStartFeedbacks();

        PlayerAttributeUpgrader selectedUpgrader = null;
        List<PlayerAttributeUpgrader> valids = GetValidUpgrades();
        EconomyManager.Instance.SpendCoin(upgradeCost);
        ShowUpgradeSlider();

        for (int i = 0; i < jumpCountBeforeSelection; i++)
        {
            if (selectedUpgrader != null) selectedUpgrader.UnHighLight();

            int randomInt = Random.Range(0, valids.Count);
            selectedUpgrader = valids[randomInt];
            selectedUpgrader.HighLight();
            forgeText.text = "Forging : " + AttributeName.GetAttributeName(selectedUpgrader.AttributeType);
            yield return new WaitForSeconds(jumpTimeInterval);
        }

        yield return new WaitForSeconds(0.5f);

        HandleUpgradeCompletion(selectedUpgrader);

        yield return new WaitForSeconds(1);
        CheckButtonInteraction();
        CloseUpgradeSlider();
    }

    private void CheckButtonInteraction()
    {
        UpgradeButton.interactable = EconomyManager.Instance.HasEnoughCoin(upgradeCost);
    }

    private void HandleUpgradeCompletion(PlayerAttributeUpgrader selectedUpgrader)
    {
        upgradeFeedbacks.PlayFinishFeedbacks(selectedUpgrader);
        selectedUpgrader.UpgradeSelectedAttribute(); //!!        
        selectedUpgrader.UnHighLight();
        totalUpgrades++;

        SaveLoadHandler.Instance.GetPlayerData().totalUpgrades = totalUpgrades;
        SaveLoadHandler.Instance.SaveData();
        UpdateUpgradeCostText();
        
        AchievementManager.Instance.EffectQuestByType(QuestType.MakeSkillUpgrades, 1);
    }
    private void ShowUpgradeSlider()
    {
        upgradeSlider.gameObject.SetActive(true);
        upgradeSlider.value = 0;
        canSlide = true;
    }
    private void CloseUpgradeSlider()
    {
        upgradeSlider.value = 0;
        canSlide = false;
        upgradeSlider.gameObject.SetActive(false);
    }

    private List<PlayerAttributeUpgrader> GetValidUpgrades()
    {
        List<PlayerAttributeUpgrader> validUpgraders = new();
        foreach (var upgrader in upgraders)
        {
            if (upgrader.IsUpgradable)
            { 
                validUpgraders.Add(upgrader);
            }             
        }
        return validUpgraders;
    }
    private void UpdateUpgradeCostText()
    {
        upgradeCost = (int)(Mathf.Pow(totalUpgrades, 1.5f) * 500);
        upgradeCostText.text = NumberFormatter.GetDisplay(upgradeCost);
    }

}

public static class AttributeName
{
    public static string GetAttributeName(AttributeType attributeType)
    {
        switch (attributeType)
        {
            case AttributeType.Damage:
                return "Damage";
            case AttributeType.Health:
                return "Health";                
            case AttributeType.AttackSpeed:
                return "Attack Speed";
            case AttributeType.Range:
                return "Range";
            case AttributeType.LifeSteal:
                return "Life Steal";
            case AttributeType.Defense:
                return "Defense";
            case AttributeType.AbilityDamage:
                return "Ability Damage";
            case AttributeType.MovementSpeed:
                return "Movement Speed";
            case AttributeType.CritChance:
                return "Critical Damage Chance";
        }
        return null;
    }
}
