using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/Alchemist/Basic")]
public class CardAlchemist : Card
{

    public override void OnAttack(AttackInfo shot)
    {
    }
    public override void OnHit(GlobeEntity target, GlobeEntity attacker, float damage)
    {
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
