using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placable : GlobeEntity
{
    public static ProjectilePool Pool;

    protected AttackInfo shot;

    public bool deployed,active;
    public float stayDuration;
    protected float removeTime;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (active)
        {
            if (Globe.time > removeTime)
            {
                Remove();
                return;
            }
            FixedRefresh();
        }
    }

    public virtual void SetUp(AttackInfo shot)
    {
        this.shot = shot;
        stayDuration = shot.Speed;
        removeTime = Globe.time + stayDuration;
        deployed = true;
        transform.position = shot.origin;
        gameObject.SetActive(true);
        Invoke("RealActivate", shot.windUpTime);
    }

    public virtual void FixedRefresh()
    {

    }

    public virtual void Remove()
    {
        Invoke("RealRemove",0.5f);
        active = false;
    }
    public void RealActivate()
    {
        active = true;
    }
    public void RealRemove()
    {
        deployed = false;
        active = false;
        gameObject.SetActive(false);
    }
}
