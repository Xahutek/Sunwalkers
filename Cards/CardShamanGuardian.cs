using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/Shaman/Guardian")]
public class CardShamanGuardian : Card
{
    public float LowestShieldGainOnHit = 3;
    public override void OnAttack(AttackInfo shot)
    {
    }
    public override void OnHit(GlobeEntity target, GlobeEntity attacker, float damage)
    {
        if (attacker != null && attacker is Wagon)
        {
            Wagon w = attacker as Wagon;
            if (w.MasterCard == this)
                Caravan.main.GetLowestWagon().shieldHealth += LowestShieldGainOnHit + (1* Level);
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
