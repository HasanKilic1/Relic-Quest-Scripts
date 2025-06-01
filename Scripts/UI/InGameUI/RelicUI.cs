using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelicUI : MonoBehaviour
{
    PlayerStateMachine playerStateMachine;
    AbilityCardShower abilityCardShower;
    [Header("Ability sprites")]
    [SerializeField] Button selectButton;
    [SerializeField] Image abilityIcon;

    [SerializeField] TextMeshProUGUI cardName;
    [SerializeField] TextMeshProUGUI rarity;
    [SerializeField] TextMeshProUGUI cardDeclaration;
    [SerializeField] MMF_Player selectionFeedbacks;
    [SerializeField] MMF_Player startFeedBacks;

    private MMF_Position mmf_position;

    [SerializeField] RelicSO relicSO;
    private void Awake()
    {
        mmf_position = startFeedBacks.GetFeedbackOfType<MMF_Position>();
    }

    private void Start()
    {
        playerStateMachine = PlayerController.Instance.GetComponent<PlayerStateMachine>();
        selectButton.onClick.AddListener(SelectAbility);
        startFeedBacks?.PlayFeedbacks();
    }

    public void SetupRelicUI(RelicSO relicSO , AbilityCardShower abilityCardShower)
    {
        HandleUI(relicSO, abilityCardShower);
        this.relicSO = relicSO;
    }

    private void HandleUI(RelicSO relicSO, AbilityCardShower abilityCardShower)
    {
        this.abilityCardShower = abilityCardShower;
        abilityIcon.sprite = relicSO.Icon;
        cardName.text = relicSO.Name;
        rarity.text = SaveLoadHandler.Instance.GetPlayerData().Relics.Find(relic => relic.Id == relicSO.ID).rarity.ToString();
        cardDeclaration.text = relicSO.Declaration;
    }

    private void SelectAbility()
    {
        HandleSelection();
    }

    private void HandleSelection()
    {
        selectionFeedbacks?.PlayFeedbacks();
        IRelic relicInterface = GetRelicInterface();
        CloseRelicShower(relicInterface);
        UpdateRelicDataSelectionProgress();
    }

    private IRelic GetRelicInterface()
    {
        var relic = Instantiate(relicSO.abilityCardPrefab);
        IRelic relicInterface = relic.GetComponent<IRelic>();
        relicInterface.SettleEffect(playerStateMachine);
        return relicInterface;
    }

    private void CloseRelicShower(IRelic relicInterface)
    {
        if (abilityCardShower != null)
        {
            abilityCardShower.Close(relicInterface);
        }
    }

    private void UpdateRelicDataSelectionProgress()
    {
        RelicData relicData = CardDataManager.Instance.GetRelicDataByID(relicSO.ID);
        relicData.totalSelected++;
    }

    public Button GetSelectButton => selectButton;
}
