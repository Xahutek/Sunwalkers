using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    public bool isActive=false;
    protected float activeTime=0;
    public float chargeTimer = 1, chargeReload = 0.1f;

    public StateMachineGlobeEntity entity;
    public Wagon myWagon=null;
    public Enemy myEnemy=null;

    public virtual void EnterState(StateMachineGlobeEntity newEntity,float newActiveTime)
    {
        isActive = true;
        activeTime = (newActiveTime > 0 ? Globe.time : 0) + newActiveTime;
        entity = newEntity;
    }
    public virtual void LeaveState()
    {

    }
    public virtual void UpdateStateMovement()
    {

    }
    public virtual void UpdateStateActions()
    {

    }
    public virtual void FixedUpdateState()
    {
        if (isActive && activeTime != 0 && Globe.time >= activeTime)
            isActive = false;
    }
    public virtual bool CheckState(StateMachineGlobeEntity newEntity, AttackInfo attack)
    {
        return false;
    }
}

#region AttackStates
public class AttackState : State
{
    public AttackInfo shot;
    protected bool isOneShot = false, isOnAmmo=false, didOneShot=false;
    public override void EnterState(StateMachineGlobeEntity newEntity, float newActiveTime)
    {
        base.EnterState(newEntity,newActiveTime);

        shot = entity.GetAttackInformation();
        shot.target = SetTarget();

        chargeReload = shot.reload;
        isOneShot = shot.reload == 0 || (shot.ammo==1);
        isOnAmmo = shot.ammo > 0;
        chargeTimer = isOneShot? 0: Globe.time + chargeReload;
    }
    public override void LeaveState()
    {
        shot.PlayEffect(false);

        if (shot.anim)
            shot.anim.SetTrigger("Recover");
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
        shot.origin = entity.position+entity.transform.up;

        if (isActive && Globe.time >= chargeTimer && !didOneShot)
            EngageAttack();
    }

    public virtual void EngageAttack()
    {
        shot = entity.GetAttackInformation();
        shot.target = SetTarget();

        if (shot.target)
        {
            if (!isOneShot)
            {
                chargeReload = shot.reload * Random.Range(0.9f, 1.1f);
                chargeTimer = Globe.time + chargeReload;
                if (isOnAmmo)
                {
                    shot.ammo--;
                    if (shot.ammo <= 0)
                        isActive = false;
                }
            }
            else
                didOneShot=true;

            shot.PlayEffect(true);

            if (shot.anim)
                shot.anim.SetTrigger("Anticipate");

            CombatEventManager.main.Attack(shot);
            Attack();
        }
    }
    public virtual void Attack() //THE MAIN THING TO OVERRIDE
    {
        Projectile.Pool.TargetProjectile().Shoot(shot);
    }

    public virtual HittableGlobeEntity SetTarget()
    {
        Enemy fe = Caravan.main.focusedEnemy;
        if (fe)
        {
            if (CheckTarget(fe))
                return fe;
            return null;
        }

        Collider[] hitCol = Physics.OverlapSphere(shot.origin, shot.range, shot.LMHittable);
        if (hitCol.Length > 0)
        {
            HittableGlobeEntity validTarget = hitCol[0].transform.gameObject.GetComponent<HittableGlobeEntity>();
            if (validTarget != null)
                return validTarget;
        }
        return null;
    }

    public virtual bool CheckTarget(HittableGlobeEntity e)
    {
        Collider[] hitCol = Physics.OverlapSphere(shot.origin, shot.range, shot.LMHittable);
        if (hitCol.Length > 0)
        {
            HittableGlobeEntity validTarget = hitCol[0].transform.gameObject.GetComponent<HittableGlobeEntity>();
            if (validTarget == e)
                return true;
        }
        return false;
    }

    public override bool CheckState(StateMachineGlobeEntity newEntity, AttackInfo attack)
    {
        entity = newEntity;
        shot = attack;
        shot.target = SetTarget();

        return shot.target;
    }
}

