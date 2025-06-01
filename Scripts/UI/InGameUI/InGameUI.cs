using MoreMountains.Feedbacks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    public static InGameUI Instance {  get; private set; }

    [Header("Cooldowns")]
    [SerializeField] CooldownUI rollCooldown;
    [SerializeField] CooldownUI abilityCooldown;
    [SerializeField] CooldownUI potionCooldown;
    [SerializeField] CooldownUI runeCooldown;
    [Header("Rune")]
    
    [SerializeField] TextMeshProUGUI runeLifetimeText;

    [Header("Potion")]
    [SerializeField] TextMeshProUGUI potionCountText;

    [Header("Info")]
    [SerializeField] GameObject infoPanel;
    [SerializeField] TextMeshProUGUI infoText;
    [SerializeField] MMF_Player infoTextReveal;
    [SerializeField] GameObject[] runeInfoVisuals;
    MMF_TMPTextReveal textReveal;

    [Header("Panels")]
    [SerializeField] LevelProgressBar CombatUI;
    [SerializeField] AbilityCardShower cardPanel;
    [SerializeField] EndGameUI endGameUI;
    [SerializeField] GiftPanel giftPanelPrefab;

    [Header("Pause Menu")]
    [SerializeField] Button pauseMenuButton;
    [SerializeField] GameObject pauseMenu;

    [Header("Hide on Blacksmith")]
    [SerializeField] GameObject[] hideOnBlacksmith;
    private void OnEnable()
    {
        PotionUser.OnPotionSettled += ActivatePotionUI;
    }

    private void OnDisable()
    {
        PotionUser.OnPotionSettled -= ActivatePotionUI;
    }

    private void Awake()
    {       
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else Instance = this;

        potionCooldown.gameObject.SetActive(false);      
        runeCooldown.gameObject.SetActive(false);
        textReveal = infoTextReveal.GetFeedbackOfType<MMF_TMPTextReveal>();
        infoPanel.SetActive(false);
    }

    private void Start()
    {
        pauseMenuButton.onClick.AddListener(OpenPauseMenu);
    }

    public void EnterRollCooldown(float cooldown)
    {
        rollCooldown.EnterCooldown(cooldown);
    }

    public void EnterAbilityCooldown(float cooldown)
    {
        abilityCooldown.EnterCooldown(cooldown);
    }

    #region Potion
    public void EnterPotionCooldown(float cooldown)
    {
        potionCooldown.gameObject.SetActive(true);
        potionCooldown.EnterCooldown(cooldown);
    }
    private void ActivatePotionUI(PotionSO sO) => potionCooldown.gameObject.SetActive(true);

    public void DeactivatePotionUI() => potionCooldown.gameObject.SetActive(false);
    public void SetPotionText(int potionCount) => potionCountText.text = potionCount.ToString();

    #endregion

    #region Rune
    public void EnterRuneCooldown(float cooldown) => runeCooldown.EnterCooldown(cooldown); 
    public void ActivateRuneUI() => runeCooldown.gameObject.SetActive(true);
    public void DeactivateRuneUI() => runeCooldown.gameObject.SetActive(false);
    public void SetRuneLifeTimeText(int remaining) => runeLifetimeText.text = remaining.ToString();

    #endregion

    public void EnterInfoText(string text , float duration = 3f , bool isRuneInfo = false)
    {
        infoPanel.SetActive(true);
        textReveal.NewText = text;
        infoTextReveal.PlayFeedbacks();
        Invoke(nameof(CloseInfoPanel), duration);

        if (isRuneInfo) 
        {
            foreach (var visual in runeInfoVisuals)
            {
                visual.SetActive(true);
            }
        }
    }

    public void CloseInfoPanel()
    {
        if (infoPanel.activeInHierarchy)
        {
            infoPanel.SetActive(false);
            foreach (var visual in runeInfoVisuals)
            {
                visual.SetActive(false);
            }
        }
    }

    public void HideOnBlacksmithEnter()
    {
        foreach (var item in hideOnBlacksmith)
        {
            item.SetActive(false);
        }
    }

    public void ShowOnBlackSmithExit()
    {
        foreach (var item in hideOnBlacksmith)
        {
            item.SetActive(true);
        }
    }
    public void OpenAbilityCardPanel(bool test = false)
    {
        AbilityCardShower _cardPanel = Instantiate(cardPanel, transform);
        _cardPanel.TEST_CARD = test;
        _cardPanel.ShowRandomCards();
    }
   
    public void OpenGiftPanel(List<TransactionData> transactions)
    {
        GiftPanel giftPanel = Instantiate(giftPanelPrefab, this.transform);
        giftPanel.ShowDealCards(transactions);
    }
    public void OpenPauseMenu()
    {
        if (PlayerHealth.Instance)
        {
            PlayerHealth.Instance.GetComponent<PlayerStateMachine>().inputClosed = true;
        }
        MMTimeManager.Instance.SetTimeScaleTo(0);

        pauseMenu.SetActive(true);
    }

    public void ContinueGame()
    {
        if (PlayerHealth.Instance)
        {
            PlayerHealth.Instance.GetComponent<PlayerStateMachine>().inputClosed = false;
        }
        MMTimeManager.Instance.SetTimeScaleTo(1);

        if(pauseMenu.activeInHierarchy) pauseMenu.SetActive(false);
    }
    public void OpenEndGameUI()
    {
        EndGameUI ui = Instantiate(endGameUI, transform);
        ui.Show();
    }

}
