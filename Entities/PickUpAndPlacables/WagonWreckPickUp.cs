using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WagonWreckPickUp : PickUp
{
    CaravanInventory.WagonProfile myWagon;
    public void SetUp(CaravanInventory.WagonProfile profile)
    {
        myWagon = profile;
    }

    public override void PickMeUp(GameObject pickingObject)
    {
        if (myWagon == null)
            return;

        myWagon.bonusSlots = 0;
        myWagon.bonusHealthBuffs = 0;

        //if ((Random.value > 0.5f || myWagon.bonusHealthBuffs == 0) && myWagon.bonusSlots > 0)
        //    myWagon.bonusSlots = Mathf.Max(0, myWagon.bonusSlots - Random.Range(0, 2));
        //else if (myWagon.bonusHealthBuffs > 0)
        //    myWagon.bonusHealthBuffs = Mathf.Max(0, myWagon.bonusSlots - Random.Range(0, 2));
    }
    public override void FadeAway()
    {
        if (myWagon == null)
            return;

        myWagon.bonusSlots = 0;
        myWagon.bonusHealthBuffs = 0;

        foreach (Card card in myWagon.Cards)
        {
            if (card)
                card.CutHighest();
        }
        myWagon.ClearCards();
    }
}