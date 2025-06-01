using TMPro;
using UnityEngine;

public class VersionHandler : MonoBehaviour
{
    [field: SerializeField] public string AppVersion {  get; private set; }
    [SerializeField] string UpdateNotify;
    [SerializeField] TextMeshProUGUI versionText;

    private void Start()
    {
        if(versionText != null)
        {
            versionText.text = "Version " + AppVersion.ToString();
        }
    }
}
