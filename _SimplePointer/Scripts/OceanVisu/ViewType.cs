using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewType : MonoBehaviour
{
    // Start is called before the first frame update
    public TriangleMapFromLatLong map;
    protected PointMap[] planets;


    public void CircularViewChosen(bool newValue)
    {
        planets = map.GetPlanets();
        for (int i = 0; i < planets.Length; i++)
        {
            planets[i].transform.SendMessage("SetViewType", newValue);
        }
    }
}