public class ArrowAttackState : AttackState
{
    public override void Attack()
    {
        Projectile.Pool.TargetProjectile(shot.projectilePrefab as TargetedProjectile).Shoot(shot);
    }
}
public class SplashAttackState : AttackState
{
    public override void Attack()
    {
        Debug.Log("SplashAttack");
        shot.destination = shot.target.position;
        SpotProjectile sp= Projectile.Pool.SpotProjectile(shot.projectilePrefab as SpotProjectile);
        sp.Shoot(shot);
    }

}
public class AreaAttackState : AttackState
{
    public override void Attack()
    {
        Debug.Log("AreaAttack");
        shot.target=entity;
        shot.destination = entity.position;
        BurstProjectile sp = Projectile.Pool.BurstProjectile(shot.projectilePrefab as BurstProjectile);
        sp.Shoot(shot);
    }

    public override HittableGlobeEntity SetTarget() //DONT FORGET TO TAKE FOCUSED ENEMY INTO ACCOUNT SOMEWHERE?
    {
        Collider[] hitCol = Physics.OverlapSphere(shot.origin, shot.range, shot.LMHittable);

        if (hitCol.Length > 0)
        {
            HittableGlobeEntity validTarget = hitCol[0].transform.gameObject.GetComponent<HittableGlobeEntity>();
            if (validTarget != null && validTarget != entity)
            return validTarget;
        }
        return null;
    }
    public HittableGlobeEntity[] GiveTargets()
    {
        Collider[] hitCol = Physics.OverlapSphere(shot.origin, shot.range, shot.LMHittable);

        List<HittableGlobeEntity> hits = new List<HittableGlobeEntity>();

        foreach (Collider col in hitCol)
        {
            HittableGlobeEntity validTarget = col.transform.gameObject.GetComponent<HittableGlobeEntity>();
            if (validTarget != null && validTarget != entity)
                hits.Add(validTarget);
        }
        return hits.ToArray();
    }
}

//Manebat
public class DiveDownAttackState : AttackState
{
    public override void Attack()
    {
        entity.StartCoroutine(ExecuteDiveDown(new AttackInfo(shot)));
    }

    public IEnumerator ExecuteDiveDown(AttackInfo shot)
    {
        Debug.Log("Dive");
        AstarAI NavAI = myEnemy.NavAI;
        Transform Body = entity.body;

        shot.destination = shot.target.position;

        NavAI.Recalculate(shot.destination);
        NavAI.stop = true;

        entity.rb.velocity = Vector3.zero;
        entity.col.enabled = false;

        entity.MakeInvulnerable(shot.SpeedAnticipation+shot.SpeedExecution);

        Vector3 dir = Globe.Direct(shot.origin, shot.destination) * Globe.deltaTime * shot.Speed;
        entity.rotation = Quaternion.FromToRotation(Vector3.forward, dir);

        if (shot.anim)
            shot.anim.SetTrigger("Execute");

        float timer, time, i;

        Vector3 localstartBodyPos = Body.localPosition, startBodyPos = Body.position, upBodyPos = entity.position+Globe.Up(entity.position) * 10;

        timer = 0; time = shot.SpeedAnticipation * 0.5f;
        while (timer <= time)
        {
            timer += Globe.deltaTime;
            i = timer / time;

            Body.position = Vector3.Lerp(startBodyPos, upBodyPos, i);

            yield return null;
        }

        shot.destination = Vector3.zero;
        for (float f = 0; f < 1; f -= 0.1f)
        {
            bool breakme = false;
            for (float l = 0; l < 1; l += 0.1f)
            {
                Vector3 tryPos =
                    Globe.Ground(
                        shot.target.position
                        + (shot.target.transform.forward + shot.target.transform.right * Mathf.Lerp(-0.5f, 0.5f, l)).normalized * 5 * f);
                Collider[] sampleEnvironment = Physics.OverlapSphere(tryPos, shot.hitbox*0.5f, DataBase.Entities.LMEnvironment);

                if (sampleEnvironment.Length==0)
                {
                    shot.destination = Globe.Ground(tryPos + Random.insideUnitSphere.normalized * 2);
                    breakme = true;
                }
            }
            if (breakme)
                break;
        }
        if (shot.destination == Vector3.zero)
            shot.destination = Globe.Ground(shot.target.position + Random.insideUnitSphere.normalized * 2);

        Body.localScale = Vector3.zero;
        Body.position = entity.position + localstartBodyPos;

        entity.position = shot.destination;
        Globe.Orientate(entity.transform);

        shot.PlayImpactEffect(true);

        yield return new WaitForSeconds(shot.SpeedAnticipation * 0.5f);

        upBodyPos = entity.position + Globe.Up(entity.position)*10;
        startBodyPos = Body.position;

        Body.position = upBodyPos;
        Body.localScale = Vector3.one;

        if (shot.anim)
            shot.anim.SetTrigger("Execute");

        timer = 0; time = shot.SpeedExecution;
        while (timer <= time)
        {
            timer += Globe.deltaTime;
            i = timer / time;

            Body.position = Vector3.Lerp(upBodyPos, startBodyPos, i);

            yield return null;
        }

        Body.position = startBodyPos;

        Collider[] hitCol = Physics.OverlapSphere(shot.destination, shot.hitbox, shot.LMHittable);
        foreach (Collider col in hitCol)
        {
            HittableGlobeEntity targetHit = col.transform.gameObject.GetComponent<HittableGlobeEntity>();
            if (targetHit != null)
            {
                targetHit.Hit(shot.attacker,Mathf.RoundToInt(shot.Damage));
                Globe.main.mainCamControl.Shake();
            }

            yield return null;
        }

        entity.col.enabled = true;

        if (shot.anim)
            shot.anim.SetTrigger("Afterstun");

        yield return new WaitForSeconds(shot.SpeedAfterstun);

        NavAI.stop = false;
        isActive = false;
    }

