using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AstarAI))]
public class Enemy : StateMachineGlobeEntity
{
    [Header("Components")]
    [Header("Enemy")]
    public float movementSpeed=3;
    public AttackInfo NextAttackInfo=null;
    protected Animator anim;
    [HideInInspector] public AstarAI NavAI;
    [SerializeField]protected bool inCombat=false;

    Vector3 lastFixedPosition;
    public virtual bool InCombat
    {
        get
        {
            return inCombat;
        }
        set
        {
            inCombat = value;

            //if (inCombat)
            //    ChangeState(ReconciderState());
            //else
            //{
            //    nextStateActiveTime = 0;
            //    ChangeState(new State());
            //}
        }
    }
    public override bool EntityEnabeled
    {
        get => base.EntityEnabeled;
        set
        {
            if (!value)
                InCombat = value;
            base.EntityEnabeled = value;
        }
    }

    [HideInInspector]public Vector3 respawnPoint;
    protected override void Awake()
    {
        base.Awake();
        anim = body.GetComponentInChildren<Animator>();
        NavAI = GetComponent<AstarAI>();
        respawnPoint = position;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        NavAI.speed = movementSpeed * (1 - speedMalus);

        if (!inCombat)
        {
            if(NavAI.targetPosition!=respawnPoint)
            NavAI.targetPosition = respawnPoint;

            if (unitHealth < TrueMaxHealth())
                Heal(Mathf.CeilToInt(TrueMaxHealth() * Time.deltaTime));
        }

        if (anim)
            anim.SetBool("Moving", !NavAI.reachedEndOfPath);

    }

    public void SetNavMeshDestination(Vector3 destination)
    {
        NavAI.targetPosition = destination;
        NavAI.targetObject = null;
    }

    public override AttackInfo GetAttackInformation()
    {
        AttackInfo info = NextAttackInfo;
        if (info == null)
        {
            info = new AttackInfo();
            info.SetUpAll(this, DataBase.Entities.LMWagon, position,position,null, 5,5,0.1f,5,0,1);
        }
        return info;
    }
    public override State ReconciderState()
    {
        return new NavChaseCaravanState(); // DETERMINE State here
    }
    public override void ChangeState(State newState)
    {
        if (!InCombat)
        {
            nextStateActiveTime = 0.5f;
            newState = new State();
        }
        if (newState != null)
            newState.myEnemy = this;
        if (NavAI)
        {
            NavAI.targetObject = null;
            NavAI.targetPosition = position;
        }//Nullify Navigation so it doesent move in the next state
        base.ChangeState(newState);
    }
    public void Respawn(bool soft=false)
    {
        InCombat = false;
        position = respawnPoint;
    }

    public override void Die(HittableGlobeEntity attacker, bool drop = true)
    {
        NavAI.stop = true;
        base.Die(attacker, drop);
    }
}
