using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public float minCrosshairSpinSpeed,
                 maxCrosshairSpinSpeed;
    float curCrosshairAngle = 0.0f;

    bool hover = false;
    public Vector3 hoverInnerScale;
    public Vector3 hoverOuterScale;
    public float hoverAnimationDuration;
    float hoverAnimation = 0;

    Transform inner;
    Transform outer;

    public Camera textCam;
    public LayerMask textMask;

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
        transform.position = Input.mousePosition;

        Ray r = textCam.ScreenPointToRay(Input.mousePosition);
        hover = Physics.Raycast(r, 100.0f, textMask);
        //Debug.DrawLine(r.origin, hit.point, Color.white, 0.1f);

        if (hover)
        {
            if (hoverAnimation < hoverAnimationDuration) hoverAnimation += Time.deltaTime;
            if (hoverAnimation > hoverAnimationDuration) hoverAnimation = hoverAnimationDuration;
        }
        else
        {
            if (hoverAnimation > 0) hoverAnimation -= Time.deltaTime;
            if (hoverAnimation < 0) hoverAnimation = 0;

            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
                curCrosshairAngle += maxCrosshairSpinSpeed * Time.deltaTime;
            else
                curCrosshairAngle += minCrosshairSpinSpeed * Time.deltaTime;
        }

        if (curCrosshairAngle >= 90.0f) curCrosshairAngle -= 90.0f;

        float hoverAnimationFraction = hoverAnimation / hoverAnimationDuration;
        inner.localScale = Vector3.Lerp(Vector3.one, hoverInnerScale, hoverAnimationFraction);
        inner.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(-curCrosshairAngle, -45, hoverAnimationFraction));
        outer.localScale = Vector3.Lerp(Vector3.one, hoverOuterScale, hoverAnimationFraction);
        outer.localRotation = Quaternion.Euler(new Vector3(0, 0, -45) * hoverAnimationFraction);
    }

}