    public override void LeaveState()
    {
        base.LeaveState();
        entity.body.gameObject.SetActive(true);
    }

    public override HittableGlobeEntity SetTarget()
    {
        HittableGlobeEntity closestTarget = Caravan.main.Wagons[0];

        return Vector3.Distance(closestTarget.position,entity.position)<shot.range?closestTarget:null;
    }
}
public class DashThroughAttackState : AttackState
{
    public override void Attack()
    {
        entity.StartCoroutine(ExecuteDashThrough(new AttackInfo(shot)));
    }
    public IEnumerator ExecuteDashThrough(AttackInfo shot)
    {
        Debug.Log("DASH");
        Rigidbody rb = entity.rb;
        AstarAI NavAI = myEnemy.NavAI;

        shot.target = Caravan.main.GetFurthestWagon(entity.position);
        shot.destination = Globe.Ground(shot.origin+(shot.target.position-shot.origin).normalized*shot.range*1.5f);

        NavAI.Recalculate(shot.destination);
        NavAI.stop = true;

        rb.velocity = Vector3.zero;

        Debug.DrawLine(shot.origin, shot.destination, Color.red, 5);
        Debug.DrawLine(shot.origin, shot.origin+ Globe.Up(shot.origin), Color.yellow, 5);
        Debug.DrawLine(shot.destination, shot.destination+Globe.Up(shot.destination), Color.yellow, 5);

        Vector3 dir = Globe.Direct(shot.origin, shot.destination) * Globe.deltaTime * shot.Speed;
        entity.rotation = Quaternion.FromToRotation(Vector3.forward, dir);

        yield return new WaitForSeconds(shot.SpeedAnticipation);


        if (shot.anim)
            shot.anim.SetTrigger("Execute");

        List<HittableGlobeEntity> hitTargets = new List<HittableGlobeEntity>();

        float time = Globe.time + Vector3.Distance(shot.destination, shot.origin) / shot.Speed*1.5f;
        entity.MakeInvulnerable(time);

        while (entity.position != shot.destination && Globe.time < time)
        {
            float distanceLeft = Vector3.Distance(entity.position,shot.destination);
            dir = Globe.Direct(shot.origin, shot.destination) * Globe.deltaTime * shot.Speed;
            if (distanceLeft < dir.magnitude)
                dir = dir.normalized * distanceLeft;
            entity.transform.position += dir;
            Globe.Orientate(entity.transform);

            yield return null;

            entity.rotation = Quaternion.FromToRotation(Vector3.forward, dir);


            Collider[] hitCol
                = Physics.OverlapSphere(entity.position, shot.hitbox, shot.LMHittable),
                environment
                = Physics.OverlapSphere(entity.position, 1, DataBase.Entities.LMEnvironment);

            foreach (Collider col in hitCol)
            {
                HittableGlobeEntity validTarget = col.transform.gameObject.GetComponent<HittableGlobeEntity>();
                if (validTarget != null && validTarget != entity && !hitTargets.Contains(validTarget))
                {
                    validTarget.Hit(shot.attacker, shot.Damage);
                    hitTargets.Add(validTarget);
                }
            }

            if (environment.Length > 0)
                break;
        }

        entity.TakeInvulnerable();
        NavAI.Recalculate(entity.position);
        NavAI.stop = true;

        if (shot.anim)
            shot.anim.SetTrigger("Afterstun");

        yield return new WaitForSeconds(shot.SpeedAfterstun);
        Debug.Log("End Dash");

        NavAI.stop = false;
        isActive = false;
    }

