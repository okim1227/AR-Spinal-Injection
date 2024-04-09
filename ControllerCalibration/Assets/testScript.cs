using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class testScript : MonoBehaviour
{
    public TMP_Text textField;
    public GameObject rightController;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        textField.SetText("x: " + rightController.transform.position.x + "\n y:" + rightController.transform.position.y + "\n z:" + rightController.transform.position.z);
    }
}
