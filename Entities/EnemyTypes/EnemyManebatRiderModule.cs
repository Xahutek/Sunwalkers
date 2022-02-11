using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManebatRiderModule : Enemy
{
    public EnemyManebat manebat;

    [Header("Shoot")]
    public Projectile ShootPrefab;
    public int
        shootDamage = 7,
        shootAmmo = 5;
    public float
        shootRange = 25f,
        shootHitbox = 0.1f,
        shootReload = 0.2f,
        shootProjectileSpeed = 18,
        shootCooldownTime = 5;
    private float shootCooldownTimer;


    protected override void Start()
    {
        base.Start();
        NavAI.enabled = false;
    }
    public override State ReconciderState()
    {
        NextAttackInfo = new AttackInfo();
        nextStateActiveTime = 0;
        State newState = null; 
        
        if (Globe.time >= shootCooldownTimer)
        {
            newState = new ShootProjectileAttackState();
            (newState as ShootProjectileAttackState).ChickenOut = false;
            NextAttackInfo.SetUpAll(manebat, DataBase.Entities.LMWagon, manebat.position, manebat.position, null, shootDamage, shootRange, shootHitbox, shootProjectileSpeed, shootAmmo, shootReload, 0, 0, true, ShootPrefab, null, null, null);
            if (newState.CheckState(manebat, NextAttackInfo))
            {
                nextStateActiveTime = NextAttackInfo.reload * 4;
                shootCooldownTimer = Globe.time + shootCooldownTime + shootReload * (shootAmmo + 1);
                return newState;
            }
        }

        nextStateActiveTime = 2.5f;
        newState = new State();
        return newState;
    }
    protected override void Update()
    {
        if (InCombat && manebat.UpdateStates && manebat.isAlive && Caravan.main.isAlive && !manebat.isInvulnerable)
        {
            currentState.UpdateStateMovement();
            currentState.UpdateStateActions();
        }
    }
    protected override void FixedUpdate()
    {
        InCombat = manebat.InCombat;
        if (InCombat && manebat.UpdateStates && manebat.isAlive && Caravan.main.isAlive && !manebat.isInvulnerable)
        {
            currentState.FixedUpdateState();

            if (!currentState.isActive)
            ChangeState(ReconciderState());
        }
    }

}
