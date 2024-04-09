using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugDisplay : MonoBehaviour
{

    Dictionary<string, string> debugText = new Dictionary<string, string>();

    public TMP_Text display;

    // Start is called before the first frame update
    void Start()
    {
        Application.logMessageReceived += HandleLog;

    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTree, LogType type)
    {
        if (type == LogType.Log)
        {
            string[] splitString = logString.Split(char.Parse(":"));
            string debugKey = splitString[0];
            string debugValue = splitString.Length > 1 ? splitString[1] : "";

            if (debugText.ContainsKey(debugKey))
            {
                debugText[debugKey] = debugValue;
            }
            else
            {
                debugText.Add(debugKey, debugValue);
            }
        }

        string displayText = "";
        foreach (KeyValuePair<string, string> entry in debugText)
        {
            if (entry.Value == "")
            {
                displayText += entry.Key + "\n";
            }
            else
            {
                displayText += entry.Key + ": " + entry.Value + "\n";
            }
        }
        display.SetText(displayText);
    }   

    // Update is called once per frame
    void Update()
    {
    }
}
