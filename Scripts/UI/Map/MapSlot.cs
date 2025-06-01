using UnityEngine;
using UnityEngine.UI;

public class MapSlot : MonoBehaviour
{
    [SerializeField] int OPEN_MAP_ID = 0;
    [Header("UI")]
    [SerializeField] GameObject[] lockedObjects;
    [SerializeField] GameObject[] unlockedObjects;
    [SerializeField] Button interactButton;

    private void Start()
    {
        bool canOpen = SaveLoadHandler.Instance.GetPlayerData().openedMapIDs.Contains(OPEN_MAP_ID);
        interactButton.interactable = canOpen;

        foreach (var obj in unlockedObjects)
        {
            obj.SetActive(canOpen);
        }

        foreach (var obj in lockedObjects)
        {
            obj.SetActive(!canOpen);
        }
    }

}
