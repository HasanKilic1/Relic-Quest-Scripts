using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GiftPanel : MonoBehaviour
{
    public static EventHandler OnDecisionEnd;
    [SerializeField] DealCard card;
    [SerializeField] Transform cardHolder;
    [SerializeField] Button acceptButton;
    [SerializeField] Button rejectButton;
    private List<DealCard> cards;
    private List<TransactionData> pendingTransactions;

    private void Awake()
    {
        pendingTransactions = new List<TransactionData>();
        cards = new List<DealCard>();

        acceptButton.onClick.AddListener(AcceptDeal);
        rejectButton.onClick.AddListener(Close);
    }

    public void ShowDealCards(List<TransactionData> transactions)
    {
        for (int i = 0; i < transactions.Count; i++)
        {
            DealCard spawned = Instantiate(card, cardHolder);
            cards.Add(spawned);
            spawned.SetupDealCard(transactions[i]);
            pendingTransactions.Add(transactions[i]);
        }
    }

    public void AcceptDeal()
    {
        foreach (var transaction in pendingTransactions)
        {
            transaction.PerformTransaction(EconomyManager.Instance, PlayerHealth.Instance);
        }
        ClearTransactions();
        Close();
    }

    public void Close()
    {
        Time.timeScale = 1f;
        foreach (var card in cards)
        {
            Destroy(card.gameObject);
        }
        ClearTransactions();
        cards.Clear();
        OnDecisionEnd?.Invoke(this , EventArgs.Empty);
        gameObject.SetActive(false);
    }

    private void ClearTransactions()
    {
        pendingTransactions.Clear();
    }  
}
