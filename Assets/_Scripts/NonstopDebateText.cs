using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class NonstopDebateText : MonoBehaviour
{
    public Character speaker;

    public event Action Finished = delegate { };

    public Vector3 prePositionRelative,
                   preRotationRelative,
                   forceOverTime,
                   rotationOverTime;
    Vector3 mainPosition,
            mainRotation;

    public float shakeMagnitude;
    float lastShakeX, lastShakeY;
    public float tempShake;

    public float preDuration,
                 duration,
                 postDuration;

    float awakeTimer;

    TextMeshPro[] textMeshes;

    // Start is called before the first frame update
    void Awake()
    {
        mainPosition = transform.localPosition;
        mainRotation = transform.localRotation.eulerAngles;
        textMeshes = GetComponentsInChildren<TextMeshPro>();
        duration += preDuration;
    }

    public void OnEnable()
    {
        awakeTimer = 0f;
        SetAlpha(0f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = mainPosition;
        Vector3 rotation = mainRotation;

        if (awakeTimer < preDuration)
        {
            float fraction = awakeTimer / preDuration;
            position += prePositionRelative * (1f - fraction);
            rotation += preRotationRelative * (1f - fraction);
            SetAlpha(fraction);
        }
        else if(awakeTimer > duration)
        {
            Finished?.Invoke();

            float fraction = 1f - (awakeTimer - duration) / postDuration;

            if (fraction < 0f)
            {
                gameObject.SetActive(false);
                return;
            }

            SetAlpha(fraction);
        }
        else
        {
            SetAlpha(1);
        }

        if (shakeMagnitude > 0f || tempShake > 0)
        {
            float shake = shakeMagnitude > 0 ? shakeMagnitude : tempShake;
            float randomX = UnityEngine.Random.Range(-shake, shake);
            float randomY = UnityEngine.Random.Range(-shake, shake);
            position.x += (lastShakeX + randomX) / 2f;
            position.y += (lastShakeY + randomY) / 2f;
            lastShakeX = randomX;
            lastShakeY = randomY;

            if (tempShake > 0)
            {
                tempShake -= Time.deltaTime / 10f;
            }
        }

        position += forceOverTime * (awakeTimer);
        rotation += rotationOverTime * (awakeTimer);

        transform.localPosition = position;
        transform.localRotation = Quaternion.Euler(rotation);

        awakeTimer += Time.deltaTime;
    }

    private void SetAlpha(float a)
    {
        foreach (var tm in textMeshes)
            tm.alpha = a;
    }
}
