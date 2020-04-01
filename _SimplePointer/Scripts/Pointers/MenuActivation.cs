using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuActivation : MonoBehaviour
{
    public OVRInput.Button menu = OVRInput.Button.Two;
    private bool activity;

    public GameObject inGameCanvas = null;


    void Start()
    {
        activity = true;
        inGameCanvas = GameObject.Find("Canvas");
    }

    void LateUpdate()
    {
        inGameCanvas.SetActive(activity);
        if (OVRInput.Get(menu))
        {
            activity = true;
        }
        else
        {
            activity = false;
        }

    }
}

