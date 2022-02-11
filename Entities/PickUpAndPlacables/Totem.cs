using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Totem : Placable
{
    public Caravan caravan;
    public float chargeTimer;

    public override void SetUp(AttackInfo shot)
    {
        base.SetUp(shot);
        chargeTimer = 0;
        caravan = Caravan.main;
        SubscribeEvents();
    }
    public override void Remove()
    {
        UnSubscribeEvents();
        base.Remove();
    }
    public override void FixedRefresh()
    {
        base.FixedRefresh();

        if (active&&deployed && Globe.time >= chargeTimer)
            Attack();
    }

    public void Attack()
    {
        chargeTimer = Globe.time + shot.reload;

        CombatEventManager.main.Attack(shot);

        shot.destination = transform.position;
        BurstProjectile sp = Projectile.Pool.BurstProjectile(shot.projectilePrefab as BurstProjectile);

        shot.PlayEffect(true);
        sp.Shoot(shot);
    }

    public void SubscribeEvents()
    {
        CombatEventManager manager = CombatEventManager.main;

        manager.OnAttack += OnAttack;
        manager.OnHit += OnHit;
        manager.OnKill += OnKill;
    }
    public void UnSubscribeEvents()
    {
        CombatEventManager manager = CombatEventManager.main;

        manager.OnAttack -= OnAttack;
        manager.OnHit -= OnHit;
        manager.OnKill -= OnKill;
    }
    public virtual void OnAttack(AttackInfo shot)
    {
        if (shot.attacker != this) return;
        foreach (Wagon w in caravan.Wagons)
        {
            Card c = w.myProfile.Cards[0];
            if (c != null && c.keywords.Contains(Keyword.Shaman))
            {
                shot.attacker = w;
                c.OnAttack(shot);
            }
        }
    }
    public virtual void OnHit(GlobeEntity target, GlobeEntity attacker, float damage)
    {
        if (attacker != this) return;
        foreach (Wagon w in caravan.Wagons)
        {
            Card c = w.myProfile.Cards[0];
            if (c != null && c.keywords.Contains(Keyword.Shaman))
                c.OnHit(target,w,damage);
        }
    }
    public virtual void OnKill(GlobeEntity target, GlobeEntity attacker)
    {
        if (attacker != this) return;
        foreach (Wagon w in caravan.Wagons)
        {
            Card c = w.myProfile.Cards[0];
            if (c != null && c.keywords.Contains(Keyword.Shaman))
                c.OnKill(target, w);
        }
    }
}
