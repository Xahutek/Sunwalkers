using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmine : Placable
{
    SphereCollider col;
    public ParticleSystem splashParticles;

    private void OnTriggerEnter(Collider other)
    {
        if (active && shot.LMHittable.Contains(other.gameObject.layer))
            Explode();
    }

    public override void Remove()
    {        
        if (active)
            Explode(false);
        base.Remove();

    }

    public void Explode(bool remove=true)
    {
        splashParticles.Play();
        shot.PlayImpactEffect(true);

        Collider[] hitCol = Physics.OverlapSphere(transform.position, shot.splashRadius, shot.LMHittable);
        foreach (Collider col in hitCol)
        {
            HittableGlobeEntity targetHit = col.transform.gameObject.GetComponent<HittableGlobeEntity>();
            if (targetHit != null)
            {
                targetHit.Hit(shot.attacker, shot.Damage);
            }
        }
        if (remove)
            Remove();
    }
}
