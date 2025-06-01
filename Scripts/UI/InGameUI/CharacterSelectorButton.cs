using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectorButton : MonoBehaviour
{
    [SerializeField] int CharacterID;
    private Button button;
    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Select);
    }
    private void Select()
    {
        SaveLoadHandler.Instance.GetPlayerData().selectedChampionID = CharacterID;
        SaveLoadHandler.Instance.SaveData();

        SceneManager.LoadScene("HighGarden");
    }
}
