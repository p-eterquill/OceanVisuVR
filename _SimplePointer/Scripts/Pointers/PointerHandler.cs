using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PointerHandler : MonoBehaviour
{
    public GameObject inGameCanvas;
    public Panel mainPanel;
    bool canvasStatus;
    bool previousCanvasStatus;
    bool pointerChanged = false;

    protected GameObject physicsPointer;
    protected GameObject canvasPointer;
    protected EventSystem eventSystem;

    // Start is called before the first frame update
    void Start()
    {
        canvasStatus = inGameCanvas.activeSelf;
        previousCanvasStatus = inGameCanvas.activeSelf;
        canvasPointer = GameObject.Find("CanvasPointer");
        physicsPointer = GameObject.Find("PhysicsPointer");
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        canvasStatus = inGameCanvas.activeSelf;
        if(canvasStatus != previousCanvasStatus)
        {
            pointerChanged = true;
            previousCanvasStatus = canvasStatus;
        }
        else
        {
            pointerChanged = false;
        }

        if(pointerChanged)
        {
            if (canvasStatus)
            {
                physicsPointer.GetComponent<PhysicsRaycaster>().enabled = false;
                canvasPointer.gameObject.SetActive(true);

            }
            else
            {
                physicsPointer.GetComponent<PhysicsRaycaster>().enabled = true;
                inGameCanvas.GetComponent<MenuManager>().GoToPrevious();
                canvasPointer.gameObject.SetActive(false);
            }
        }

    }
}
