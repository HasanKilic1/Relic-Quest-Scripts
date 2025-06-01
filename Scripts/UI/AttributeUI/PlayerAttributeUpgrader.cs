using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttributeUpgrader : MonoBehaviour
{
    Attribute effectedAttribute;
    [field : SerializeField] public AttributeType AttributeType {  get; private set; }
    [SerializeField] string attributeInfo;
    public int index;

    [Header("Visual and Info")]
    private RectTransform rectTransform;
    [SerializeField] TextMeshProUGUI attributeLevelText;
    [SerializeField] GameObject highLight;
    [SerializeField] GameObject highLightFrame;
    [SerializeField] GameObject info_panel;
    [SerializeField] TextMeshProUGUI infoText;
    private Button button;

    PlayerData playerData;
    private bool canRotateHighlight = false;
    private void Awake()
    {
        button = GetComponentInChildren<Button>();
        button.onClick.AddListener(() =>
        {
            info_panel.SetActive(false); // disable to refresh 
            info_panel.transform.position = transform.position + Vector3.right * 35f;
            info_panel.SetActive(true);
            infoText.text = attributeInfo;
        });
    }
    void Start()
    {
        playerData = SaveLoadHandler.Instance.GetPlayerData();
        effectedAttribute = playerData.GetPlayerAttributeByType(AttributeType);
        UpdateAttributeLevelText();
    }
    private void Update()
    {
        if (canRotateHighlight)
        {
            //highLightFrame.transform.Rotate(90 * Time.deltaTime * Vector3.forward);
            highLightFrame.GetComponent<RectTransform>().localEulerAngles += new Vector3(0,0,90) * Time.deltaTime;
        }
    }
    public void UpgradeSelectedAttribute()
    {
        effectedAttribute.attributeLevel += 1;
        Debug.Log("type of: " +effectedAttribute.attributeType.ToString() + " attribute, now has level of : " + effectedAttribute.attributeLevel);
        UpdateAttributeLevelText();
    }

    private void UpdateAttributeLevelText()
    {
        attributeLevelText.text = effectedAttribute.attributeLevel.ToString();
    }

    public void HighLight()
    {
        canRotateHighlight = true;
        highLight.SetActive(true);
    }

    public void UnHighLight()
    {
        ResetHighlightRotation();
        highLight.SetActive(false);
    }

    private void ResetHighlightRotation()
    {
        canRotateHighlight = false;
        highLightFrame.transform.rotation = Quaternion.identity;
    }

    public bool IsUpgradable => effectedAttribute.attributeLevel < effectedAttribute.maxLevel;
}
