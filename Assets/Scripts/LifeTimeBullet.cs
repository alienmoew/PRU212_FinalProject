using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeTimeBullet : MonoBehaviour
{
    public float Time;

    private void Start()
    {
        Destroy(this.gameObject, Time);
    }
}
