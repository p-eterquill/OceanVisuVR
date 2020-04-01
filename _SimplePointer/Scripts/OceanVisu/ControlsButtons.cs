using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsButtons : MonoBehaviour
{
    public TriangleMapFromLatLong map;
    protected PointMap[] planets;
    protected PointGraphs[] graphs;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void JoystickChosen(bool newValue)
    {
        planets = map.GetPlanets();
        graphs = map.GetGraphs();
        for (int i = 0; i < 4; i++)
        {
            planets[i].transform.SendMessage("SetMovementType", newValue);
            graphs[i].transform.SendMessage("SetMovementType", newValue);
        }
    }
}
