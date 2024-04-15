using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus;

public class AnchorPlacement : MonoBehaviour
{

    public GameObject anchorPrefab;

    // Start is called before the first frame update
    //void Start()
    //{
    //    Debug.Log("Started App");
    //}

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            //Debug.Log("Primary Index Trigger Pressed");
            CreateSpatialAnchor();
            //Debug.Log("passed spatial anc");
        }
    }

    public void CreateSpatialAnchor()
    {
        //string ovrInputMsg = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch)

        //Debug.Log($"controller pos: {OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch)}");

        //Debug.Log($"prefab outputtt: {anchorPrefab}");


        GameObject prefab = Instantiate(anchorPrefab, OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch), OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch));
        //Debug.Log("Anchor instantiated");

        prefab.AddComponent<OVRSpatialAnchor>();


        //Debug.Log($"added component at location: {prefab}");
    }
}
