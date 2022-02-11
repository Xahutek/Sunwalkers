using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLightdrinkerSwarm : Enemy
{
    [Header("Shoot")]
    public AudioClip shootClip;
    public Projectile ShootPrefab;
    public int 
        shootDamage = 5,
        shootAmmo = 5;
    public float
        shootRange = 25f,
        shootHitbox = 0.1f,
        shootReload = 0.5f,
        shootProjectileSpeed = 17,
        shootCooldownTime = 5;
    private float shootCooldownTimer;

    [Header("Scatter")]
    public Animator scatterAnim;
    public int
        scatterDamage = 3;
    public float
        scatterDuration = 4,
        scatterHitbox = 3,
        scatterCooldownTime = 10;
   [SerializeField] private float scatterCooldownTimer;
    bool  wasInCombat;

    public override State ReconciderState()
    {
        NextAttackInfo = new AttackInfo();
        nextStateActiveTime = 0;
        State newState = null;        
        
        if (Globe.time >= shootCooldownTimer)
        {
            newState = new ShootProjectileAttackState();
            NextAttackInfo.SetUpAll(this, DataBase.Entities.LMWagon, position, position, null, shootDamage, shootRange, shootHitbox, shootProjectileSpeed, shootAmmo, shootReload, 0, 0, true, ShootPrefab, null, null, null);
            NextAttackInfo.SetUpAudio(shootClip,shootClip);
            if (newState.CheckState(this, NextAttackInfo))
            {
                nextStateActiveTime = NextAttackInfo.reload * 4;
                shootCooldownTimer = Globe.time + shootCooldownTime + shootReload * (shootAmmo+1);
                return newState;
            }
        }

        nextStateActiveTime = 2.5f;
        newState = new NavStalkCaravanState();
        return newState;
    }
    public override void ChangeState(State newState)
    {
        base.ChangeState(newState);
        scatterAnim.SetBool("GO", false);
    }
    public override bool Hit(HittableGlobeEntity attacker, int value)
    {
        if (Globe.time >= scatterCooldownTimer && attacker &&
           (/*attacker == Caravan.main.Wagons[0] || */(attacker.position - position).magnitude < scatterHitbox))
        {
            StopAllCoroutines();
            NextAttackInfo = new AttackInfo();

            State newState = new ScatterSwarmAttackState();
            NextAttackInfo.SetUpAll(
                this, DataBase.Entities.LMWagon, position, position, null, scatterDamage, scatterHitbox, scatterHitbox, scatterDuration,
                0, 0, 0, 0, true, ShootPrefab, null, null, null);

            nextStateActiveTime = scatterDuration;
            scatterCooldownTimer = Globe.time + scatterCooldownTime * Random.Range(0.85f, 1.15f);
            ChangeState(newState);
            scatterAnim.SetBool("GO", true);
            return false;
        }

        return base.Hit(attacker, value);
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (isAlive)
        {
            if (inCombat!=wasInCombat)
                SoundManager.main.SwarmPlays = inCombat;
            wasInCombat = inCombat;
        }
    }
    public override void Die(HittableGlobeEntity attacker, bool drop = true)
    {
        base.Die(attacker, drop);

        if (wasInCombat)
            SoundManager.main.SwarmPlays = false;
        wasInCombat = false;
    }
}
