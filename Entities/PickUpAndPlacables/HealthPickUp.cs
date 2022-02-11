using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : PickUp
{
    [Range(0, 1)] public float healMargin;

    public override void PickMeUp(GameObject pickingObject)
    {
        Wagon lowestWagon = Caravan.main.GetLowestWagon();

        lowestWagon.Heal(lowestWagon.TrueMaxHealth() * healMargin);
    }
}
