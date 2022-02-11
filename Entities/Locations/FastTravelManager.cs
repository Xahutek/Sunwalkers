using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastTravelManager : MonoBehaviour
{
    public static FastTravelManager main;

    public Transform
        HajabimbimPort,
        KorNabelQuerry;

    private void Awake()
    {
        main = this;
    }

    public void TeleportCaravanToHajabimbim()
    {
        Caravan.main.position = HajabimbimPort.position;
    }
    public void TeleportCaravanToKorNabel()
    {
        Caravan.main.position = KorNabelQuerry.position;
    }
}