    public override HittableGlobeEntity SetTarget()
    {
        HittableGlobeEntity closestTarget = Caravan.main.GetClosestWagon(entity.position);

        return closestTarget&&Vector3.Distance(closestTarget.position, entity.position) < shot.range ? closestTarget : null;
    }
}

//Yakbat
public class StampedeChaseAttackState : AttackState
{
    float originalSpeed;

    public override void Attack()
    {
        entity.StartCoroutine(ExecuteStampedeChase(new AttackInfo(shot)));
    }
    public IEnumerator ExecuteStampedeChase(AttackInfo shot)
    {
        Debug.Log("Chase");
        Rigidbody rb = entity.rb;
        AstarAI NavAI = myEnemy.NavAI;

        rb.velocity = Vector3.zero;

        shot.target = Caravan.main.GetClosestWagon(entity.position);
        Vector3 dir = shot.target.position - entity.position;

        float finalDashDistance =5;
        originalSpeed= NavAI.speed;

        NavAI.speed = shot.Speed;
        NavAI.targetObject = shot.target.transform;
        NavAI.stop = false;

        bool shakeScreen=false;

        Collider[] hitCol, environment;

        bool chasing = true, hitWall = false;
        float timer = 0, time = shot.SpeedAnticipation, i;
        while (timer<=time)
        {
            Debug.DrawLine(entity.position+Globe.Up(entity.position)*0.5f, shot.destination+ Globe.Up(shot.destination)*0.5f, Color.red);
            Debug.DrawLine(entity.position+Globe.Up(entity.position), entity.position +dir.normalized*Mathf.Min(finalDashDistance,dir.magnitude) + Globe.Up(shot.destination), Color.green);

            timer += Globe.deltaTime;
            i = timer / (time * 0.5f);

            if (chasing)
            {
                //finalDashDistance = Mathf.Min(finalDashDistance + 0.75f * Globe.deltaTime, shot.range); //increase final lockOn range
                shot.destination = shot.target.position; //Refresh Target to chase it
                dir = shot.target.position - entity.position;

                NavAI.speed = Mathf.Lerp(originalSpeed,shot.Speed,i);
            }
            else 
            {
                float distanceLeft = Vector3.Distance(entity.position, shot.destination);
                dir = Globe.Direct(shot.origin, shot.destination) * Globe.deltaTime * shot.Speed;
                if (distanceLeft < dir.magnitude)
                    dir = dir.normalized * distanceLeft;
                entity.transform.position += dir;
                entity.rotation = Quaternion.FromToRotation(Vector3.forward, dir);

                Globe.Orientate(entity.transform);

                if (distanceLeft <= 0.1f)
                    break;
            }

            yield return null;

            if (chasing && dir.magnitude < finalDashDistance) //Lock on final destination when in range
            {
                Debug.Log("Executing Final Dash");
                chasing = false;

                timer = 0; timer = shot.SpeedAnticipation * 0.33f;

                NavAI.stop = true;

                shot.Speed = NavAI.speed * 1.25f;
                shot.origin = entity.position;
                shot.destination = Globe.Ground(shot.origin + (shot.target.position - shot.origin).normalized * finalDashDistance*1.5f);

                if (shot.anim)
                    shot.anim.SetTrigger("Execute");
            }

            hitCol
                = Physics.OverlapSphere(entity.position, shot.hitbox, shot.LMHittable);
            environment
                = Physics.OverlapSphere(entity.position, 1, DataBase.Entities.LMEnvironment);

            foreach (Collider col in hitCol)
            {
                HittableGlobeEntity validTarget = col.transform.gameObject.GetComponent<HittableGlobeEntity>();
                if (validTarget != null && validTarget != entity)
                {
                    validTarget.Hit(shot.attacker, shot.Damage);
                    validTarget.KnockBack(validTarget.position-entity.position,1.5f);
                    shakeScreen = true;
                }
            }

            if (!chasing && environment.Length > 0) 
            {
                Debug.Log("Stampede Into Wall");
                entity.Hit(shot.attacker, shot.Damage);
                hitWall = true;
                ThrowUpRocks(5,dir,shot);
                shot.SpeedAfterstun *= 2;
                break;
            }

            if (hitCol.Length > 0)
                break;
        }

        if (!hitWall)
            ThrowUpRocks(3, dir, shot);

        NavAI.stop = true;

        if (shot.anim)
            shot.anim.SetTrigger("Afterstun");

        shot.PlayImpactEffect(true);
        hitCol = Physics.OverlapSphere(entity.position, shot.hitbox, shot.LMHittable);

        foreach (Collider col in hitCol)
        {
            HittableGlobeEntity validTarget = col.transform.gameObject.GetComponent<HittableGlobeEntity>();
            if (validTarget != null && validTarget != entity)
            {
                validTarget.Hit(shot.attacker, shot.Damage);
                shakeScreen = true;
            }
        }

        if (shakeScreen)
            Globe.main.mainCamControl.Shake();

        Debug.Log("End Stampede");
        yield return new WaitForSeconds(shot.SpeedAfterstun);
        Debug.Log("Recover from Stampede");

        NavAI.speed = originalSpeed;
        NavAI.Recalculate(entity.position);
        NavAI.stop = false;
        isActive = false;
    }

