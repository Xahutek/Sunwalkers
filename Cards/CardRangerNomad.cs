using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/Ranger/Nomad")]
public class CardRangerNomad : Card
{
    public int OnSlowedBonusDamage = 2;
    public override void OnAttack(AttackInfo shot)
    {
    }
    public override void OnHit(GlobeEntity target, GlobeEntity attacker, float damage)
    {
        if (attacker != null && target != null && attacker is Wagon && target is Enemy)
        {
            Wagon w = attacker as Wagon;
            Enemy e = target as Enemy;
            if (w.MasterCard == this && e.isSlowed)
                e.Hit(null, OnSlowedBonusDamage * Level);
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
