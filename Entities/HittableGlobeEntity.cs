using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
public class HittableGlobeEntity : GlobeEntity
{
    [Header("Hittable Entity")]
    public bool isInvulnerable=false;
    public bool isAlive
    {
        get
        {
            return alive;
        }
        set
        {
            SetAlive(value);
        }
    }
    public virtual void SetAlive(bool value)
    {
        alive = value;
    }
    [SerializeField]private bool alive;

    public bool healthScalesWithGlobe=true;
    public float health, maxHealth=1;
    public float woundedHealthMargin = 0.35f;

    public float speedMalus;
    private float speedMalusCounter;
    public bool isSlowed
    {
        get { return speedMalus > 0; }
    }

    public float unitHealth
    {
        get
        {
            return Mathf.RoundToInt(health);
        }
    }

    public virtual float TrueMaxHealth()
    {
        return maxHealth + (healthScalesWithGlobe && Globe.main && Time.time > 0.5f ? 5 * Globe.main.cycleDate : 0);
    }

    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public BoxCollider col;
    [HideInInspector] public Transform body;

    [SerializeField] AudioClip  hitAudio, deathAudio;

    [SerializeField] ParticleSystem hitParticles, deathParticles, woundedParticles;
    HittableGlobeEntity lastAttacker;

    protected Vector3 localBodyPos;

    protected override void Awake()
    {
        base.Awake();

        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
        body = transform.GetChild(0);

        localBodyPos = body.localPosition;

        maxHealth = Mathf.Max(maxHealth,1);
        Resurrect();
    }

    protected override void FixedUpdate()
    {
        speedMalus = Mathf.Max(speedMalus -= Globe.fixedDeltaTime * 0.08f, 0);

        if (alive&&unitHealth <= 0)
                Die(lastAttacker);

        base.FixedUpdate();
    }

    public virtual void Slow(float value)
    {
        speedMalus= Mathf.Clamp(speedMalus+value,0,0.6f);
    }
    public virtual bool Heal(float value)
    {
        health = Mathf.Clamp(unitHealth + value, 0, TrueMaxHealth());
        if (NumberPopPool.main&&value>1)
            NumberPopPool.main.NumberPop().Pop(transform.position, Mathf.CeilToInt(value), Color.Lerp(Color.green, Color.white,0.5f));

        if (woundedParticles && (float)unitHealth / (float)TrueMaxHealth() > woundedHealthMargin)
            woundedParticles.Stop();

        if (CombatEventManager.main) CombatEventManager.main.Heal(this, value);

        if (unitHealth <= 0)
            Die(null);
        return isAlive;
    }
    public virtual bool Hit(HittableGlobeEntity attacker,int value)
    {
        if (isInvulnerable)
            return isAlive;

        //if (healthScalesWithGlobe && Globe.main && Time.time > 0.5f)
        //    Debug.Log("a");

        health = Mathf.Clamp(unitHealth - value, 0, TrueMaxHealth());
        if (NumberPopPool.main&&value>=1)
            NumberPopPool.main.NumberPop().Pop(transform.position, value, Color.Lerp(Color.red, Color.white, 0.5f));

        if (hitParticles)
            hitParticles.Play();
        if (woundedParticles && (float)unitHealth / (float)TrueMaxHealth() <= woundedHealthMargin)
            woundedParticles.Play();

        if (CombatEventManager.main) CombatEventManager.main.Hit(this,attacker, value);

        if (SoundManager.main&&value>1) SoundManager.main.PlayOneShot(hitAudio);

        if (unitHealth <= 0)
            Die(attacker);

        lastAttacker = attacker;
        return isAlive;
    }

    public void TakeInvulnerable()
    {
        isInvulnerable = false;
        if (InvulnerableRoutine != null)
            StopCoroutine(InvulnerableRoutine);
    }
    public void MakeInvulnerable(float time)
    {
        if (!gameObject.activeSelf || !isAlive)
            return;
        isInvulnerable = false;
        if (InvulnerableRoutine != null)
            StopCoroutine(InvulnerableRoutine);
        InvulnerableRoutine=StartCoroutine(ExecuteInvulnerability(time));
    }
    Coroutine InvulnerableRoutine = null;
    public IEnumerator ExecuteInvulnerability(float time)
    {
        float timer = Globe.time + time;
        while (timer>Globe.time)
        {
            isInvulnerable = true;
            yield return null;
        }
        isInvulnerable = false;
    }
    public void Resurrect()
    {
        isAlive = true;
        Heal(TrueMaxHealth());
    }
    public virtual void Die(HittableGlobeEntity attacker,bool drop=true)
    {
        if (!isAlive)
            return;
        isAlive = false;
        Debug.Log(name+" has died!");

        if(deathParticles)
            deathParticles.Play();

        if (CombatEventManager.main) CombatEventManager.main.Kill(this, attacker);

        if (SoundManager.main&&drop) SoundManager.main.PlayOneShot(deathAudio);

        Globe.main.StartCoroutine(ExecuteDeactivation(1f,drop));
    }

    public IEnumerator ExecuteDeactivation(float time, bool drop=true)
    {
        float timer = 0;
        while (timer<=time)
        {
            timer += Time.deltaTime;

            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, timer / time);

            yield return null;
        }

        if (woundedParticles)
            woundedParticles.Stop();


        if (drop)
            Drop();

        gameObject.SetActive(false);

        transform.localScale = Vector3.one;
    }

    public virtual void Drop()
    {

    }

    public void KnockBack(Vector3 dir, float force)
    {
        StartCoroutine(ExecuteKnockBack(dir.normalized*force));
    }

    public IEnumerator ExecuteKnockBack(Vector3 dir)
    {
        float s = 0.3f;
        float time= dir.magnitude*s, timer = 0;
        Vector3 cutDir = dir * Globe.deltaTime * 1/s;
        while (timer<=time)
        {
            timer += Globe.deltaTime;

            transform.position += cutDir;

            yield return null;
        }
    }
}
