using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/Shaman/Basic")]
public class CardShaman : Card
{
    public float FuelOnHit = 1;
    public override void OnAttack(AttackInfo shot)
    {
    }
    public override void OnHit(GlobeEntity target, GlobeEntity attacker, float damage)
    {
        if (attacker != null && attacker is Wagon)
        {
            Wagon w = attacker as Wagon;
            Caravan caravan = Caravan.main;
            if (w.MasterCard == this)
                caravan.sprintFuel = Mathf.Clamp(caravan.sprintFuel + FuelOnHit, 0, caravan.maxSprintFuel);
        }
    }
    public override void OnRam(GlobeEntity target, GlobeEntity attacker, float damage)
    {
    }
    public override void OnKill(GlobeEntity target, GlobeEntity attacker)
    {
    }
    public override void OnHeal(GlobeEntity target, float amount)
    {
    }
}
