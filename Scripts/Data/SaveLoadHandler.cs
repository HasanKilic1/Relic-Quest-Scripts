using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveLoadHandler : MonoBehaviour
{
    public static SaveLoadHandler Instance { get; private set; }
    PlayerData playerData;
    private const string saveName = "/data.json";
    [SerializeField] List<AchievementSO> allAchievements;
    [SerializeField] List<ChampionSO> allChampions;
    [SerializeField] List<RelicSO> allRelics;
    [SerializeField] List<ItemSO> allItems;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        LoadData();
    }

    private void Update()
    {
        if (Keyboard.current.deleteKey.wasPressedThisFrame)
        {
            ClearData();
        }
    }
    public void SaveData()
    {
        playerData.LastSaveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string json = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(Application.persistentDataPath + saveName, json);
    }
    public void LoadData()
    {
        if (File.Exists(Application.persistentDataPath + saveName))
        {
            string json = File.ReadAllText(Application.persistentDataPath + saveName);
            playerData = JsonUtility.FromJson<PlayerData>(json);   
            if(playerData.appVersion != GetComponent<VersionHandler>().AppVersion) // version is different.
            {
                HandleUpdate();
            }
        }

        else
        {
            OpenNewJsonDataFile();
        }
    }

    private void HandleUpdate()
    {
        PlayerData oldVersion = playerData;
        ClearData();
        OpenNewJsonDataFile();
        playerData.MatchWithOldVersion(oldVersion, GetComponent<VersionHandler>().AppVersion, allAchievements);
    }

    public void OpenNewJsonDataFile()
    {
        playerData = new PlayerData();
        playerData.appVersion = GetComponent<VersionHandler>().AppVersion; 
        InitializeFirstTime();
        string json = JsonUtility.ToJson(playerData);        
        File.WriteAllText(Application.persistentDataPath + saveName, json);      
        SaveData();

        HKDebugger.LogInfo("New Json File has opened");
    }

    public PlayerData GetPlayerData()
    {
        return this.playerData;
    }
    public void InitializeFirstTime()
    {
        playerData.InitializeFirstTime(AttributeManager.Instance.BasePlayerAttributes , allRelics , allItems , allChampions, allAchievements);
    }
    public void ClearData()
    {
        if (File.Exists(Application.persistentDataPath + saveName))
        {
            File.Delete(Application.persistentDataPath + saveName);
        }
        playerData = new PlayerData();
        Debug.Log("Data cleared");
    }
}
