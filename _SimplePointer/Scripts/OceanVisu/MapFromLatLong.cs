﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;
using System.Linq;

public abstract class MapFromLatLong : MonoBehaviour {
	public PointMap pointMapToInstanciate ;
    protected PointMap [] pointMaps = null ;

    protected GameObject[] labelList = null;
    protected GameObject inGameCanvas;
    protected GameObject configPanel;
    protected Button OKButton;
    protected bool coroutineCalled = false;


    /*
     * Test de courbes avec points (pointgraph2) et cubes (pointgraphs)
     */
    public PointGraphs pointGraphToInstanciate;
    protected PointGraphs[] pointGraphs;
    /*
     * 
     */


    protected SortedDictionary<int,PointMap> visiblePlanets;
    protected SortedDictionary<int,PointMap> invisiblePlanets;
    
    public ClippingPlane clippingPlaneToInstantiate;
    protected ClippingPlane[] clippingBehaviors;
    protected GameObject[] clippingPlanes;

    public int nbTimeMax = 1 ;
    protected int nbTime = 0 ;
    protected int nbDepth = 0 ;
    protected int nbY = 0 ;
    protected int nbX = 0 ;

    private Rect popup;
    private bool showPopUp = true;
    private bool toggleTemp = false;
    private bool toggleSal = false;
    private bool toggleSpeed = false;
    private bool toggleMix = false;

    private GameObject tempLabel;
    private GameObject speedLabel;
    private GameObject salLabel;


    protected float salMin = 100, salMax = -1, tempMin = 100, tempMax = -100;

    protected bool isSphere = true;

    private const int ECARTEMENT = 600;
    private const int NUMBER_OF_PLANETS = 3;


    void Start () {
        //GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube) ;
        //cube.transform.parent = transform ;
        invisiblePlanets = new SortedDictionary<int, PointMap>();
        visiblePlanets = new SortedDictionary<int, PointMap>();
        inGameCanvas = GameObject.Find("Canvas");
        configPanel = GameObject.Find("Panel_config");
        labelList = new GameObject[NUMBER_OF_PLANETS];

    }

    private void OnGUI()
    {
        if (showPopUp)
        {
            configPanel.SetActive(true);
            ShowGUI(0);
        }
    }
    public IEnumerator testCreateMesh() 
    {
        bool test = false;
        int result = CreateMesh();
        float timer = 0;
        yield return new WaitForEndOfFrame();
        /*
        while (test==false) 
        {
            timer += Time.deltaTime;
            if (result == 1 || timer >20.0f) 
            {
                test = true;
                Debug.Log("temps à charger :" + timer);
                yield return null;
            }
            yield return null;
        }
        */
        GameObject.Find("ToggleTemp").GetComponent<Toggle>().isOn = toggleTemp;
        GameObject.Find("ToggleSalinity").GetComponent<Toggle>().isOn = toggleSal;
        GameObject.Find("ToggleSpeed").GetComponent<Toggle>().isOn = toggleSpeed;
        GameObject.Find("ToggleMix").GetComponent<Toggle>().isOn = toggleMix;

        //GameObject.Find("ScrollView").SetActive(false);
        showPopUp = false;
        yield return new WaitForEndOfFrame();
        inGameCanvas.transform.SendMessage("AfterConfig");
        inGameCanvas.SetActive(false);

        
        


        for (int i = 0; i < labelList.Length; i++)
        {
            labelList[i] = GameObject.Find("PlanetTag"+i.ToString());
            labelList[i].transform.position = pointMaps[i].transform.position;
            labelList[i].transform.Translate(Vector3.up * (-300));
            labelList[i].transform.forward = labelList[i].transform.position;
            
        }

        labelList[0].GetComponent<TextMesh>().text = "Temperature";
        labelList[1].GetComponent<TextMesh>().text = "Speed";
        labelList[2].GetComponent<TextMesh>().text = "Salinity";



        yield return null;
    }

    public GameObject[] GetLabels()
    {
        return labelList;
    }

