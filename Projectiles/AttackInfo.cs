using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackInfo
{
    public LayerMask LMHittable;

    public Vector3 origin;
    public Vector3 destination;

    public Animator anim=null;
    public HittableGlobeEntity attacker=null, target = null;
    public Projectile projectilePrefab;
    public AudioClip ReleaseAudio, ImpactAudio;
    public ParticleSystem additionalEffects, impactEffects;
    public void PlayEffect(bool effect=true)
    {
        if (ReleaseAudio&&effect)
            SoundManager.main.PlayOneShot(ReleaseAudio);

        if (additionalEffects)
        {
            additionalEffects.gameObject.SetActive(effect);
            if (effect)
                additionalEffects.Play();
            else
                additionalEffects.Stop();
        }
    }
    public void PlayImpactEffect(bool effect=true)
    {
        if (ImpactAudio&&effect)
            SoundManager.main.PlayOneShot(ImpactAudio);

        if (impactEffects)
        {
            impactEffects.gameObject.SetActive(effect);
            if (effect)
                impactEffects.Play();
            else
                impactEffects.Stop();
        }
    }

    public float fluidDamage;
    public int Damage
    {
        get
        {
            return Mathf.RoundToInt(fluidDamage);
        }
        set
        {
            fluidDamage = value;
        }
    }

    public float range,hitbox=0.1f;
    [SerializeField] protected Vector3 speed;
    public float Speed
    {
        get
        {
            return Mathf.Max(speed[1], 0.5f);
        }
        set
        {
            speed[1] = value;
        }
    }
    public float SpeedAnticipation
    {
        get
        {
            return Mathf.Max(speed[0], 0);
        }
        set
        {
            speed[0] = value;
        }
    }
    public float SpeedExecution
    {
        get
        {
            return Mathf.Max(speed[1],0.5f);
        }
        set
        {
            speed[1] = value;
        }
    }
    public float SpeedAfterstun
    {
        get
        {
            return Mathf.Max(speed[2], 0);
        }
        set
        {
            speed[2] = value;
        }
    }

    public int ammo;
    public float reload;

    public bool WindUpTotal = false;
    public float WindUpRange = 0;

    public bool blockable = false;

    public float splashRadius = 0, windUpTime=0;

    public AttackInfo()
    {

    }
    public AttackInfo(AttackInfo s)
    {
        SetUpAll(s.attacker,s.LMHittable,s.origin,s.destination,s.target,s.Damage,s.range,s.hitbox,s.Speed,s.ammo,s.reload,s.splashRadius, s.windUpTime, s.blockable,s.projectilePrefab,s.additionalEffects,s.impactEffects,s.anim);
        SetUpRichSpeed(s.speed);
        SetUpAudio(s.ReleaseAudio,s.ImpactAudio);
    }

    public void SetUpAudio(AudioClip release, AudioClip impact)
    {
        ReleaseAudio = release;
        ImpactAudio = impact;
    }
    public void SetUpBasic(HittableGlobeEntity attacker ,LayerMask LMHittable, Vector3 origin, Vector3 destination, HittableGlobeEntity target)
    {
        this.attacker = attacker;
        this.LMHittable = LMHittable;
        this.origin = origin;
        this.destination = destination;
        this.target = target;
    }
    public void SetUpAll( HittableGlobeEntity attacker,
        LayerMask LMHittable, Vector3 origin, Vector3 destination, HittableGlobeEntity target,
        int damage, float range, float hitbox, float speed, int ammo, float reload, float splashRadius = 0, float windUpTime = 0, bool blockable = false,
        Projectile prefab = null, ParticleSystem effects = null, ParticleSystem impactEffect =null, Animator anim = null)
    {
        SetUpBasic(attacker,LMHittable, origin, destination, target);

        this.anim = anim;
        this.projectilePrefab = prefab;
        this.additionalEffects = effects;
        this.impactEffects = impactEffect;

        this.fluidDamage = damage;

        this.range = range;
        this.hitbox = hitbox;
        Speed = speed;

        this.ammo = ammo;
        this.reload = reload;

        this.blockable = blockable;

        this.splashRadius = splashRadius;
        this.windUpTime = windUpTime;
    }
    public void SetUpRichSpeed(float anticipation, float execution, float afterstun)
    {
        SetUpRichSpeed(new Vector3(anticipation,execution,afterstun));
    }
    public void SetUpRichSpeed(Vector3 speed)
    {
        this.speed = speed;
    }

    //public AttackInfo(AttackInfo s) //copies other
    //{
    //    SetUpAll(s.LMHittable,s.particleEffect, s.projectilePrefab, s.origin,s.destination,s.target,s.range,s.ammo,s.reload,s.distance,s.anticipationSpeed,s.speed,s.afterstunSpeed,s.Damage,s.pierceTargets,s.blockable,s.splashRadius);
    //}
    //public void SetUpRichSpeed(float anticipation,float duration, float afterstun)
    //{
    //    anticipationSpeed = anticipation;
    //    speed = duration;
    //    afterstunSpeed = afterstun;
    //}
    //public void SetUpBasic(LayerMask LMHittable, Vector3 origin, float range, float reload, float speed, int damage, float splashRadius = 0, Projectile prefab=null)
    //{
    //    this.LMHittable = LMHittable;
    //    this.origin = origin;
    //    this.projectilePrefab = prefab;

    //    this.reload = reload;
    //    this.range = range;

    //    this.speed = speed;
    //    this.fluidDamage = damage;

    //    this.splashRadius = splashRadius;
    //}
    //public void AlterBasic(float range, float reload, float speed, int damage, float splashRadius = 0)
    //{
    //    this.reload += reload;
    //    this.range += range;

    //    this.speed += speed;
    //    this.fluidDamage += damage;

    //    this.splashRadius += splashRadius;
    //}
    //public void MultiplyBasic(float range, float reload, float speed, float damage, float splashRadius = 0)
    //{
    //    this.reload *= reload;
    //    this.range *= range;

    //    this.speed *= speed;
    //    this.fluidDamage *= damage;

    //    this.splashRadius *= splashRadius;
    //}

    //public void SetUpAll(LayerMask LMHittable, ParticleSystem effect, Projectile prefab, Vector3 origin, Vector3 direction, HittableGlobeEntity target, float range,int ammo, float reload, float distance,float anticipationSpeed, float speed,float afterstunSpeed, int damage, bool pierceTargets = false, bool blockable = false, float splashRadius = 0)
    //{
    //    this.LMHittable = LMHittable;
    //    this.particleEffect = effect;
    //    this.projectilePrefab = prefab;

    //    this.origin = origin;
    //    this.destination = direction;
    //    this.target = target;

    //    this.ammo = ammo;
    //    this.reload = reload;
    //    this.range = range;

    //    this.distance = distance;

    //    this.anticipationSpeed = anticipationSpeed;
    //    this.speed = speed;
    //    this.afterstunSpeed = afterstunSpeed;

    //    this.fluidDamage = damage;

    //    this.pierceTargets = pierceTargets;
    //    this.blockable = blockable;

    //    this.splashRadius = splashRadius;
    //}

    //public void SetUpDirectedShot(LayerMask LMHittable, Vector3 origin, Vector3 destination, float range, float reload, float distance, float speed, int damage, bool pierceTargets = false, float splashRadius = 0, Projectile prefab = null)
    //{
    //    this.LMHittable = LMHittable;

    //    this.origin = origin;
    //    this.destination = destination;
    //    this.projectilePrefab = prefab;

    //    this.reload = reload;
    //    this.range = range;

    //    this.distance = distance;
    //    this.speed = speed;
    //    this.fluidDamage = damage;

    //    this.pierceTargets = pierceTargets;

    //    this.splashRadius = splashRadius;
    //}
    //public void SetUpTargetedShot(LayerMask LMHittable, Vector3 origin, HittableGlobeEntity target, float range, float reload, float speed, int damage, float splashRadius = 0, Projectile prefab = null)
    //{
    //    this.LMHittable = LMHittable;

    //    this.origin = origin;
    //    this.target = target;
    //    this.projectilePrefab = prefab;

    //    this.reload = reload;
    //    this.range = range;

    //    this.speed = speed;
    //    this.fluidDamage = damage;

    //    this.splashRadius = splashRadius;
    //}
    //public void SetUpSpotShot(LayerMask LMHittable, Vector3 origin, Vector3 destination, float range, float reload, float speed, int damage, float splashRadius = 0, bool blockable = false, Projectile prefab = null)
    //{
    //    this.LMHittable = LMHittable;

    //    this.origin = origin;
    //    this.destination = destination;
    //    this.projectilePrefab = prefab;

    //    this.reload = reload;
    //    this.range = range;

    //    this.speed = speed;
    //    this.fluidDamage = damage;

    //    this.blockable = blockable;

    //    this.splashRadius = splashRadius;
    //}
}
