using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabelHandler : MonoBehaviour
{
    Quaternion initRotation;
    // Start is called before the first frame update
    void Awake()
    {
        initRotation = this.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 campos = Camera.main.transform.position;
        transform.LookAt(transform.position);
    }

    void CompensateRotation(Vector3 rotation)
    {
        this.transform.Rotate(initRotation.eulerAngles);
        this.transform.forward = this.transform.position ;
    }
}