    private void ShowGUI(int id)
    {
        toggleTemp = GameObject.Find("ToggleTempConf").GetComponent<Toggle>().isOn;
        toggleSal = GameObject.Find("ToggleSalinityConf").GetComponent<Toggle>().isOn;
        toggleSpeed = GameObject.Find("ToggleSpeedConf").GetComponent<Toggle>().isOn;
        toggleMix = GameObject.Find("ToggleMixConf").GetComponent<Toggle>().isOn;


        OKButton = GameObject.Find("OK").GetComponent<Button>();
        OKButton.onClick.AddListener(delegate () { CallCoroutine(); });
    }

    public void CallCoroutine()
    {
        if(!coroutineCalled)
        {
            Debug.Log("In call coroutine");
            StartCoroutine(testCreateMesh());
            coroutineCalled = true;
        }
        
    }

    public virtual int CreateMesh () {
        string path = ChooseName();

        string firstLine = System.IO.File.ReadLines(path).First();

        string[] firstDatas = firstLine.Split(',');

        int.TryParse(firstDatas[0], out nbTime);
        int.TryParse(firstDatas[1], out nbDepth);
        int.TryParse(firstDatas[2], out nbY);
        int.TryParse(firstDatas[3], out nbX);

        // to load only some time steps
        nbTime = Math.Min(nbTime, nbTimeMax);


        string[] lines = System.IO.File.ReadLines(path).Take(nbTime * nbDepth * nbY * nbX + 1).ToArray();

        pointMaps = new PointMap[NUMBER_OF_PLANETS];
        //Added
        clippingBehaviors = new ClippingPlane[NUMBER_OF_PLANETS];
        clippingPlanes = new GameObject[NUMBER_OF_PLANETS];

        /*
         * Test de courbes avec cubes (pointgraphs)
         */
        pointGraphs = new PointGraphs[1];
        pointGraphs[0] = (PointGraphs)Instantiate(pointGraphToInstanciate, new Vector3(0, 0, 0), new Quaternion());
        pointGraphs[0].initPointGroups(nbTime, nbDepth, nbY, nbX);
        pointGraphs[0].SetShader(pointGraphs[0].ChooseShader());

        Dictionary<Vector3, List<Vector3>> graphToSphere = pointGraphs[0].getPointsDict();
        Dictionary<Vector3, List<Vector3>> graphToPlane = pointGraphs[0].getPointsDictPlane();
        /*
         * 
         */



        Color incolore = new Color (0.0f, 0.0f, 0.0f, 0.0f) ;
        Shader shader = ChooseShader () ;
        MeshTopology meshTopology = ChooseTopology () ;
        CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        ci.NumberFormat.CurrencyDecimalSeparator = ".";

        // to load only some time steps
        nbTime = Math.Min (nbTime, nbTimeMax) ;

        for (int i = 0 ; i < pointMaps.Length ; i++) {
            pointMaps [i] = (PointMap)Instantiate (pointMapToInstanciate, new Vector3 (0, 0, 0), new Quaternion ()) ;
            //Added
            clippingBehaviors[i] = (ClippingPlane)Instantiate(clippingPlaneToInstantiate, new Vector3(0, 0, 0), new Quaternion());
            clippingBehaviors[i].name = "ClippingBehavior";
            clippingPlanes[i] = new GameObject();
            clippingPlanes[i].AddComponent<InteractPlane>();
            clippingPlanes[i].tag = "ClippingPlane";
            clippingPlanes[i].name = "ClippingPlane";
            pointMaps [i].initPointGroups (nbTime, nbDepth, nbY, nbX) ;
            pointMaps [i].SetShader (shader) ;
            pointMaps[i].tag = "planet";
        }




        /*
         * Test de courbes avec points (pointgraph2) et cubes (pointgraphs)
         */
        int[] indicesIndices = new int[nbX * nbY * 36];

        float cubeSize = 1.0f;
        Vector3[] offsets = {  new Vector3 ( -cubeSize, -cubeSize, -cubeSize),
                                new Vector3 ( -cubeSize, -cubeSize, +cubeSize),
                                new Vector3 ( -cubeSize, +cubeSize, -cubeSize),
                                new Vector3 ( -cubeSize, +cubeSize, +cubeSize),
                                new Vector3 ( +cubeSize, -cubeSize, -cubeSize),
                                new Vector3 ( +cubeSize, -cubeSize, +cubeSize),
                                new Vector3 ( +cubeSize, +cubeSize, -cubeSize),
                                new Vector3 ( +cubeSize, +cubeSize, +cubeSize) };
        Vector3 antipodes = new Vector3(10000.0f, 10000.0f, 10000.0f);
        /*
         * 
         */


        float sphereRadius = 250.0f ;

        // calcul du maillage : ici des points
        int [] indices = ComputeIndices (nbX, nbY) ;
        int [] indicesSlice1 = ComputeIndicesSlice1(nbX, nbY);
        

        int line = 1 ;

        Vector3[] myOldSphericalPoints = new Vector3[nbX * nbY];
        Vector3[] myOldPlanePoints = new Vector3[nbX * nbY];
        Color[][] myOldColors = new Color[pointMaps.Length][];
        for (int i = 0; i < pointMaps.Length; i++)
        {
            myOldColors[i] = new Color[nbX * nbY];
        }

        for (int t = 0 ; t < nbTime ; t++) {

            for (int d = 0 ; d < nbDepth ; d++) {
                //print ("d = " + d) ;
                int l = 0 ;



                /*
                 * Test de courbes avec points (pointgraph2) et cubes (pointgraphs)
                 */
                int indiceIndices = 0;
                int indicePoints = 0;
                Vector3[] myGraphPoints = new Vector3[nbX * nbY * 8];

                Color[] myGraphColors = new Color[nbX * nbY * 8];

                /*
                 * 
                 */


                Vector3 [] mySphericalPoints = new Vector3 [nbX * nbY] ;
                Vector3 [] myPlanePoints = new Vector3 [nbX * nbY] ;

                Color [][] myColors = new Color [pointMaps.Length][] ;
                for (int i = 0 ; i < pointMaps.Length ; i++) {
                    myColors [i] = new Color [nbX * nbY] ;
                }

                

                for (int iy = 0 ; iy < nbY ; iy++) {
                    for (int ix = 0 ; ix < nbX ; ix++) {
                        string [] currentLine = lines [line].Split (',') ;
                        int currentTime = 0 ;
                        float currentDepth = 0 ;
                        float currentLat = 0 ;
                        float currentLong = 0 ;
                        int.TryParse (currentLine [0], out currentTime) ;
                        currentDepth = float.Parse (currentLine [1], NumberStyles.Any, ci) ;
                        currentLat = (float)(float.Parse (currentLine [2], NumberStyles.Any, ci) * Math.PI / 180.0f) ;
                        currentLong = (float)(float.Parse (currentLine [3], NumberStyles.Any, ci) * Math.PI / 180.0f) ;
                        // compute spherical projection
                        float R = sphereRadius - currentDepth / 50.0f ;
                        float currentX = (float)(R * Math.Cos (currentLong) * Math.Cos (currentLat)) ;
				        float currentY = (float)(R * Math.Sin (currentLat)) ;							
				        float currentZ = (float)(R * Math.Sin (currentLong) * Math.Cos (currentLat)) ;
                        Vector3 myPoint = new Vector3 (currentX, currentY, currentZ) ;
                        // compute plane projection
                        float currentX2 = 180.0f * currentLong / (float)Math.PI ;
				        float currentY2 = (float)(180.0f * Math.Log (Math.Tan (Math.PI / 4 + currentLat / 2)) / Math.PI) ;							
                        Vector3 myPoint2 = new Vector3 (currentX2, currentY2, currentDepth/50.0f) ;

                        float temperature = 0 ;
                        temperature = float.Parse (currentLine [4], NumberStyles.Any, ci) ;
                        float salinity = 0 ;
                        salinity =  float.Parse (currentLine [5], NumberStyles.Any, ci) ;
                        float vx = 0 ;
                        vx =  float.Parse (currentLine [6], NumberStyles.Any, ci) ;
                        float vy = 0 ;
                        vy =  float.Parse (currentLine [7], NumberStyles.Any, ci) ;
                        float vz = 0 ;
                        vz =  float.Parse (currentLine [8], NumberStyles.Any, ci) ;

                        if (temperature > tempMax && temperature != 0)
                        {
                            tempMax = temperature;
                        }
                        if (temperature < tempMin && temperature != 0)
                        {
                            tempMin = temperature;
                        }
                        if (salinity > salMax && salinity != 0)
                        {
                            salMax = salinity;
                        }
                        if (salinity < salMin && salinity != 0)
                        {
                            salMin = salinity;
                        }



                        mySphericalPoints [l] = myPoint ;
                        myPlanePoints [l] = myPoint2 ;


                        // temperature
                        float tMax = 32.0f;
                        float tMin = -3.0f;
                        float tMiddle = (tMin + tMax) / 2.0f;

                        // speed
                        float vMax = 0.5f;
                        float vMin = 0.0f;
                        float vMiddle = (vMin + vMax) / 2.0f;
                        float speed = (float)Math.Sqrt(vx * vx + vy * vy + vz * vz);

                        // salinity
                        float sMax = 41.0f;
                        float sMin = 5.0f;
                        float sMiddle = (sMin + sMax) / 2.0f;




                        /*
                         * Test de courbes avec points (pointgraph2) et cubes (pointgraphs)
                         */
                        Vector3 myPoint3 = new Vector3((temperature - tMin) * 10, (salinity - sMin) * 10, 10 + d * 5f);

                        for (int i = 0; i < 8; i++)
                        {
                            myGraphPoints[indicePoints + i] = myPoint3 + offsets[i];
                        }

                        int[] indiceLocal = { 0, 2, 1,
                                              1, 2, 3,
                                              1, 7, 5,
                                              1, 3, 7,
                                              4, 7, 6,
                                              4, 5, 7,
                                              0, 6, 2,
                                              0, 4, 6,
                                              0, 1, 5,
                                              0, 5, 4,
                                              2, 7, 3,
                                              2, 6, 7 };

                        // Build the CUBE tile by submitting triangle vertices
                        int indiceGlobal = indicePoints;
                        for (int j = 0; j < 36; j++)
                        {
                            indicesIndices[indiceIndices + j] = indiceGlobal + indiceLocal[j];
                        }

                        /*
                         * 
                         */



                        // mix des 3
                        if ((salinity != 0.0f) && (speed != 0.0f) && (temperature != 0.0f)) {

                            if (graphToSphere.ContainsKey(myPoint3))
                            {
                                graphToSphere[myPoint3].Add(myPoint);
                                graphToPlane[myPoint3].Add(myPoint2);
                            }
                            else
                            {
                                graphToSphere.Add(myPoint3, new List<Vector3>());
                                graphToSphere[myPoint3].Add(myPoint);

                                graphToPlane.Add(myPoint3, new List<Vector3>());
                                graphToPlane[myPoint3].Add(myPoint2);
                            }

                            if (temperature <= tMiddle) {
                                myColors [0][l] = new Color (0.0f,
                                                          Math.Max (0, Math.Min (1, (temperature - tMin) / (tMiddle - tMin))),
                                                          Math.Max (0, Math.Min (1, (tMiddle - temperature) / (tMiddle - tMin))), 1f) ;
                            } else {
                                myColors [0][l] = new Color (Math.Max (0, Math.Min (1, (temperature - tMiddle) / (tMax - tMiddle))),
                                                          Math.Max (0, Math.Min (1, (tMax - temperature) / (tMax - tMiddle))),
                                                          0.0f, 1f) ;
                            }    
                            if (speed <= vMiddle) {
                                myColors [1][l] = new Color (0.0f,
                                                          Math.Max (0, Math.Min (1, (speed - vMin) / (vMiddle - vMin))),
                                                          Math.Max (0, Math.Min (1, (vMiddle - speed) / (vMiddle - vMin))), 1f) ;
                            } else {
                                myColors [1][l] = new Color (Math.Max (0, Math.Min (1, (speed - vMiddle) / (vMax - vMiddle))),
                                                          Math.Max (0, Math.Min (1, (vMax - speed) / (vMax - vMiddle))),
                                                          0.0f, 1f) ;
                            }    
                            if (salinity <= sMiddle) {
                                myColors [2][l] = new Color (0.0f,
                                                          Math.Max (0, Math.Min (1, (salinity - sMin) / (sMiddle - sMin))),
                                                          Math.Max (0, Math.Min (1, (sMiddle - salinity) / (sMiddle - sMin))), 1f) ;
                                for (int i = 0; i < 8; i++)
                                {
                                    myGraphColors[indicePoints + i] = new Color(0.0f,
                                                          Math.Max(0, Math.Min(1, (salinity - sMin) / (sMiddle - sMin))),
                                                          Math.Max(0, Math.Min(1, (sMiddle - salinity) / (sMiddle - sMin))), 1f);
                                }
                            } else {
                                myColors [2][l] = new Color (Math.Max (0, Math.Min (1, (salinity - sMiddle) / (sMax - sMiddle))),
                                                          Math.Max (0, Math.Min (1, (sMax - salinity) / (sMax - sMiddle))),
                                                          0.0f, 1f) ;
                                for (int i = 0; i < 8; i++)
                                {
                                    myGraphColors[indicePoints + i] = new Color(Math.Max(0, Math.Min(1, (salinity - sMiddle) / (sMax - sMiddle))),
                                                          Math.Max(0, Math.Min(1, (sMax - salinity) / (sMax - sMiddle))),
                                                          0.0f, 1f);
                                }
                            }
//######################################## NORMAL QUE ENLEVE???
                            /* myColors [3][l] = new Color (Math.Max (0, Math.Min (1, (temperature - tMin) / (tMax - tMin))),
                                                          Math.Max (0, Math.Min (1, (salinity - sMin) / (sMax - sMin))),
                                                          Math.Max (0, Math.Min (1, (speed - vMin) / (vMax - vMin))), 1f) ;*/
                        } else {
                            for (int i = 0 ; i < pointMaps.Length ; i++) {
                                myColors [i][l] = incolore ;
                            }
                            mySphericalPoints [l] = ManageNullValue (myPoint) ;
                            myPlanePoints [l] = ManageNullValue (myPoint2) ;



                            /*
                             * Test de courbes avec points (pointgraph2) et cubes (pointgraphs)
                             */
                            for (int i = 0; i < 8; i++)
                            {
                                myGraphColors[indicePoints + i] = incolore;
                                myGraphPoints[indicePoints + i] = antipodes + offsets[i];
                            }
                            /*
                             * 
                             */
                        }

                        /*
                         * Test de courbes avec points (pointgraph2) et cubes (pointgraphs)
                         */
                        indiceIndices = indiceIndices + 36;
                        indicePoints = indicePoints + 8;
                        /*
                         * 
                         */



                        l++ ;
                        lines [line] = null ;
                        line++ ;
                    }
                }


                //print ("l = " + l) ;
                //print ("line = " + line) ;

                Vector3[] myNewSphericalPoints = new Vector3[nbX * nbY * 2];
                Array.ConstrainedCopy(mySphericalPoints, 0, myNewSphericalPoints, 0, nbX * nbY);
                Array.ConstrainedCopy(myOldSphericalPoints, 0, myNewSphericalPoints, nbX * nbY, nbX * nbY);
                Vector3[] myNewPlanePoints = new Vector3[nbX * nbY * 2];
                Array.ConstrainedCopy(myPlanePoints, 0, myNewPlanePoints, 0, nbX * nbY);
                Array.ConstrainedCopy(myOldPlanePoints, 0, myNewPlanePoints, nbX * nbY, nbX * nbY);
                Color[][] myNewColors = new Color[pointMaps.Length][];
                for (int j = 0; j < pointMaps.Length; j++)
                {
                    myNewColors[j] = new Color[nbX * nbY * 2];
                    Array.ConstrainedCopy(myColors[j], 0, myNewColors[j], 0, nbX * nbY);
                    Array.ConstrainedCopy(myOldColors[j], 0, myNewColors[j], nbX * nbY, nbX * nbY);
                }

                for (int i = 0 ; i < pointMaps.Length ; i++) {
                    if (d == 0) 
                    {
                        pointMaps[i].initSlice(t, d, mySphericalPoints, myPlanePoints, indicesSlice1, myColors[i], meshTopology);
                    } 
                    else
                    {
                        pointMaps[i].initSlice(t, d, myNewSphericalPoints, myNewPlanePoints, indices, myNewColors[i], meshTopology);
                    }
                }


                /*
                 * Test de courbes avec points (pointgraph2) et cubes (pointgraphs)
                 */
                pointGraphs[0].initSlice(t, d, myGraphPoints, indicesIndices, myGraphColors, meshTopology);
                /*
                 * 
                 */


                mySphericalPoints.CopyTo(myOldSphericalPoints, 0);
                myPlanePoints.CopyTo(myOldPlanePoints, 0);
                for (int i = 0; i < pointMaps.Length; i++)
                {
                    Array.Copy(myColors[i], myOldColors[i], nbX * nbY);
                    myColors[i] = null;
                }
                mySphericalPoints = null ;
                myPlanePoints = null ;
                myColors = null ;
                GC.Collect (GC.MaxGeneration, GCCollectionMode.Forced) ;
            }
        }
        lines = null ;
        GC.Collect (GC.MaxGeneration, GCCollectionMode.Forced) ;

        for (int i = 0; i < NUMBER_OF_PLANETS; i++)
        {
            clippingBehaviors[i].SetPointMap(pointMaps[i]);
            clippingBehaviors[i].transform.SetParent(clippingPlanes[i].transform);
            ///////////////////////////////////////////////ADDED///////////////////////////////////////////////
            //Obligé de retirer le script d'interaction, qui est bizarrement ajouté quand on l'ajoute au parent. 
            Destroy(clippingBehaviors[i].GetComponent<InteractPlane>());
        }


        for (int i = 0; i < pointMaps.Length; i++)
        {
            invisiblePlanets[i] = pointMaps[i];
            pointMaps[i].transform.Translate(ECARTEMENT * i, 0, 0);
            clippingPlanes[i].transform.Translate(new Vector3(ECARTEMENT * i, 0, 0));
        }


        for (int i = 0; i < pointMaps.Length; i++)
        {
            invisiblePlanets[i] = pointMaps[i];
        }





        /*
         * Test de courbes avec points (pointgraph2) et cubes (pointgraphs)
         */
        for (int i = 0; i < pointGraphs.Length; i++)
        {
            pointGraphs[i].transform.Translate(ECARTEMENT * (i + visiblePlanets.Count), -200, 500);
        }
        pointGraphs[0].SetLabels("Température", "Salinité", (int)tempMin, (int)tempMax, (int)salMin, (int)salMax);




        for (int i = 0; i < pointMaps.Length; i++)
        {
            clippingBehaviors[i].SetPointMap(pointMaps[i]);
            clippingBehaviors[i].transform.SetParent(clippingPlanes[i].transform);
            ///////////////////////////////////////////////ADDED///////////////////////////////////////////////
            //Obligé de retirer le script d'interaction, qui est bizarrement ajouté quand on l'ajoute au parent. 
            Destroy(clippingBehaviors[i].GetComponent<InteractPlane>());
            invisiblePlanets[i] = pointMaps[i];
        }


      

        

        ActivateTime () ;
        return 1;

    }
    


