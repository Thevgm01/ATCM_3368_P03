using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderRotator : MonoBehaviour
{
    public float spinSpeed;
    float curAngle = 0.0f;

    Transform inner;
    Transform outer;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.name == "Inner") inner = child;
            else if (child.name == "Outer") outer = child;
        }
    }

    // Update is called once per frame
    void Update()
    {
        curAngle += spinSpeed * Time.deltaTime;
        if (curAngle >= 180.0f) curAngle -= 180.0f;
        Vector3 angle = new Vector3(0, 0, curAngle);
        inner.localRotation = Quaternion.Euler(-angle);
        outer.localRotation = Quaternion.Euler(angle);
    }
}
