using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ImageSlider : MonoBehaviour
{

    public Texture[] imageArray;
    private int currentImage;
    public Slider slider;
    public RawImage rawImage;

    // Start is called before the first frame update
    void Start()
    {
        slider.maxValue = imageArray.Length;
        slider.value = 0;
        print(slider.value);
        print(slider.maxValue);

        slider.onValueChanged.AddListener((float value) => OnSliderValueChanged(value));
        
        int index = (int)slider.value;
        UpdateDisplayedImage(index);
    }

    void OnSliderValueChanged(float value) {
        int index = (int)slider.value;
        UpdateDisplayedImage(index);
    }

    void UpdateDisplayedImage(int index) {

        // Ensure index is within valid range
        if (index >= 0 && index < imageArray.Length)
        {
            // Update RawImage to display the selected texture
            rawImage.texture = imageArray[index];
        }
    }

}
