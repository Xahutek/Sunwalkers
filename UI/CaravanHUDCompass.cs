using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CaravanHUDCompass : MonoBehaviour
{
    public CameraController mainCam;
    public NightCycle mainNight;
    public Transform miniGlobe, nightShade, caravanMarker;
    public TMP_Text nightfallTimerText;

    private void FixedUpdate()
    {
        if (Globe.isCounting)
        {
            //float
            //    totalTime = 1 - (NightCycle.main.currentTime + 0.75f) % 1,
            //    caravanRotation = mainCam.HorizontalRotator.rotation.eulerAngles.x;
            //if (caravanRotation < 0) caravanRotation = (360 - caravanRotation) % 360;
            //float
            //    caravanTime = (caravanRotation / 360f);
            //float timeTillNightfall =
            //    (totalTime + caravanTime) %1;
            //float timer =
            //    timeTillNightfall * NightCycle.main.cycleDuration_Minutes;
            //float minutes = Mathf.Floor(timer);
            //float seconds = Mathf.Floor((timer - minutes) * 100);
            //string m = "0", s = "0";
            //if (minutes < 10)
            //{
            //    m = "0" + minutes.ToString();
            //}
            //if (seconds < 10)
            //{
            //    s = "0" + Mathf.RoundToInt(seconds).ToString();
            //}
            //nightfallTimerText.text =
            //    m + ":" + s;
            //nightfallTimerText.text = (Mathf.Floor((caravanTime) *100)/100).ToString();


            caravanMarker.localRotation = mainCam.PolarRotator.localRotation;
            miniGlobe.localRotation = mainCam.HorizontalRotator.localRotation;
            nightShade.localRotation = mainNight.transform.rotation;
        }
    }
}
