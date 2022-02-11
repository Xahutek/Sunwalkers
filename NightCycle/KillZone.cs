using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    public LayerMask LMWagon;

    private void OnTriggerEnter(Collider other)
    {
        if (LMWagon.Contains(other.gameObject.layer) && other.gameObject == Caravan.main.Wagons[0].gameObject)
            Caravan.main.inNightfall = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (LMWagon.Contains(other.gameObject.layer) && other.gameObject == Caravan.main.Wagons[0].gameObject)
            Caravan.main.inNightfall = false;
    }
}
