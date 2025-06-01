using Cinemachine;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChampionPanelController : MonoBehaviour
{
    private Canvas canvas;
    private CinemachineBrain brain;
    private ChampionPanelVisual panelVisual;

    [SerializeField] List<ChampionSO> champions;
    [SerializeField] Button SpawnNextButton;
    [SerializeField] Button SpawnPreviousButton;
    [SerializeField] Button exitButton;
    [SerializeField] Button buyButton;
    [SerializeField] TextMeshProUGUI buyPriceText;
    [SerializeField] Button selectButton;
    [SerializeField] Button upgradeButton;
    [SerializeField] List<GameObject> selectedCharacterIdentifiers;
    [SerializeField] TextMeshProUGUI upgradePriceText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] GameObject[] stars;
    [SerializeField] int upgradePricePerLevel = 500;

    private int currentIndex;
    private GameObject currentChampionDummy;
    private ChampionSO currentChampionSO;
    private Champion currentChampion;

    private void Awake()
    {
        panelVisual = GetComponent<ChampionPanelVisual>();
        canvas = GetComponentInChildren<Canvas>();
        canvas.worldCamera = Camera.main;
    }
    private void Start()
    {
        Time.timeScale = 1f;
        AssignButtonMethods();
        ResetChampionNotification();

        currentIndex = SaveLoadHandler.Instance.GetPlayerData().OwnedChampions.Find(c => c.isSelected).ID;
        SpawnChampionAtIndex(currentIndex);
    }

    private void AssignButtonMethods()
    {
        SpawnNextButton.onClick.AddListener(SpawnNextChampion);
        SpawnPreviousButton.onClick.AddListener(SpawnPreviousChampion);
        buyButton.onClick.AddListener(OpenChampion);
        selectButton.onClick.AddListener(SelectChampion);
        upgradeButton.onClick.AddListener(UpgradeChampion);
        exitButton.onClick.AddListener(Exit);
    }

    private void ResetChampionNotification()
    {
        SaveLoadHandler.Instance.GetPlayerData().notification.ChampionNotification = false;
        NotificationSystem.Instance.ResetNotificationTrigger(NotificationSystem.Champion_Trigger);
    }

    private void SpawnChampionAtIndex(int index)
    {
        if (currentChampionDummy != null) { Destroy(currentChampionDummy); }

        var temporaries = FindObjectsByType<ChampionPanelTemporaryComponent>(FindObjectsSortMode.None);
        foreach (var temporary in temporaries) { Destroy(temporary.gameObject); }

        currentChampionSO = champions[index];
        currentChampion = SaveLoadHandler.Instance.GetPlayerData().OwnedChampions.Find(champ => champ.ID == currentChampionSO.ID);
        if (currentChampion == null) { currentChampion = new Champion(currentChampionSO.ID, 1); }
        ChampionDummy championDummy = Instantiate(currentChampionSO.Dummy , transform);
        currentChampionDummy = championDummy.gameObject;

        panelVisual.HandleChampionSpawnVisual(currentChampionSO, championDummy);
        ChangeButtonStatus();
    }

    private void SpawnNextChampion()
    {
        currentIndex++;
        if(currentIndex > champions.Count - 1)
        {
            currentIndex = 0;
        }
        SpawnChampionAtIndex(currentIndex);
    }

    private void SpawnPreviousChampion()
    {
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = champions.Count - 1;
        }
        SpawnChampionAtIndex(currentIndex);
    }
    
    private void ChangeButtonStatus()
    {
        if (currentChampion is null) { Debug.LogError("Champion is null!!!"); return; }

        CheckBuyStatus();
        CheckSelectStatus();
        CheckUpgradeStatus();

        levelText.text = currentChampion.Level.ToString() + " Lvl.";
        ShowStars();
    }

    private void CheckBuyStatus()
    {
        buyButton.gameObject.SetActive(CanBuy);
        buyButton.interactable = CanBuy;
        buyPriceText.text = currentChampionSO.BuyPrice.ToString();
    }

    private void CheckSelectStatus()
    {
        selectButton.gameObject.SetActive(CanSelect());
        selectButton.interactable = CanSelect();
        selectedCharacterIdentifiers.ForEach(characterIdentifier => characterIdentifier.SetActive(currentChampion.isSelected));
    }

    private void CheckUpgradeStatus()
    {
        bool isOwned = ChampionManager.Instance.GetOwnedChampionSOs().Contains(currentChampionSO);
        upgradeButton.gameObject.SetActive(isOwned);
        upgradePriceText.gameObject.SetActive(isOwned);
        upgradePriceText.text = currentChampion.Level == 5 ? "Max Level" : UpgradeCost.ToString();
        upgradeButton.interactable = CanUpgrade();
    }

    private void ShowStars()
    {
        foreach (var star in stars)
        {
            star.SetActive(false);
        }
        for (int i = 0; i < currentChampion.Level; i++)
        {
            stars[i].SetActive(true);
        }
    }

    private bool CanBuy => !ChampionManager.Instance.IsOwnedChampion(currentChampionSO) && EconomyManager.Instance.HasEnoughGem(currentChampionSO.BuyPrice);
    private bool CanSelect() => ChampionManager.Instance.IsOwnedChampion(currentChampionSO) && !currentChampion.isSelected;
    private bool CanUpgrade() => ChampionManager.Instance.IsOwnedChampion(currentChampionSO) && currentChampion.Level < 5 && 
                                 EconomyManager.Instance.HasEnoughGem(currentChampion.Level * upgradePricePerLevel);
    private int UpgradeCost => currentChampion.Level * upgradePricePerLevel;
    private void OpenChampion()
    {
        if (CanBuy)
        {
            EconomyManager.Instance.SpendGem(currentChampionSO.BuyPrice);
            SaveLoadHandler.Instance.GetPlayerData().OwnedChampions.Add(currentChampion);
            SaveLoadHandler.Instance.SaveData();
            panelVisual.PlayGlobalFeedbacks();
            SelectChampion();
        }
        ChangeButtonStatus();        
    }

    private void SelectChampion()
    {
        if (CanSelect()) 
        {
            SaveLoadHandler.Instance.GetPlayerData().OwnedChampions.Find(c => c.isSelected).isSelected = false; // Unselect current selected
            currentChampion.isSelected = true;
            SaveLoadHandler.Instance.SaveData();
        }
        ChangeButtonStatus();
    }

    private void UpgradeChampion()
    {
        if (CanUpgrade()) 
        {
            currentChampion.Level++;
            SaveLoadHandler.Instance.SaveData();
            panelVisual.PlayGlobalFeedbacks();
            EconomyManager.Instance.SpendGem(UpgradeCost);
        }
        ChangeButtonStatus();
    }

    public void SetCBrain(CinemachineBrain cinemachineBrain)
    {
        brain = cinemachineBrain;
        brain.enabled = true;
    }
    private void Exit()
    {
        brain.enabled = false;
        Camera.main.transform.rotation = Quaternion.identity;
        
        var temporaries = FindObjectsByType<ChampionPanelTemporaryComponent>(FindObjectsSortMode.None);
        foreach (var temporary in temporaries) { Destroy(temporary.gameObject); }

        MainMenu.Instance.gameObject.SetActive(true);
        Destroy(gameObject);
    }
}
