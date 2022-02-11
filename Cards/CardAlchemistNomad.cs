using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/Alchemist/Nomad")]
public class CardAlchemistNomad : Card
{
    public float
        deathSplashDamage,
        deathSplashRadius;
    public BurstProjectile
        deathProjectilePrefab;

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
        AttackInfo shot = new AttackInfo();
        shot.SetUpAll(null,
            DataBase.Entities.LMEnemy, target.position, target.position, null,
            AttackDamage, 0, deathSplashRadius, 0, 0, 0, deathSplashRadius,0, false, deathProjectilePrefab);

        CombatEventManager.main.Attack(shot);
        BurstProjectile sp = Projectile.Pool.BurstProjectile(shot.projectilePrefab as BurstProjectile);
        sp.Shoot(shot);
    }
    public override void OnHeal(GlobeEntity target, float amount)
    {
    }
}
