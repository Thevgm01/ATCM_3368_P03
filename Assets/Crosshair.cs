using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public float minCrosshairSpinSpeed,
                 maxCrosshairSpinSpeed;
    float curCrosshairAngle = 0.0f;

    Transform inner;

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in transform)
        {
            if (child.name == "Inner") inner = child;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;

        if(Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            curCrosshairAngle += maxCrosshairSpinSpeed * Time.deltaTime;
        else 
            curCrosshairAngle += minCrosshairSpinSpeed * Time.deltaTime;

        inner.rotation = Quaternion.Euler(0, 0, -curCrosshairAngle);
    }
}
