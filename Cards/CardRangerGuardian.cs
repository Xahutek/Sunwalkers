using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/Ranger/Guardian")]
public class CardRangerGuardian : Card
{
    public float
        maxShieldedDamage,
        maxShieldedReload;

    public override void OnAttack(AttackInfo shot)
    {
        if (shot.attacker != null && shot.attacker is Wagon)
        {
            Wagon w = shot.attacker as Wagon;
            if (w.MasterCard == this)
            {
                buffAttackDamage = Mathf.CeilToInt(Mathf.Max(buffAttackDamage, Mathf.Lerp(buffAttackDamage, maxShieldedDamage * Level, w.shieldHealth / w.maxShieldHealth)));
                shot.fluidDamage = AttackDamage;
            }
        }
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
