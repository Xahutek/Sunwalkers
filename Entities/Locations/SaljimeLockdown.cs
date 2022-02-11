using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SaljimeLockdown : MonoBehaviour
{
    public CityProfile SaljimeCity;
    public PointOfInterest SaljimePointOfInterest;
    public TMP_Text StatusText;
    public bool inLockdown;
    [Range(0,1)]public float 
        totalTime,
        timeTillLockdown,
        lockdownTime;
    private void Start()
    {
        lockdownTime =
            0.25f - 0.05f * (SaljimeCity.reputationLevel.costModifier - 1);
    }
    private void FixedUpdate()
    {
        totalTime = NightCycle.main.currentTime;
         inLockdown =
            totalTime > lockdownTime;
        timeTillLockdown =
            Mathf.Max(0, lockdownTime - totalTime);

        SaljimePointOfInterest.forceDisable = inLockdown;

        float timer =
            timeTillLockdown * NightCycle.main.cycleDuration_Minutes;
        float minutes = Mathf.Floor(timer);
        float seconds = Mathf.Floor((timer - minutes) * 100);
        string m = "0", s = "0";
        if (minutes < 10)
        {
            m = "0" + minutes.ToString();
        }
        if (seconds < 10)
        {
            s = "0" + Mathf.RoundToInt(seconds).ToString();
        }
        StatusText.text =
            (inLockdown ? "Closed" : "Open \n" +
            m + ":" + s);
    }
}
