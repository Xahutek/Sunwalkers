using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetedProjectile : Projectile
{
    //Projectile that is locked onto a target and follows it until it arrives for a guaranteed hit.

    public override IEnumerator ExecuteShot(AttackInfo shot)
    {

        if (!shot.target)
        {
            EndShot(shot);
            yield break;
        }

        StartShot(shot);

        shot.destination = Globe.Ground(shot.target.position);

        float time = Vector3.Distance(shot.origin, shot.destination)/shot.Speed;
        float timer = 0, i;
        while (timer <= time)
        {
            timer += Globe.deltaTime;
            i = Mathf.Clamp01(timer / time);

            Vector3 newPos = Globe.Ground(Vector3.Lerp(shot.origin, shot.destination, i));

            transform.position = newPos;

            body.transform.position = transform.position+Globe.Up(transform.position) * ACTrajectory.Evaluate(i);

            yield return null;

            shot.destination = Globe.Ground(shot.target.position);
            time = Vector3.Distance(shot.origin, shot.destination) / shot.Speed;
        }

        if (shot.target != null)
        {
            if (shot.splashRadius > 0.1f)
            {
                SplashDamage(shot);
                yield return new WaitForSeconds(1);
            }
            else
                shot.target.Hit(shot.attacker,shot.Damage);
        }

        EndShot(shot);
    }
}
