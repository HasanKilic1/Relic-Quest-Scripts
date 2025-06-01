using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopTapButton : MonoBehaviour
{
    [SerializeField] GameObject goldPanel;
    [SerializeField] GameObject goldFocus;
    [SerializeField] TextMeshProUGUI goldText;
    [SerializeField] GameObject chestPanel;
    [SerializeField] GameObject chestFocus;
    [SerializeField] TextMeshProUGUI chestText;
    [SerializeField] GameObject gemPanel;
    [SerializeField] GameObject gemFocus;
    [SerializeField] TextMeshProUGUI gemText;

    GameObject activePanel;
    GameObject activeFocus;
    TextMeshProUGUI activeText;
    [SerializeField] Button goldTapButton;
    [SerializeField] Button chestTapButton;
    [SerializeField] Button gemTapButton;
    [SerializeField] Button closeButton;
    [SerializeField] GameObject panelParent;
    [SerializeField] Color textNormalColor;
    [SerializeField] Color textActiveColor;
    void Start()
    {
        goldTapButton.onClick.AddListener(OpenGoldPanel);
        chestTapButton.onClick.AddListener(OpenChestPanel);
        gemTapButton.onClick.AddListener(OpenGemPanel);
        closeButton.onClick.AddListener(() => Destroy(panelParent));
        OpenChestPanel();
    }

    private void OpenGoldPanel()
    {
        CloseActivePanel();
        goldPanel.SetActive(true);
        goldFocus.SetActive(true);
        activePanel = goldPanel;
        activeFocus = goldFocus;
        
        goldText.color = textActiveColor;
        activeText = goldText;
    }
    private void OpenChestPanel()
    {
        CloseActivePanel();
        chestPanel.SetActive(true);
        chestFocus.SetActive(true);
        activePanel = chestPanel;
        activeFocus = chestFocus;

        chestText.color = textActiveColor;
        activeText = chestText;
    }
    private void OpenGemPanel()
    {
        CloseActivePanel();
        gemPanel.SetActive(true);
        gemFocus.SetActive(true);
        activePanel = gemPanel;
        activeFocus = gemFocus;

        gemText.color = textActiveColor;
        activeText = gemText;
    }

    private void CloseActivePanel()
    {
        if(activeText != null) { activeText.color = textNormalColor; }
        if (activePanel != null) { activePanel.SetActive(false); }
        if(activeFocus != null) { activeFocus.SetActive(false);}
    }
}
