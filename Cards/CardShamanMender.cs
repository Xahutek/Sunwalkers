using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/Shaman/Mender")]
public class CardShamanMender : Card
{
    public HealthPickUp healthPickUpPrefab;
    public LayerMask LMEnemiesAndPickUps;
    [Range(0,1)]public float 
        healthPickUpProbability,
        healthPickUpMargin;
    private int missed;

    public override void OnAttack(AttackInfo shot)
    {
        if (shot.attacker != null && shot.attacker is Wagon)
        {
            Wagon w = shot.attacker as Wagon;
            if (w.MasterCard == this)
                shot.LMHittable=LMEnemiesAndPickUps;
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
        if (target is Enemy)
        {
            if (Random.value - missed * 0.25f < healthPickUpProbability)
            {
                missed = 0;
                HealthPickUp h =
                    Instantiate(healthPickUpPrefab, Globe.Ground(target.position), Quaternion.identity, Projectile.Pool.transform);
                h.healMargin = healthPickUpMargin+(0.1f*Level);
            }
            else
                missed++;
        }
    }
    public override void OnHeal(GlobeEntity target, float amount)
    {
    }
}
