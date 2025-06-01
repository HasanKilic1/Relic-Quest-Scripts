using MoreMountains.Feedbacks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilityCardShower : MonoBehaviour
{
    PlayerStateMachine playerStateMachine;
    PlayerFeedbackController playerFeedbackController;
    [SerializeField] Transform[] cardPositions;
    [SerializeField] RelicUI cardPrefab;
    [SerializeField] MMF_Player showFeedbacks; 
    private List<RelicUI> spawnedRelics;
    private IRelic selectedRelic;

    [HideInInspector] public bool TEST_CARD = false;
    private void Start()
    {
        playerStateMachine = PlayerController.Instance.GetComponent<PlayerStateMachine>();
        playerStateMachine.inputClosed = true;
        playerFeedbackController = playerStateMachine.GetComponent<PlayerFeedbackController>();
    }
    public void Close(IRelic relic)
    {
        Invoke(nameof(Deactivate), 1.3f);
        foreach (var card in spawnedRelics)
        {
            card.GetSelectButton.interactable = false;
        }
        playerStateMachine.inputClosed = false;
        if (!TEST_CARD)
        {
            GameStateManager.Instance.StartNewLevel();
        }        
        selectedRelic = relic;
    }
    
    private void Deactivate()
    {
        gameObject.SetActive(false);
        playerFeedbackController.PlayCardSettleFeedbacks(selectedRelic);
    }

    public void ShowRandomCards()
    {
        selectedRelic = null;
        spawnedRelics = new List<RelicUI>();

        if (showFeedbacks) showFeedbacks.PlayFeedbacks();

        List<RelicSO> availableRelics = new List<RelicSO>(CardDataManager.Instance.RelicSOs); // Set all the relics as available

        for (int i = 0; i < cardPositions.Length; i++)
        {
            RelicSO randomRelic = availableRelics[Random.Range(0, availableRelics.Count)]; 
            RelicUI card = Instantiate(cardPrefab, cardPositions[i]);
            card.SetupRelicUI(randomRelic , this);

            availableRelics.Remove(randomRelic); //remove the selected relic
            spawnedRelics.Add(card);
        }
        
        Invoke(nameof(SetButtonAsSelected), 0.2f);
    }
    private void SetButtonAsSelected()
    {
        EventSystem.current.SetSelectedGameObject(spawnedRelics[0].GetSelectButton.gameObject);
    }

}