    public void ThrowUpRocks(int amount, Vector3 direction, AttackInfo shot)
    {
        AttackInfo s = new AttackInfo();
        s.SetUpAll(shot.attacker,DataBase.Entities.LMHittable, entity.position, entity.position, null, 3, 0, 0, 5, 0, 0, 1);

        for (int i = 0; i < amount; i++)
        {
            float power = Random.value;

            s.destination =
                entity.position + (direction.RotateOnAxis(Random.Range(-90, 90), Globe.Up(entity.position)).normalized
                * Mathf.Lerp(3, 5, Random.value));
            s.Damage = Mathf.RoundToInt(Mathf.Lerp(1, 3, power));
            s.Speed= Mathf.Lerp(5, 8, power);

            Debug.DrawLine(entity.position+Globe.Up(entity.position),s.destination + Globe.Up(s.destination), Color.cyan,5);

            Projectile p = shot.projectilePrefab ? Projectile.Pool.Projectile(shot.projectilePrefab) : Projectile.Pool.SpotProjectile();

            p.Shoot(s);
        }
    }

    public override void LeaveState()
    {
        base.LeaveState();
        myEnemy.NavAI.speed = originalSpeed;
    }
    public override HittableGlobeEntity SetTarget()
    {
        HittableGlobeEntity closestTarget = Caravan.main.GetClosestWagon(entity.position);
        if (!closestTarget||!entity)
            return null;
        return Vector3.Distance(closestTarget.position, entity.position) < shot.range ? closestTarget : null;
    }
}

//Lightdrinker
public class ScatterSwarmAttackState : AttackState
{
    public override void EnterState(StateMachineGlobeEntity newEntity, float newActiveTime)
    {
        Debug.Log("Arrived at State");

        base.EnterState(newEntity, newActiveTime);

        entity.rb.velocity = Vector3.zero;
        myEnemy.NavAI.stop = true;
    }
    public override void LeaveState()
    {
        Debug.Log("Left State");

        base.LeaveState();
        myEnemy.NavAI.stop = false;
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }
    public override void EngageAttack()
    {
        base.EngageAttack();
    }
    public override void Attack()
    {
        Debug.Log("Arrived at Attack");

        entity.StartCoroutine(ExecuteScatter(shot));
    }

