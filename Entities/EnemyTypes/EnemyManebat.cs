using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManebat : Enemy
{
    [Header("Dash")]
    public ParticleSystem DashEffect;
    public AudioClip DashRelease, DashImpact;
    public int dashDamage = 5;
    public float
        dashRange = 5f,
        dashHitbox = 1,
        dashAnticipationTime=0.5f,
        dashSpeed=20f,
        dashAfterstunTime=1f,
        dashCooldownTime=3;
    private float dashCooldownTimer;

    [Header("Dive")]
    public ParticleSystem DiveEffect;
    public AudioClip DiveRelease, DiveImpact;
    public int diveDamage = 10;
    public float
        diveRange = 10,
        diveHitbox = 2,
        diveAnticipationTime = 1.25f,
        diveExecutionTime = 0.25f,
        diveAfterstunTime = 1f,
        diveCooldownTime = 8f;
    private float diveCooldownTimer;

    protected override void Awake()
    {
        base.Awake();
        diveCooldownTimer = 3;
        dashCooldownTimer = 5;
    }
    public override State ReconciderState()
    {
        NextAttackInfo = new AttackInfo();
        nextStateActiveTime = 0;
        State newState = null;

        if (Globe.time >= diveCooldownTimer)
        {
            newState = new DiveDownAttackState();
            NextAttackInfo.SetUpAll(this, DataBase.Entities.LMWagon, position, Vector3.zero,null,diveDamage,diveRange,diveHitbox,1,0,0,0,0,false,null,null, DiveEffect, anim);
            NextAttackInfo.SetUpRichSpeed(diveAnticipationTime, diveExecutionTime, diveAfterstunTime);
            NextAttackInfo.SetUpAudio(DiveRelease, DiveImpact);
            if (newState.CheckState(this, NextAttackInfo))
            {
                diveCooldownTimer = Globe.time + diveCooldownTime * Random.Range(0.85f, 1.15f);
                return newState;
            }
        }

        if (Globe.time >= dashCooldownTimer)
        {
            newState = new DashThroughAttackState();
            NextAttackInfo.SetUpAll(this, DataBase.Entities.LMWagon, position, position, null, dashDamage, dashRange, dashHitbox, dashSpeed, 0, 0, 0,0, false, null, DashEffect,null,anim);
            NextAttackInfo.SetUpRichSpeed(dashAnticipationTime, dashSpeed, dashAfterstunTime);
            NextAttackInfo.SetUpAudio(DashRelease, DashImpact);
            if (newState.CheckState(this, NextAttackInfo))
            {
                dashCooldownTimer = Globe.time + dashCooldownTime * Random.Range(0.85f, 1.15f);
                return newState;
            }
        }

        nextStateActiveTime = 2.5f;
        newState = new NavChaseCaravanState();
        return newState;
    }

}