    // Update is called once per frame
    void Update () {
        if(inGameCanvas.activeSelf == false)
        {
            for (int i = 0; i < pointMaps.Length; i++)
            {
                labelList[i].transform.position = pointMaps[i].transform.position;
                labelList[i].transform.Translate(Vector3.up * (-300));
                labelList[i].transform.forward = labelList[i].transform.position;
                labelList[i].SetActive(!pointMaps[i].GetHiddenStatus());
            }
        }
        

        var activateSlice = OVRInput.GetDown(OVRInput.Button.Three);//F
        if (activateSlice) {
            ActivateSlice () ;
        }
        var activateSliceInverse = OVRInput.GetDown(OVRInput.Button.Four) ;
        if (activateSliceInverse) {
            ActivateSliceInverse () ;
        }
       
        var resetSlice = OVRInput.Get(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.LTouch);
        if (resetSlice) {
            ResetSlice () ;
        }
		var activateTime = OVRInput.Get(OVRInput.RawButton.LHandTrigger); ;
        if (activateTime) {
            ActivateTime () ;
        }
		var changeToSphere = Input.GetKeyDown (KeyCode.C) ;
        if (changeToSphere) {
            ChangeToSphere () ;
        }
		var changeToPlane = Input.GetKeyDown (KeyCode.X) ;
        if (changeToPlane) {
            ChangeToPlane () ;
        }

    }

   


