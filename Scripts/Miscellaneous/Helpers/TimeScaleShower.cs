using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeScaleShower : MonoBehaviour
{
    private TextMeshProUGUI text;
    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        text.text = "Time scale: " + Time.timeScale;
    }
}
