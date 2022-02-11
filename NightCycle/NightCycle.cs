using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightCycle : MonoBehaviour
{
    public static NightCycle main;
    public bool move=true;

    public float cycleDuration_Minutes;
    public float totalCycles
    {
        get
        {
            return (currentTime + ((Globe.deltaTime * (360 / cycleDuration_Minutes / 60)) / 360));
        }
    }
    [Range(0,1)] public float currentTime=0.5f;

    private void Awake()
    {
        main = this;
    }

    private void Update()
    {
        if (Globe.isCounting)
        {
            if(move)
                currentTime = (currentTime + ((Globe.deltaTime * (360 / cycleDuration_Minutes / 60)) / 360)) % 1;

            float angle = currentTime*360;

            transform.rotation = Quaternion.Euler(angle,0,0);
        }
    }

}