    /// <summary>
    /// //////////////////////////ADDED THE IFS
    /// </summary>
    void ActivateSlice () {
        for (int i = 0 ; i < pointMaps.Length ; i++) {
            if (!pointMaps[i].GetHiddenStatus())
            {
                pointMaps[i].ActivateSlice();
            } 
        }
        for (int i = 0; i < pointGraphs.Length; i++)
        {
            if (!pointGraphs[i].GetHiddenStatus())
            {
                pointGraphs[i].ActivateSlice();
            }
        }
    }

    void ActivateSliceInverse () {
        for (int i = 0 ; i < pointMaps.Length ; i++) {
            if (!pointMaps[i].GetHiddenStatus())
            {
                pointMaps[i].ActivateSliceInverse();
            }
        }
        for (int i = 0; i < pointGraphs.Length; i++)
        {
            if (!pointGraphs[i].GetHiddenStatus())
            {
                pointGraphs[i].ActivateSliceInverse();
            }
        }
    }

    void ResetSlice () {
        for (int i = 0 ; i < pointMaps.Length ; i++) {
            if (!pointMaps[i].GetHiddenStatus())
            {
                pointMaps[i].ResetSlice();
            }
        }
        for (int i = 0; i < pointGraphs.Length; i++)
        {
            if (!pointGraphs[i].GetHiddenStatus())
            {
                pointGraphs[i].ResetSlice();
            }
        }
    }

