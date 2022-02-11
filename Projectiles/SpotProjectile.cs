using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotProjectile : Projectile
{
    public override IEnumerator ExecuteShot(AttackInfo shot)
    {

        StartShot(shot);

        if (shot.target)
        {
            shot.destination = Globe.Ground(shot.target.position);
        }
        else
            shot.destination = Globe.Ground(shot.destination);

        Debug.DrawLine(Vector3.zero, transform.position * 2, Color.cyan, 1f);
        Debug.DrawLine(Vector3.zero, shot.destination * 2, Color.cyan, 1f);


        float time = Mathf.Max(Vector3.Distance(shot.origin, shot.destination),0.5f) / shot.Speed;
        float timer = 0, i;
        while (timer <= time)
        {
            timer += Globe.deltaTime;
            i = Mathf.Clamp01(timer / time);

            Vector3 newPos = Globe.Ground(Vector3.Lerp(shot.origin, shot.destination, i));
            Debug.DrawLine(Vector3.zero,newPos*2,Color.blue,0.2f);
            transform.position = newPos;

            body.transform.position = transform.position + Globe.Up(transform.position) * ACTrajectory.Evaluate(i);

            yield return null;
        }

        if (shot.splashRadius > 0.1f)
        {
            SplashDamage(shot);
            yield return new WaitForSeconds(1);
        }

        EndShot(shot);
    }
}
