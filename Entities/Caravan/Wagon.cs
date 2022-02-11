using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wagon : StateMachineGlobeEntity
{
    [Header("Wagon")]
    [HideInInspector] public Caravan caravan;
    public CaravanInventory.WagonProfile myProfile;
    public WagonWreckPickUp wreckPrefab;

    public float shieldHealth;
    public float maxShieldHealth
    {
        get { return TrueMaxHealth() * 0.25f; }
    }

    public override float TrueMaxHealth()
    {
        return maxHealth+myProfile.TotalHealthBuff;
    }
    public override void SetAlive(bool value)
    {
        base.SetAlive(value);
        myProfile.alive = value;
    }

    public Card MasterCard
    {
        get
        {
            Card c = myProfile.Cards[0];
            return c ? c : caravan.inventory.BasicCard;
        }
    }
    public float Charge
    {
        get
        {
            if (!caravan || currentState == null)
                return 0;
            if (myProfile.type==WagonType.Engine)
                return caravan.sprintFuel/caravan.maxSprintFuel;
            return (currentState.chargeTimer-Globe.time)/currentState.chargeReload;
        }
    }

    public void SetWagonProfile(CaravanInventory.WagonProfile newProfile)
    {
        myProfile = newProfile;
        isAlive = myProfile.unlocked;
        gameObject.SetActive(myProfile.unlocked);
    }

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        caravan = Caravan.main;
        SetAlive(isAlive);
        base.Start();
    }

    private void OnEnable()
    {
        Invoke("SubscribeAllCards",0.1f);
    }
    private void OnDisable()
    {
        UnSubscribeAllCards();
    }
    public void SubscribeAllCards()
    {
        foreach (Card c in myProfile.Cards)
        {
            if (c)
                c.SubscribeEvents();
        }
    }
    public void UnSubscribeAllCards()
    {
        foreach (Card c in myProfile.Cards)
        {
            if (c)
                c.UnSubscribeEvents();
        }
    }

    public GameObject normalWagon, LastWagon;
    bool islast;
    public bool isLast
    {
        set
        {
            islast = value;
            if(myProfile.type!=WagonType.Engine)
            {
                normalWagon.SetActive(!value);
                LastWagon.SetActive(value);
            }
        }
        get { return islast; }
    }

    public override State ReconciderState()
    {
        nextStateActiveTime = 0;
        return MasterCard.AttackState;
    }
    public override bool Hit(HittableGlobeEntity attacker, int value)
    {
        value = Mathf.RoundToInt(value + 2 * Globe.main.cycleDate);

        int shieldedAmount = Mathf.Min(Mathf.FloorToInt(shieldHealth), value);
        value -= shieldedAmount;
        shieldHealth -= shieldedAmount;

        return base.Hit(attacker, value);
    }
    public override void Die(HittableGlobeEntity attacker, bool drop = true)
    {
        base.Die(attacker,drop);
        myProfile.alive = false;

        if (drop)
            caravan.inventory.RefreshKeywords();
    }
    public override void Drop()
    {
        WagonWreckPickUp wreck = Instantiate(wreckPrefab, transform.position, Quaternion.identity, caravan.transform);
        wreck.SetUp(myProfile);
    }


    public override AttackInfo GetAttackInformation()
    {
        AttackInfo shot = new AttackInfo();
        Card master = MasterCard;

        master.SetUpAudio(shot);

        shot.SetUpAll( this,
            DataBase.Entities.LMEnemy, 
            position,position,null, 
            master.AttackDamage, 
            master.AttackRange,
            master.AttackHitbox,
            master.AttackSpeed,0,
            master.AttackReload,
            master.AttackSplashRadius,0,false,
            master.ProjectilePrefab);

        if (caravan)
            caravan.SampleAttackInfo(this, master, shot);

        return shot;
    }
    public override void ChangeState(State newState)
    {
        if (newState != null)
            newState.myWagon = this;
        base.ChangeState(newState);
    }
}
