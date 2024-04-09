using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderControl : MonoBehaviour
{
    // Start is called before the first frame update

    public Slider slider;

    public TMP_Text text;

    void Start()
    {
    }



    // Update is called once per frame
    void Update()
    {
        bool buttonPressed = OVRInput.GetDown(OVRInput.Button.One);
        if (buttonPressed)
        {
            slider.value += 1;
            text.SetText("Value: " + slider.value.ToString("F2"));
        }

        bool otherButtonPressed = OVRInput.GetDown(OVRInput.Button.Two);
        if (otherButtonPressed)
        {
            slider.value -= 1;
            text.SetText("Value: " + slider.value.ToString("F2"));
        }

    }
}
