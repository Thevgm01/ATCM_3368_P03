using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShotText : MonoBehaviour
{
    public Action<Collider> trigger = delegate { };

    void OnTriggerEnter(Collider other)
    {
        trigger?.Invoke(other);
    }
}
