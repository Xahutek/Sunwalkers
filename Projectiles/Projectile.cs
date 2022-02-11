using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector]public float lastActiveTime = -99;
    public static ProjectilePool Pool;
    public bool isShooting;

    protected Transform body;
    protected SphereCollider col;

    public AnimationCurve ACTrajectory;
    protected TrailRenderer trail;
    public ParticleSystem trailParticles, splashParticles;
    bool playedImpactEffect=false;

    protected virtual void Awake()
    {
        body = transform.GetChild(0);
        col = GetComponent<SphereCollider>();
        trail = body.GetComponent<TrailRenderer>();
        if(!trailParticles)
        trailParticles = body.GetComponent<ParticleSystem>();
        if(!splashParticles)
        splashParticles = transform.GetChild(1).GetComponent<ParticleSystem>();
    }

    public virtual void Shoot(AttackInfo shot)
    {
        gameObject.SetActive(true);
        isShooting = true;

        col.radius = shot.hitbox;
        if (currentShot!=null)
            StopCoroutine(currentShot);
       currentShot = StartCoroutine(ExecuteShot(new AttackInfo(shot)));
    }

    protected Coroutine currentShot=null;
    public virtual IEnumerator ExecuteShot(AttackInfo shot)
    {
        StartShot(shot);
        yield return null;
        EndShot(shot);
    }
    public void StartShot(AttackInfo shot)
    {
        transform.position = shot.origin;
        trail.Clear();
        trailParticles.Stop();
        splashParticles.Stop();

        gameObject.SetActive(true);

        playedImpactEffect = false;

        trail.enabled = true;
        trailParticles.Play();

        isShooting = true;
    }
    public void EndShot(AttackInfo shot)
    {
        trail.enabled = false;

        if (!playedImpactEffect)
        {
            shot.PlayImpactEffect();
            playedImpactEffect = true;
        }

        gameObject.SetActive(false);

        isShooting = false;

        lastActiveTime = Time.time;
    }

    public void SplashDamage(AttackInfo shot, float damageModifier=1)
    {
        splashParticles.Play();
        if (!playedImpactEffect)
        {
            shot.PlayImpactEffect();
            playedImpactEffect = true;
        }

        Collider[] hitCol = Physics.OverlapSphere(transform.position, shot.splashRadius, shot.LMHittable);
        foreach (Collider col in hitCol)
        {
            HittableGlobeEntity targetHit = col.transform.gameObject.GetComponent<HittableGlobeEntity>();
            if (targetHit != null)
            {
                targetHit.Hit(shot.attacker, Mathf.RoundToInt(shot.Damage * damageModifier));
            }
        }
    }

    public float GetDamageModifier(AttackInfo shot, float timer, float time, float distanceTraveled)
    {

        if (shot.WindUpTotal)
            return Mathf.Clamp01(timer / time);
        else
            return Mathf.Clamp01(distanceTraveled/ shot.WindUpRange);
    }
}
