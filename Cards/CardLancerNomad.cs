using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/Lancer/Nomad")]
public class CardLancerNomad : Card
{
    [Range(0, 1)] public float OnRamSpeedMalus = 1;
    public override void OnAttack(AttackInfo shot)
    {
    }
    public override void OnHit(GlobeEntity target, GlobeEntity attacker, float damage)
    {
    }
    public override void OnRam(GlobeEntity target, GlobeEntity attacker, float damage)
    {
        if (target!=null&&target is Enemy)
        {
            Enemy e = target as Enemy;
            e.speedMalus += OnRamSpeedMalus;
        }
    }
    public override void OnKill(GlobeEntity target, GlobeEntity attacker)
    {
    }
    public override void OnHeal(GlobeEntity target, float amount)
    {
    }
}
