using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImageSelector : MonoBehaviour
{
    public Texture2D[] _sprites;
    public RawImage image;
    public Slider slider;
    public TextMeshProUGUI textMeshPro;
    // Start is called before the first frame update
    void Start()
    {
        slider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        slider.minValue = 0;
        slider.maxValue = _sprites.Length;
    }

    public void ValueChangeCheck()
    {
        image.texture = _sprites[(int) slider.value];
        textMeshPro.text = $"CT Slice Number: {slider.value}";
    }
    private Texture2D LoadTexture(string FilePath)
    {

        // Load a PNG or JPG file from disk to a Texture2D
        // Returns null if load fails

        Texture2D Tex2D;
        byte[] FileData;

        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
            if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                return Tex2D;                 // If data = readable -> return texture
        }
        return null;                     // Return null if load failed
    }

    // Update is called once per frame
    void Update()
    {

        
    }
}
