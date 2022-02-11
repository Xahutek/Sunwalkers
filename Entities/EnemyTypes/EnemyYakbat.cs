using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyYakbat : Enemy
{
    [Header("Stampede")]
    public SpotProjectile rockScatter;
    public ParticleSystem StampedeEffect,StampedeImpactEffect;
    public AudioClip StampedeRelease, StampedeImpact;
    public int stampedeDamage = 8;
    public float
        stampedeRange = 10,
        stampedeHitbox = 2,
        stampedeMaxChaseTime = 5f,
        stampedeSpeed = 10,
        stampedeAfterstunTime = 1f,
        stampedeCooldownTime = 10f;
    private float stampedeCooldownTimer;
    bool isGalloping,wasGalloping;

    protected override void Awake()
    {
        base.Awake();
        stampedeCooldownTimer = 3;
    }
    public override State ReconciderState()
    {
        NextAttackInfo = new AttackInfo();
        nextStateActiveTime = 0;
        State newState = null;

        if (Globe.time >= stampedeCooldownTimer)
        {
            newState = new StampedeChaseAttackState();
            NextAttackInfo.SetUpAll(this,DataBase.Entities.LMWagon, position, Vector3.zero, null, stampedeDamage, stampedeRange, stampedeHitbox, 1, 0, 0, 0,0, false, rockScatter, StampedeEffect,StampedeImpactEffect, anim);
            NextAttackInfo.SetUpRichSpeed(stampedeMaxChaseTime, stampedeSpeed, stampedeAfterstunTime);
            NextAttackInfo.SetUpAudio(StampedeRelease,StampedeImpact);
            if (newState.CheckState(this, NextAttackInfo))
            {
                stampedeCooldownTimer = Globe.time + stampedeCooldownTime;
                return newState;
            }
        }

        nextStateActiveTime = 2.5f;
        newState = new NavChaseCaravanState();
        return newState;
    }
    public override void ChangeState(State newState)
    {
        isGalloping = false;
        base.ChangeState(newState);
        isGalloping = currentState is StampedeChaseAttackState;
    }


    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (isAlive)
        {
            if (isGalloping != wasGalloping)
                SoundManager.main.GallopPlays = isGalloping;
            wasGalloping = isGalloping;
        }
    }
    public override void Die(HittableGlobeEntity attacker, bool drop = true)
    {
        base.Die(attacker, drop);

        isGalloping = false;
        if (isGalloping != wasGalloping)
            SoundManager.main.GallopPlays = isGalloping;
        wasGalloping = false;
    }
}
