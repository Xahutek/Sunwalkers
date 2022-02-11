using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/Shaman/Nomad")]
public class CardShamanNomad : Card
{
    [Range(0, 1)] public float speedMalusOnHit = 0.3f;
    public override void OnAttack(AttackInfo shot)
    {
    }
    public override void OnHit(GlobeEntity target, GlobeEntity attacker, float damage)
    {
        if (attacker != null && target != null && attacker is Wagon &&target is Enemy)
        {
            Wagon w = attacker as Wagon;
            if (w.MasterCard == this)
            {
                Enemy e = target as Enemy;
                e.speedMalus += speedMalusOnHit;
            }
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
