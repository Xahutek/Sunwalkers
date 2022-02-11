using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Active Abilities/Lancer")]
public class LancerActiveAbility : ActiveAbility
{
    public AnimationCurve ACBulldozeDamageMultiuplierBonus;

    public override void Activate(Caravan c)
    {
        base.Activate(c);
        c.isBulldozing = true;
        c.BulldozeEffect.Play();
    }
    public override void Press(Caravan c)
    {
        base.Press(c);
        charge.x -= Globe.fixedDeltaTime * (c.sprintFuelSufficient ? 2 : 1);
        c.BulldozeDamageMultiplier = 1 + ACBulldozeDamageMultiuplierBonus.Evaluate(timePressed)+(0.5f*((float)powerBonus-1));
    }
    public override void Release(Caravan c)
    {
        base.Release(c);
        c.isBulldozing=false;
        c.BulldozeEffect.Stop();
    }
}
