using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineGlobeEntity : HittableGlobeEntity
{
    [Header("State Machine")]
    public bool UpdateStates = true;
    public State currentState=null;
    protected float nextStateActiveTime = 3;

    public virtual void ChangeState(State newState)
    {
        if (body)
        {
            body.position = transform.TransformPoint(localBodyPos);
            body.rotation = transform.rotation ;
        }
        TakeInvulnerable();

        if (currentState != null)
            currentState.LeaveState();
        currentState = newState;
        if (currentState != null)
            currentState.EnterState(this,nextStateActiveTime);

        //Debug.Log(name+" entered "+newState);
    }
    public virtual State ReconciderState()
    {
        return new State();
    }

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
        ChangeState(ReconciderState());
    }
    protected override void Update()
    {
        base.Update();
        UpdateState();
    }
    public virtual void UpdateState()
    {
        if (UpdateStates && isAlive && Caravan.main.isAlive) 
        {
            currentState.UpdateStateMovement();
            currentState.UpdateStateActions();
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (UpdateStates && isAlive && Caravan.main.isAlive)
        {
            currentState.FixedUpdateState();

            if (!currentState.isActive)
                ChangeState(ReconciderState());
        }
    }

    public virtual AttackInfo GetAttackInformation()
    {
        AttackInfo info = new AttackInfo();
        return info;
    }

    public virtual bool EntityEnabeled
    {
        get
        {
            return gameObject.activeSelf;
        }
        set
        {
            gameObject.SetActive(value);
        }
    }
}
