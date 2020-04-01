using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VRPlanetInteract : MonoBehaviour
{
    public OVRInput.Controller controller = OVRInput.Controller.RTouch;

    private float speed = 80.0f;

    private bool circleViewType = true;


    //Used for translation of planets
    bool rightTriggerDown;
    //Use for rotation of planets
    bool leftTriggerDown;
    
    bool useJoystick = true;
    
    //Use if joystick
    Vector2 joystickPositionRight;
    Vector2 joystickPositionLeft;

    //Used if handmove = true
    Vector3 ctrlPosition;
    Vector3 previousCtrlPosition;

    GameObject[] obj;
    GameObject[] allClipP;
    List<int> indices;
    GameObject[] clipP;
    GameObject[] planetLabel;
    bool planetsFound = false;

    // Start is called before the first frame update
    void Start()
    {
        rightTriggerDown = false;
        leftTriggerDown = false;
    }

    
    // Update is called once per frame
    void Update()
    {

        if(!planetsFound)
        {
            obj = GameObject.FindGameObjectsWithTag("planet");
            allClipP = GameObject.FindGameObjectsWithTag("ClippingPlane");
            if (obj.Length!=0)
            {
                planetsFound = true;
                planetLabel = new GameObject[4];

                indices = new List<int>();

                clipP = new GameObject[obj.Length];

                for (int i = 0;i<allClipP.Length;i++)
                {
                    if(allClipP[i].name == "ClippingPlane")
                    {
                        indices.Add(i);
                    }
                }
                for(int i = 0; i < indices.Count; i++)
                {
                    clipP[i] = allClipP[indices[i]];
                }
                for (int i=0; i<obj.Length;i++)
                {
                    planetLabel[i] = GameObject.Find("PlanetTag" + i.ToString());
                }
                
                
            }
            
        }

        if(useJoystick)
        {
            joystickPositionRight = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
            joystickPositionLeft = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
        }
        else
        {
            ctrlPosition = OVRInput.GetLocalControllerPosition(controller);
        }
        

        var delta = (ctrlPosition - previousCtrlPosition) * 600;

        if (planetsFound)
        {
            ///TRANSLATION OF THE PLANETS ON X AND Y DEPENDING ON VIEW TYPE
            if (rightTriggerDown)
            {
                for (int i = 0; i < obj.Length; i++)
                {
                    if (circleViewType)
                    {
                        if (useJoystick)
                        {
                            CircularTranslation(obj[i], planetLabel[i], joystickPositionRight*Vector3.one);
                            CircularTranslation(clipP[i], planetLabel[i], joystickPositionRight *Vector3.one);
                        }
                        else
                        {
                            CircularTranslation(obj[i], planetLabel[i], delta);
                            CircularTranslation(clipP[i], planetLabel[i], delta);
                        }
                    }
                    else
                    {
                        if (useJoystick)
                        {
                            LinearTranslation(obj[i], planetLabel[i], joystickPositionRight * Vector3.one);
                            LinearTranslation(clipP[i], planetLabel[i], joystickPositionRight * Vector3.one);
                        }
                        else
                        {
                            LinearTranslation(obj[i], planetLabel[i], delta);
                            LinearTranslation(clipP[i], planetLabel[i], delta);
                        }
                    }
                }
            }

            ///ROTATION OF THE PLANETS AROUND THEMSELVES; CAN ONLY USE JOYSTICK
            if (leftTriggerDown)
            {
                for (int i = 0; i < obj.Length; i++)
                {
                    if (useJoystick)
                    {
                       Rotation(obj[i], joystickPositionLeft * Vector3.one);
                       Rotation(clipP[i], joystickPositionLeft * Vector3.one);
                    }
                    else
                    {
                        Rotation(obj[i], delta);
                        //Rotation(clipP[i], delta);
                    }
                }
            }


        }

        previousCtrlPosition = ctrlPosition;
    }


    private void CircularTranslation(GameObject go, GameObject planetLabel, Vector3 movement)
    {
        Debug.Log(go == null);
        go.transform.RotateAround(Vector3.zero, Vector3.up, movement.x * speed * Time.deltaTime);
        go.transform.Translate(Vector3.up * movement.y * speed * 2 * Time.deltaTime);
       
        planetLabel.transform.RotateAround(Vector3.zero, Vector3.up, movement.x * speed/2f * Time.deltaTime);
        planetLabel.transform.Translate(Vector3.up * movement.y * speed * Time.deltaTime,Space.World);
    }

    private void LinearTranslation(GameObject go, GameObject planetLabel, Vector3 movement)
    {
        go.transform.Translate(Vector3.right * movement.x * speed * 2 * Time.deltaTime,Space.World);
        go.transform.Translate(Vector3.up * movement.y * speed * 2 * Time.deltaTime,Space.World);
        planetLabel.transform.Translate(Vector3.right * movement.x * speed * 2 * Time.deltaTime);
        planetLabel.transform.Translate(Vector3.up * movement.y * speed * 2 * Time.deltaTime);
    }

    private void Rotation(GameObject go, Vector3 movement)
    {
        go.transform.RotateAround(go.transform.position, Vector3.up, -movement.x * Time.deltaTime * speed);
        go.transform.RotateAround(go.transform.position, Vector3.right, movement.y * Time.deltaTime * speed);

        //planetLabel.transform.SendMessage("CompensateRotation", new Vector3(movement.y, -movement.x, 0.0f) *Time.deltaTime * speed);
    }
    

    ///TRIGGERS CALLED IN PHYSICS POINTER
    void OnTriggerIsDown(int trigger)
    {
        if(trigger == 1)
        {
            rightTriggerDown = true;
        }
        else if(trigger == 2)
        {
            leftTriggerDown = true;
        }
    }

    void OnTriggerIsUp(int trigger)
    {
        if (trigger == 1)
        {
            rightTriggerDown = false;
        }
        else if (trigger == 2)
        {
            leftTriggerDown = false;
        }
    }

    

    void SetMovementType(bool type)
    {
        if(!type)
        {
            useJoystick = false;
        }
        else
        {
            useJoystick = true;
        }
        
    }

    void SetViewType(bool type)
    {
        if(!type)
        {
            circleViewType = false;
        }
        else
        {
            circleViewType = true;
        }
        GameObject.Find("Canvas").transform.SendMessage("SetViewType", type);
    }

    public bool GetViewType()
    {
        return circleViewType;
    }
}
