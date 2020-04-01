using UnityEngine;

public class PhysicsPointer : MonoBehaviour
{
    public float defaultLength = 3.0f;

    public GameObject go;
    public OVRInput.Controller controller = OVRInput.Controller.All;
    public GameObject inGameCanvas;

    private LineRenderer lineRenderer;// = null;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        Debug.Log(lineRenderer != null);
        go = new GameObject();
    }

    private void Update()
    {
        UpdateLength();
    }

    private void UpdateLength()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, CalculateEnd());
    }

    private Vector3 CalculateEnd()
    {
        RaycastHit hit = CreateForwardRaycast();
        Vector3 endPosition = DefaultEnd(defaultLength);


        if (hit.collider)
        {
            Debug.Log(hit.collider.tag);
            endPosition = hit.point;
            if (hit.collider.tag == "planet")
            {
                go = hit.transform.gameObject;
            }
            else if (hit.collider.tag == "graph")
            {
                go = hit.transform.gameObject;
            }
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                Debug.Log("Call trigger down");
                go.transform.SendMessage("OnTriggerIsDown", 1);
            }
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger,OVRInput.Controller.LTouch))
            {
                Debug.Log("Call trigger second down");
                go.transform.SendMessage("OnTriggerIsDown", 2);
            }
            if (OVRInput.GetDown(OVRInput.RawButton.RHandTrigger))
            {
                Debug.Log("Call righthandtrigger second down");
                go.transform.parent.SendMessage("OnHandTriggerIsDown", hit);
            }

        }
        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            Debug.Log("Call trigger Up");
            if (go.tag == "planet")
            {
                go.transform.SendMessage("OnTriggerIsUp", 1);
            }
        }
        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
        {
            Debug.Log("Call trigger Up");
            if (go.tag == "planet")
            {
                go.transform.SendMessage("OnTriggerIsUp", 2);
            }
        }
        if (OVRInput.GetUp(OVRInput.RawButton.RHandTrigger))
        {
            Debug.Log("Call righthandtrigger second up");
            go.transform.SendMessage("OnHandTriggerIsUp");
        }

        return endPosition;
    }

    private RaycastHit CreateForwardRaycast()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);

        if (OVRInput.GetDown(OVRInput.Button.Two, controller))
        {
            bool activeStatus = inGameCanvas.activeSelf;
            inGameCanvas.SetActive(!activeStatus);
            inGameCanvas.transform.position = ray.origin + ray.direction * 2;
            inGameCanvas.transform.forward = ray.direction;
        }

        Physics.Raycast(ray, out hit, defaultLength);
        return hit;
    }

    private Vector3 DefaultEnd(float length)
    {
        return transform.position + (transform.forward * length);
    }
}
