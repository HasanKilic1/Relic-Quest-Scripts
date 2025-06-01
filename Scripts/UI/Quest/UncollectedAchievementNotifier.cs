using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UncollectedAchievementNotifier : MonoBehaviour
{
    private QuestContainer questContainer;
    [SerializeField] GameObject notifier;

    private void Awake()
    {
        questContainer = GetComponent<QuestContainer>();
    }

    private void Start()
    {
        StartCoroutine(Check());
    }
    private IEnumerator Check()
    {
        while (true)
        {
            notifier.SetActive(HasAnyUncollected());
            yield return new WaitForSeconds(1f);
        }
    }

    private bool HasAnyUncollected()
    {
        return SaveLoadHandler.Instance.GetPlayerData().Achievements.Any(quest => quest.isCompleted && !quest.isPrizeTaken);
    }
}
