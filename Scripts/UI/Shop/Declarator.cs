using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Declarator : MonoBehaviour
{
    TextMeshProUGUI textMeshProUGUI;
    void Start()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        textMeshProUGUI.text = TextColorer.GetColorChangedTextViaItemType(textMeshProUGUI.text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
