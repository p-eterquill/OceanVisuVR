using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRGraphInteract : MonoBehaviour
{
    public OVRInput.Controller controller = OVRInput.Controller.RTouch;
    protected GameObject tMap;
    protected TriangleMapFromLatLong map;
    protected GameObject pGr;

    protected bool isRayCasting;
    private float speed = 80.0f;

    bool rightTriggerDown;
    bool leftTriggerDown;
    bool handTriggerIsDown;
    RaycastHit hit;
    private bool circleViewType = true;
    bool useJoystick = true;
    bool graphFound;

    //Use if joystick
    Vector2 joystickPositionRight;
    Vector2 joystickPositionLeft;

    //Used if handmove = true
    Vector3 ctrlPosition;
    Vector3 previousCtrlPosition;

    // Start is called before the first frame update
    void Start()
    {
        tMap = GameObject.Find("TriangleMapFromLatLong");
        map = tMap.GetComponent<TriangleMapFromLatLong>();
        hit = new RaycastHit();
        pGr = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (useJoystick)
        {
            joystickPositionRight = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
            joystickPositionLeft = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
        }
        else
        {
            ctrlPosition = OVRInput.GetLocalControllerPosition(controller);
        }


        var delta = (ctrlPosition - previousCtrlPosition) * 600;

        /*if (handTriggerIsDown)
        {
            PointMap[] planets = map.GetPlanets();
            for (int i = 0; i < planets.Length; i++)
            {
                planets[i].RemoveLaserAndPoints();
            }

            if (hit.collider)//.tag == "graph")
                {
                    float distMin = 10000;
                    Vector3 pointProche = new Vector3();

                    Dictionary<Vector3, List<Vector3>> graphToSphere = hit.collider.transform.parent.GetComponent<PointGraphs>().getPointsDict();
                    Dictionary<Vector3, List<Vector3>> graphToPlane = hit.collider.transform.parent.GetComponent<PointGraphs>().getPointsDictPlane();
                    foreach (Vector3 point in graphToSphere.Keys)
                    {
                        float dist = Vector3.Distance(hit.collider.transform.position + point, hit.point);
                        if (dist < distMin)
                        {
                            pointProche = point;
                            distMin = dist;
                        }
                    }

                    for (int i = 0; i < planets.Length; i++)
                    {
                        planets[i].SetLaser(graphToSphere[pointProche], true);
                        planets[i].SetLaserPlane(graphToPlane[pointProche], true);
                        planets[i].RefreshLasers(planets[i].GetHiddenStatus());
                    }
            }
            
        }
        */

        /*
                if (Input.GetKey(KeyCode.LeftAlt))
                {

                    PointMap[] planets = map.GetPlanets();

                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(mousePosition);

                    if (Physics.Raycast(ray, out hit, 2000f))
                    {
                        if (hit.collider.tag == "graph")
                        {
                            float distMin = 10000;
                            Vector3 pointProche = new Vector3();

                            Dictionary<Vector3, List<Vector3>> graphToSphere = hit.collider.transform.parent.GetComponent<PointGraphs>().getPointsDict();
                            Dictionary<Vector3, List<Vector3>> graphToPlane = hit.collider.transform.parent.GetComponent<PointGraphs>().getPointsDictPlane();
                            foreach (Vector3 point in graphToSphere.Keys)
                            {
                                float dist = Vector3.Distance(hit.collider.transform.position + point, hit.point);
                                if (dist < distMin)
                                {
                                    pointProche = point;
                                    distMin = dist;
                                }
                            }

                            for (int i = 0; i < planets.Length; i++)
                            {
                                planets[i].SetLaser(graphToSphere[pointProche], true);
                                planets[i].SetLaserPlane(graphToPlane[pointProche], true);
                                planets[i].RefreshLasers(planets[i].GetHiddenStatus());
                            }
                        }
                    }
                }
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    PointMap[] planets = map.GetPlanets();

                    for (int i = 0; i < planets.Length; i++)
                    {
                        planets[i].RemoveLaserAndPoints();
                    }
                }*/
        if(!graphFound)
        {
            pGr = GameObject.FindGameObjectWithTag("wholeGraph");
        }
        if(pGr != null)
        {
            graphFound = true;
        }
        rightTriggerDown = true;
        if (graphFound)
        {
            Debug.Log("GRAPH FROUND");
            ///TRANSLATION OF THE PLANETS ON X AND Y DEPENDING ON VIEW TYPE
            if (rightTriggerDown)
            {
                //APPLIQUER LA TRANSFOR A TOUS LES SOUS6GAMEOBJECTS TAGGE GRAPH AVEC UN FOR
                if (circleViewType)
                {
                    if (useJoystick)
                    {
                        CircularTranslation(pGr.transform.parent.gameObject, joystickPositionRight * Vector3.one);
                    }
                    else
                    {
                        CircularTranslation(pGr.transform.parent.gameObject, delta);
                    }
                }
                else
                {
                    if (useJoystick)
                    {
                        LinearTranslation(pGr.transform.parent.gameObject, joystickPositionRight * Vector3.one);
                    }
                    else
                    {
                        LinearTranslation(pGr.transform.parent.gameObject, delta);
                    }
                }
                
            }

            ///ROTATION OF THE PLANETS AROUND THEMSELVES; CAN ONLY USE JOYSTICK
            if (leftTriggerDown)
            {
               
                if (useJoystick)
                {
                    Rotation(pGr.transform.parent.gameObject, joystickPositionLeft * Vector3.one);
                }
                else
                {
                    Rotation(pGr.transform.parent.gameObject, delta);
                    //Rotation(clipP[i], delta);
                }
                
            }


        }

        previousCtrlPosition = ctrlPosition;
    }


    private void CircularTranslation(GameObject go, Vector3 movement)
    {
        Debug.Log(go == null);
        go.transform.RotateAround(Vector3.zero, Vector3.up, movement.x * speed * Time.deltaTime);
        go.transform.Translate(Vector3.up * movement.y * speed * 2 * Time.deltaTime);
        
    }

    private void LinearTranslation(GameObject go, Vector3 movement)
    {
        go.transform.Translate(Vector3.right * movement.x * speed * 2 * Time.deltaTime, Space.World);
        go.transform.Translate(Vector3.up * movement.y * speed * 2 * Time.deltaTime, Space.World);
    }

    private void Rotation(GameObject go, Vector3 movement)
    {
        go.transform.RotateAround(go.transform.position, Vector3.up, -movement.x * Time.deltaTime * speed);
        go.transform.RotateAround(go.transform.position, Vector3.right, movement.y * Time.deltaTime * speed);

        //planetLabel.transform.SendMessage("CompensateRotation", new Vector3(movement.y, -movement.x, 0.0f) *Time.deltaTime * speed);
    }





void OnTriggerIsDown(int trigger)
    {
        if (trigger == 1)
        {
            rightTriggerDown = true;
        }
        else if (trigger == 2)
        {
            leftTriggerDown = true;
        }
    }


    public void OnHandTriggerIsDown(RaycastHit pHit)
    {
        Debug.Log("###########################################################################");
        handTriggerIsDown = true;
        hit = hit;
    }

    public void OnHandTriggerIsUp()
    {
        
        handTriggerIsDown = false;
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
        if (!type)
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
        if (!type)
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