using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NonstopDebateText : MonoBehaviour
{
    public Vector3 prePosition,
                   preRotation,
                   startPosition,
                   startRotation,
                   forceOverTime,
                   rotationOverTime;

    public float preDuration,
                 duration,
                 postDuration;

    float awakeTimer;

    TextMeshPro[] textMeshes;

    // Start is called before the first frame update
    void Start()
    {
        textMeshes = GetComponentsInChildren<TextMeshPro>();

        duration += preDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if(awakeTimer < preDuration)
        {
            float fraction = awakeTimer / preDuration;
            transform.localPosition = Vector3.Lerp(prePosition, startPosition, fraction);
            transform.localRotation = Quaternion.Euler(Vector3.Lerp(preRotation, startRotation, fraction));
            foreach(var tm in textMeshes)
                tm.alpha = fraction;
        }
        else if(awakeTimer < duration)
        {
            transform.localPosition = startPosition - forceOverTime * (awakeTimer - preDuration);
            transform.localRotation = Quaternion.Euler(startRotation - rotationOverTime * (awakeTimer - preDuration));
        }

        if(awakeTimer > duration)
        {
            float fraction = 1.0f - (awakeTimer - duration) / postDuration;
            foreach (var tm in textMeshes)
                tm.alpha = fraction;
        }

        awakeTimer += Time.deltaTime;
    }
}
