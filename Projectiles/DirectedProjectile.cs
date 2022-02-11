using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectedProjectile : Projectile
{
    List<HittableGlobeEntity> hitEntities = new List<HittableGlobeEntity>(), lastHitEntities = new List<HittableGlobeEntity>();
    bool hitEnvironment;
    public override IEnumerator ExecuteShot(AttackInfo shot)
    {

        StartShot(shot);

        if (shot.target)
            shot.destination = Globe.Ground(shot.target.position);

        hitEnvironment = false;
        hitEntities = new List<HittableGlobeEntity>();
        lastHitEntities = new List<HittableGlobeEntity>();
        bool hasHit = false;

        float time = Vector3.Distance(shot.origin, shot.destination) / shot.SpeedExecution;
        float timer = 0, i, windUp;
        while (timer <= time)
        {
            timer += Globe.deltaTime;
            i = Mathf.Clamp01(timer / time);
            windUp = Mathf.Clamp01(timer / shot.windUpTime);

            Vector3 newPos = Globe.Ground(Vector3.Lerp(shot.origin, shot.destination, i));

            transform.position = newPos;

            body.transform.position = transform.position + Globe.Up(transform.position) * ACTrajectory.Evaluate(i);

            yield return null;

            for (int h = 0; h < lastHitEntities.Count; h++)
            {
                HittableGlobeEntity validTarget = lastHitEntities[h];

                if (!shot.LMHittable.Contains(validTarget.gameObject.layer))
                {
                    hitEntities.Add(validTarget);
                    lastHitEntities.Remove(validTarget);
                    continue;
                }

                if (validTarget != null)
                {
                    float damageModifier = GetDamageModifier(shot, timer, time, Vector3.Distance(shot.origin, shot.destination) * i);
                    hasHit = true;
                    if (shot.splashRadius > 0.1f)
                    {
                        SplashDamage(shot, damageModifier);
                        yield return new WaitForSeconds(1);
                    }
                    else
                        validTarget.Hit(shot.attacker,Mathf.RoundToInt(shot.Damage * damageModifier));

                    hitEntities.Add(validTarget);
                    lastHitEntities.Remove(validTarget);
                    h--;
                }
            }

            if ((hasHit && shot.blockable) || hitEnvironment)
                break;
        }

        if (!hasHit && shot.splashRadius > 0f)
        {
            SplashDamage(shot);
            yield return new WaitForSeconds(1);
        }

        EndShot(shot);
    }

    private void OnTriggerEnter(Collider other)
    {
        HittableGlobeEntity validTarget = other.transform.gameObject.GetComponent<HittableGlobeEntity>();
        if (validTarget != null && !hitEntities.Contains(validTarget)&&!lastHitEntities.Contains(validTarget))
        {
            lastHitEntities.Add(validTarget);
            return;
        }

        hitEnvironment = DataBase.Entities.LMEnvironment.Contains(other.gameObject.layer);
    }

    //public override IEnumerator ExecuteShot(AttackInfo shot)
    //{
    //    yield return new WaitForEndOfFrame();

    //    if (!shot.target)
    //    {
    //        EndShot();

    //        yield break;
    //    }

    //    StartShot();

    //    bool hasHit = false;

    //    Debug.DrawLine(shot.origin, shot.destination, Color.red, 2);

    //    float time = Vector3.Distance(shot.origin, shot.destination) / shot.speed;
    //    float timer = 0, i;
    //    while (timer <= time)
    //    {
    //        timer += Globe.deltaTime;
    //        i = timer / time;

    //        transform.position= Vector3.Lerp(shot.origin, shot.destination, i);

    //        Globe.Orientate(transform);

    //        yield return null;

    //        Collider[] hitCol
    //            = Physics.OverlapSphere(transform.position, 0.1f, shot.LMHittable),
    //            environment
    //            = Physics.OverlapSphere(transform.position, 0.1f, DataBase.Entities.LMEnvironment);

    //        if (hitCol.Length > 0)
    //        {
    //            HittableGlobeEntity validTarget = hitCol[0].transform.gameObject.GetComponent<HittableGlobeEntity>();
    //            if (validTarget != null)
    //            {
    //                hasHit = true;
    //                if (shot.splashRadius > 0.1f)
    //                {
    //                    SplashDamage(shot);
    //                    yield return new WaitForSeconds(1);
    //                }
    //                else
    //                    validTarget.Hit(shot.Damage);
    //            }
    //        }

    //        if ((hasHit && !shot.pierceTargets) || environment.Length > 0)
    //            break;
    //    }

    //    if (!hasHit && shot.splashRadius > 0.1f)
    //    {
    //        SplashDamage(shot);
    //        yield return new WaitForSeconds(1);
    //    }

    //    EndShot();
    //}
}
