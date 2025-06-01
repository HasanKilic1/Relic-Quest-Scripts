using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class QuestPrizeCollector : MonoBehaviour
{
    public static event Action OnAllPrizesCollected;

    [SerializeField] private Button collectAllButton;

    private void Start()
    {
        collectAllButton.onClick.AddListener(CollectAllPrizes);
    }

    private void CollectAllPrizes()
    {
        OnAllPrizesCollected?.Invoke();
    }
}