    public IEnumerator ExecuteScatter(AttackInfo shot)
    {
        entity.col.enabled = false;

        entity.MakeInvulnerable(shot.Speed);

        Wagon engine = Caravan.main.Wagons[0];
        CaravanInventory inventory = Caravan.main.inventory;

        Debug.Log("Arrived at Executable");

        float timer = 0, hitTimer=0, time = shot.Speed, i;
        while (timer <= time)
        {
            timer += Globe.deltaTime;
            hitTimer += Globe.deltaTime;
            i = timer / time;

            Debug.Log("Hit Timer = "+hitTimer);

            if (hitTimer >= 0.25)
            {
                hitTimer -= 0.25f;
                Collider[] hitCol = Physics.OverlapSphere(shot.origin, shot.hitbox, shot.LMHittable);
                foreach (Collider col in hitCol)
                {
                    HittableGlobeEntity targetHit = col.transform.gameObject.GetComponent<HittableGlobeEntity>();
                    if (targetHit is Wagon)
                    {
                        targetHit.Hit(shot.attacker, Mathf.RoundToInt(shot.Damage * 0.25f));
                        inventory.Amber -= 0.333f*0.25f;
                    }

                    yield return null;
                }
            }

            yield return null;
        }

        entity.TakeInvulnerable();
        entity.col.enabled = true;
        isActive = false;
    }
    public override HittableGlobeEntity SetTarget()
    {
        HittableGlobeEntity engine = Caravan.main.Wagons[0];

        //if ((engine.position - entity.position).magnitude > 2)
        //    return null;

        return engine;
    }
}
public class ShootProjectileAttackState : AttackState
{
    public bool ChickenOut = true;
    public override void EnterState(StateMachineGlobeEntity newEntity, float newActiveTime)
    {
        base.EnterState(newEntity, newActiveTime);
        entity.rb.velocity = Vector3.zero;
        myEnemy.NavAI.stop = true;
    }
    public override void LeaveState()
    {
        base.LeaveState();
        myEnemy.NavAI.stop = false;
    }
    public override void FixedUpdateState()
    {
            base.FixedUpdateState();

        if (ChickenOut && (Caravan.main.Wagons[0].position - entity.position).magnitude < 2)
            isActive = false;
    }
    
    public override void Attack()
    {
        shot.target = Caravan.main.GetFurthestWagon(entity.position);

        Vector3 dir = (shot.target.position - shot.origin).normalized;
        shot.destination = Globe.Ground(shot.origin + dir * shot.range*2);

        Projectile.Pool.Projectile(shot.projectilePrefab).Shoot(shot);
    }

    public override HittableGlobeEntity SetTarget()
    {
        HittableGlobeEntity closestTarget = Caravan.main.GetClosestWagon(entity.position);

        if (Vector3.Distance(closestTarget.position, entity.position) > shot.range * 0.65f)
            return null;

        Vector3
            start = entity.position + entity.transform.up * 2f,
            direction = (closestTarget.position + entity.transform.up * 2) - start;
        float distance = direction.magnitude;

        RaycastHit hit;
        if (Physics.Raycast(start,direction,out hit, distance, DataBase.Entities.LMEnvironment))
            return null;

        return closestTarget;
    }
}

#endregion

#region MovementState

public class NavMovementState : State //Used by Enemies
{
    public AstarAI NavAI= null;

    public override void EnterState(StateMachineGlobeEntity newEntity, float newActiveTime)
    {
        base.EnterState(newEntity, newActiveTime);

        if (!myEnemy || !myEnemy.NavAI)
        {
            isActive = false;
            return;
        }
        NavAI = myEnemy.NavAI;
    }
    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        if (!isActive || !NavAI)
        {
            NavAI.targetPosition = entity.position;
            return;
        }

        NavAI.targetObject = null;
        NavAI.targetPosition = GetDestination();

    }

    public virtual Vector3 GetDestination() //THE MAIN THING TO OVERRIDE
    {
        return Caravan.main.Wagons[0].position;
    }
}
public class NavChaseCaravanState : NavMovementState
{
    public override Vector3 GetDestination()
    {
        return Caravan.main.GetClosestWagon(entity.position).position;
    }
}

public class NavStalkCaravanState : NavMovementState
{
    public override Vector3 GetDestination()
    {
        Vector3
            wagon = Caravan.main.GetClosestWagon(entity.position).position,
            direction = wagon - entity.position,
            counterDirection = Globe.Ground(entity.position - direction.normalized * 3f);

        return direction.magnitude > 6 ? wagon : entity.position;
    }
}

#endregion
