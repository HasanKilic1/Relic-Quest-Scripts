using Cinemachine;
using MoreMountains.Feedbacks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    [SerializeField] CinemachineBrain cinemachineBrain;
    public static MainMenu Instance { get; private set; }

    [SerializeField] Button abilityPanelButton;
    [SerializeField] GameObject abilityPanel;
    [SerializeField] Button worldPanelButton;
    [SerializeField] GameObject worldPanel;
    [SerializeField] Button inventoryPanelButton;
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] Button shopPanelButton;
    [SerializeField] GameObject shopPanel;
    [SerializeField] Button championPanelButton;
    [SerializeField] GameObject championPanel;
    [SerializeField] Button mapButton;
    [SerializeField] GameObject mapPanel;

    [SerializeField] RewardShower rewardShower;
    RewardShower rewarder;
    private List<GameObject> allPanels;
    [SerializeField] GameObject economyUI;

    private void Awake()
    {
        Instance = this;

    }

    void Start()
    {
        Time.timeScale = 1.0f;
        Time.maximumDeltaTime = 0.33333f;
        abilityPanelButton.onClick.AddListener(OpenAbilityPanel);
        inventoryPanelButton.onClick.AddListener(OpenInventoryPanel);
        shopPanelButton.onClick.AddListener(OpenShopPanel);
        championPanelButton.onClick.AddListener(OpenChampionPanel);
        mapButton.onClick.AddListener(OpenMapPanel);
    }

    private void OpenAbilityPanel()
    {
        Instantiate(abilityPanel, transform);
    }

    private void OpenMapPanel()
    {
        Instantiate(mapPanel, transform);
    }
   
    private void OpenInventoryPanel()
    {
        Instantiate(inventoryPanel, transform);
    }
    public void OpenShopPanel()
    {
        Instantiate(shopPanel, transform);
    }
    
    private void OpenChampionPanel()
    {
        gameObject.SetActive(false);
        ChampionPanelController controller = Instantiate(championPanel).GetComponent<ChampionPanelController>();
        controller.SetCBrain(cinemachineBrain);
    }

    public void ShowRewardVisual(RewardType rewardType , int amount , ItemSO itemSO)
    {
        if(rewarder == null)
        {
            RewardShower _rewardShower = Instantiate(rewardShower, this.transform);
            rewarder = _rewardShower;
        }        
        rewarder.ShowReward(rewardType , amount , itemSO);
    }
    public void OpenEconomyUI() => economyUI.SetActive(true);
    public void CloseEconomyUI() => economyUI.SetActive(false);
    
}
