using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstProjectile : Projectile
{

    public override IEnumerator ExecuteShot(AttackInfo shot)
    {
        StartShot(shot);

        StartCoroutine(AlignPosition(shot));
        transform.localScale = Vector3.one*shot.splashRadius;

        HittableGlobeEntity[] hits = GiveTargets(shot);

        SplashDamage(shot);

        yield return new WaitForSeconds(0.5f);

        EndShot(shot);
    }
    public IEnumerator AlignPosition(AttackInfo shot)
    {
        GlobeEntity locus= shot.attacker?shot.attacker:(shot.target?shot.target:null);
        if (locus==null)
            yield break;
        float time = Globe.time + 0.5f;
        while (Globe.time<=time)
        {
            transform.position = locus.position;
            yield return null;
        }
    }

    public HittableGlobeEntity[] GiveTargets(AttackInfo shot)
    {
        Collider[] hitCol = Physics.OverlapSphere(shot.origin, shot.splashRadius, shot.LMHittable);

        List<HittableGlobeEntity> hits = new List<HittableGlobeEntity>();

        foreach (Collider col in hitCol)
        {
            HittableGlobeEntity validTarget = col.transform.gameObject.GetComponent<HittableGlobeEntity>();
            if (validTarget != null)
                hits.Add(validTarget);
        }
        return hits.ToArray();
    }
}
