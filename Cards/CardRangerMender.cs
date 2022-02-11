using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/Ranger/Mender")]
public class CardRangerMender : Card
{
    public float damageMultiplierAfterHeal=2;
    private float lastHealTimer=float.NegativeInfinity;
    public bool isEmpowered
    {
        get { return Mathf.Abs(lastHealTimer-Globe.time)<AttackReload; }
    }
    public override void OnAttack(AttackInfo shot)
    {
        if (isEmpowered)
        {
            if (shot.attacker != null && shot.attacker is Wagon)
            {
                Wagon w = shot.attacker as Wagon;
                if (w.MasterCard == this)
                    shot.fluidDamage *= damageMultiplierAfterHeal+Level;
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
        if (target is Wagon)
        {
            Wagon w = target as Wagon;
            if (w.MasterCard == this)
                lastHealTimer = Globe.time;
        }
    }
}
