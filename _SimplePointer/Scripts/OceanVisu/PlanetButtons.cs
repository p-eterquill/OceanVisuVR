using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetButtons : MonoBehaviour
{
    public TriangleMapFromLatLong map;

    protected PointMap[] planets;
    protected ClippingPlane[] cp;

    protected bool circleViewType = true;

    protected SortedDictionary<int, PointMap> visiblePlanets;
    protected SortedDictionary<int, PointMap> invisiblePlanets;

    private const int ECARTEMENT = 600;
    private const int MAX_SLIDER_SIZE = 500;
    
    void Start()
    {
        
    }

    public void ClippingToggle(bool newValue)
    {
        cp = map.GetClippingPlanes();
        planets = map.GetPlanets();
        for (int i = 0; i < planets.Length; i++)
        {
            planets[i].EnableClippingPlane(newValue);
            cp[i].gameObject.SetActive(newValue);
        }

    }

    public void RealignAll()
    {
        RefreshPositions(invisiblePlanets, visiblePlanets);
    }

   

    public void TemperatureToggle(bool newValue)
    {
        PlanetsToggle(0,newValue);
    }

    public void SpeedToggle(bool newValue)
    {
        PlanetsToggle(1,newValue);
    }

    public void SalinityToggle(bool newValue)
    {
        PlanetsToggle(2,newValue);
    }

    public void MixToggle(bool newValue)
    {
        PlanetsToggle(3,newValue);

    }

    private void PlanetsToggle(int index, bool newValue)
    {
        planets = map.GetPlanets();
        invisiblePlanets = map.GetDict(0);
        visiblePlanets = map.GetDict(1);
        planets[index].Hide(!newValue);
        planets[index].SetHiddenStatus(!newValue);

        shiftPlanets(newValue, index);
    }

    private void shiftPlanets(bool toggleVal, int indiceOfPlanet)
    {
        if (!toggleVal)
        {
            invisiblePlanets[indiceOfPlanet] = planets[indiceOfPlanet];
            visiblePlanets.Remove(indiceOfPlanet);
            RefreshPositions(invisiblePlanets, visiblePlanets);
        }
        else
        {
            visiblePlanets[indiceOfPlanet] = planets[indiceOfPlanet];
            invisiblePlanets.Remove(indiceOfPlanet);
            RefreshPositions(invisiblePlanets, visiblePlanets);
        }
    }

    private void RefreshPositions(SortedDictionary<int, PointMap> invisible, SortedDictionary<int, PointMap> visible)
    {
        int i = 0;

        cp = map.GetClippingPlanes();

        if(circleViewType)
        {
            float radius = 700f;
            foreach (int key in visible.Keys)
            {
                float angle = i * Mathf.PI * 2f / 8 - Mathf.PI;
                Vector3 newPos = new Vector3(Mathf.Cos(-angle) * radius, 0, Mathf.Sin(-angle) * radius);
                visible[key].transform.position = newPos;
                cp[key].transform.position = newPos;
                i++;
            }
            foreach (int key in invisible.Keys)
            {
                float angle = i * Mathf.PI * 2f / 8 - Mathf.PI ;
                Vector3 newPos = new Vector3(Mathf.Cos(-angle) * radius, 0, Mathf.Sin(-angle) * radius);
                invisible[key].transform.position = newPos;
                cp[key].transform.position = newPos;
                i++;
            }
        }
        else
        {
            foreach (int key in visible.Keys)
            {
                visible[key].transform.position = new Vector3(ECARTEMENT * (i-1), 0, 700f);
                cp[key].transform.position = new Vector3(ECARTEMENT * (i - 1), 0, 700f);
                i++;
            }
            foreach (int key in invisible.Keys)
            { 
                invisible[key].transform.position = new Vector3(ECARTEMENT * (i-1), 0, 700f);
                cp[key].transform.position = new Vector3(ECARTEMENT * (i - 1), 0, 700f);
                i++;
            }
        }
        
    }

    public void SetViewType(bool value)
    {
        circleViewType = value;
        RefreshPositions(invisiblePlanets, visiblePlanets);
        PointMap[] pointMaps = map.GetPlanets();
        GameObject[] planetsLabel = map.GetLabels();
        for (int i = 0; i < pointMaps.Length; i++)
        {
            planetsLabel[i].transform.position = pointMaps[i].transform.position;
            planetsLabel[i].transform.Translate(Vector3.up * (-300));
            planetsLabel[i].transform.forward = planetsLabel[i].transform.position;
            ////RESET PLANET LABELS
        }
    }
}
