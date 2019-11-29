using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

//public enum characters { MAKOTO_NAEGI, SAYAKA_MAIZONO, KIYOTAKA_ISHIMARU, JUNKO_ENOSHIMA, SAKURA_OGAMI, KYOKO_KIRIGIRI, MONDO_OWADA, AOI_ASAHINA, YASUHIRO_HAGAKURE, CHIHIRO_FUJISAKI, BYAKUYA_TOGAMI, CELESTIA_LUDENBERG, LEON_KUWATA, TOKO_FUKAWA, HIFUMI_YAMADA }
public enum Character { MAKOTO, SAYAKA, TAKA, JUNKO, SAKURA, KYOKO, MONDO, HINA, HIRO, CHIHIRO, BYAKUYA, CELESTE, LEON, TOKO, HIFUMI }

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

    public float preDuration,
                 duration,
                 postDuration;

    float awakeTimer;

    TextMeshPro[] textMeshes;

    // Start is called before the first frame update
    void Awake()
    {
        int a = (int)Character.MAKOTO;
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

        if (shakeMagnitude > 0f)
        {
            float randomX = UnityEngine.Random.Range(-shakeMagnitude, shakeMagnitude);
            float randomY = UnityEngine.Random.Range(-shakeMagnitude, shakeMagnitude);
            position.x += (lastShakeX + randomX) / 2f;
            position.y += (lastShakeY + randomY) / 2f;
            lastShakeX = randomX;
            lastShakeY = randomY;
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