    void ActivateTime () {
        for (int i = 0 ; i < pointMaps.Length ; i++) {
            if (!pointMaps[i].GetHiddenStatus())
            {
                pointMaps[i].ActivateTime();
            }
        }
        for (int i = 0; i < pointGraphs.Length; i++)
        {
            if (!pointGraphs[i].GetHiddenStatus())
            {
                pointGraphs[i].ActivateTime();
            }
        }
    }
    /// <summary>
    /// //////////////////////////////////////
    /// </summary>
    void ChangeToSphere()
    {
        for (int i = 0; i < pointMaps.Length; i++)
        {
            pointMaps[i].ChangeToSphere();
            pointMaps[i].SetIsSphere(true);
        }
    }

    void ChangeToPlane()
    {
        for (int i = 0; i < pointMaps.Length; i++)
        {
            pointMaps[i].ChangeToPlane();
            pointMaps[i].SetIsSphere(false);
        }
    }

    public void SetClippingPlane(Vector4 vector)
    {
        for (int i = 0; i < pointMaps.Length; i++)
        {
            pointMaps[i].SetClippingPlane(vector);
        }
    }

    public PointMap[] GetPlanets()
    {
        return pointMaps;
    }

    public PointGraphs[] GetGraphs()
    {
        return pointGraphs;
    }

    public SortedDictionary<int,PointMap> GetDict(int type)
    {
        if (type == 0)
        {
            return invisiblePlanets;
        }
        else if (type == 1)
        {
            return visiblePlanets;
        }
        else return new SortedDictionary<int, PointMap>();
    }

    public ClippingPlane[] GetClippingPlanes()
    {
        return clippingBehaviors;
    }

    abstract public int [] ComputeIndices (int nbX, int nbY) ;

    abstract public int[] ComputeIndicesSlice1(int nbX, int nbY);

    abstract public Shader ChooseShader () ;

    abstract public String ChooseName () ;

    abstract public MeshTopology ChooseTopology () ;

    abstract public Vector3 ManageNullValue (Vector3 value) ;
}

