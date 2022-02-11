using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KorLibetLockdown : MonoBehaviour
{
    public CityProfile KorLibetCity;
    public PointOfInterest KorLibetPointOfInterest;
    public TMP_Text StatusText;
    public GameObject Blockade;
    public GameObject ArenaOff, ArenaOn;
    public bool inLockdown;

    private void Start()
    {
        Refresh();
    }
    private void FixedUpdate()
    {
        if (KorLibetCity.ReputationLerp <= 0)
        {
            inLockdown = true;
            Refresh();
        }
    }
    public void Refresh()
    {
        StatusText.text = !inLockdown ? "Open" : "Hostile";
        Blockade.SetActive(inLockdown);
        KorLibetPointOfInterest.forceDisable = inLockdown;

            ArenaOff.SetActive(!inLockdown);

            ArenaOn.SetActive(inLockdown);
    }
}
