using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DevConsole : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TextMeshProUGUI outputText;
    [SerializeField] List<CommandSO> commands;
    [SerializeField] TextMeshProUGUI dataPathText;
    [SerializeField] TextMeshProUGUI dataInfoText;
    [SerializeField] bool showRelicCount = true;
    private void Start()
    {
        inputField.onSubmit.AddListener(delegate { SubmitCommand(); });
        AdjustDataTexts();
    }
    private void Update()
    {
        if(Keyboard.current.tabKey.wasPressedThisFrame)
        {
            OpenInputField();
        }       
    }

    private void OpenInputField()
    {
        if (inputField.gameObject.activeSelf)
        {
            Time.timeScale = 1.0f;
            inputField.gameObject.SetActive(false);
        }
        else
        {
            Time.timeScale = 0f;
            inputField.gameObject.SetActive(true);
        }
    }

    private void SubmitCommand()
    {
        string command = inputField.text;
        outputText.text = command;        
        inputField.text = "";

        bool isValidCommand = false;
        foreach (var commandSO in commands)
        {
            commandSO.Execute(command);
            if (commandSO.ValidCommand(command))
            {
                isValidCommand = true;
                outputText.text = commandSO.Output;
            }           
        }

        if(!isValidCommand)
        {
            outputText.text = "Invalid command!";
        }
       
    }

    private void AdjustDataTexts()
    {
        dataPathText.text = "Path : " + Application.persistentDataPath;
        string json = File.ReadAllText(Application.persistentDataPath + "/data.json");
        PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);

        string dataInfo = "";
        if (playerData == null) dataInfo = "Invalid serialization";
        else 
        {            
            dataInfo = "Selected character ID : " + playerData.selectedChampionID.ToString();
            dataInfo += "Save date :" + playerData.LastSaveTime;
            dataInfo += "Total relic count : " + playerData.Relics.Count;
            if(showRelicCount) dataInfo += "Relic manager relics : " + CardDataManager.Instance.RelicSOs.Count;
        }         
        dataInfoText.text = dataInfo;
    }
}
