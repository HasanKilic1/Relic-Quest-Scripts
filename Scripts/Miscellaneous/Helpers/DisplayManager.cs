using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayManager : MonoBehaviour
{
    void Start()
    {
        // Activate the first display (index 0) if multiple displays are supported
        if (Display.displays.Length > 1)
        {
            Display.displays[0].Activate();
            // Optionally, you can activate other displays if needed
            // Display.displays[1].Activate();
            // Display.displays[2].Activate();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
