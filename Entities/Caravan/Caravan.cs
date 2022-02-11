using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caravan : MonoBehaviour
{
    public static Caravan main;

    [Header("Components")]
    public CaravanInventory inventory;
    public Menu deathMenu;
    public AudioSource EngineAudioSource;
    public ParticleSystem BulldozeEffect,RamEnemyEffect, RamWallEffect;
    public LayerMask LMOcean;
    public AudioClip BulldozeHitAudioClip;

    [Header("Wagon Variables")]
    public Wagon[] Wagons= new Wagon[5];
    public List<Wagon> activeWagons = new List<Wagon>();
    public bool isAlive = true;
    public Vector3 position
    {
        get
        {
            return Wagons[0].position;
        }
        set
        {
            foreach (Wagon w in Wagons)
            {
                w.position = value;
                Globe.Orientate(w.transform);
            }
        }
    }
    public Vector3 forward
    {
        get
        {
            return Wagons[0].transform.forward;
        }
    }

    public ActiveAbility activeAbility
    {
        get
        {
            return inventory.activeAbility;
        }
        set
        {
            inventory.activeAbility = value;
        }
    }
    [Header("Combat")]
    public Enemy focusedEnemy = null;
    public static bool inCombat
    {
        get
        {
            bool
                inArena = CombatArena.currentArenas.Count > 0,
                withEnemies = false;
            foreach (CombatArena arena in CombatArena.currentArenas)
            {
                if (!arena.allDead)
                    withEnemies = true;
            }
            return inArena && withEnemies;
        }
    }

    public bool isBulldozing = false;
    [SerializeField] Vector3 ramEnemyBaseDamage, ramWallBaseDamage;
    [HideInInspector]public float BulldozeDamageMultiplier;
    public int RamEnemyDamage
    {
        get
        {
            float value =  Mathf.Lerp(ramEnemyBaseDamage.x, Mathf.Lerp(ramEnemyBaseDamage.y, ramEnemyBaseDamage.z, sprintForce), speedForce);
            if (isBulldozing)
                value *= BulldozeDamageMultiplier;

            return Mathf.Max(0, Mathf.RoundToInt(value));
        }
    }
    public int RamEnemySelfDamage
    {
        get
        {
            float value = isBulldozing?0: 0.333f * Mathf.Lerp(ramEnemyBaseDamage.x, Mathf.Lerp(ramEnemyBaseDamage.y, ramEnemyBaseDamage.z, sprintForce), speedForce);

            return Mathf.Max(0, Mathf.RoundToInt(value));
        }
    }
    public int RamWallDamage
    {
        get
        {
            float value = Mathf.Lerp(ramWallBaseDamage.x, Mathf.Lerp(ramWallBaseDamage.y, ramWallBaseDamage.z, sprintForce), speedForce);
            if (isBulldozing)
                value *= BulldozeDamageMultiplier;

            return Mathf.Max(0, Mathf.RoundToInt(value));
        }
    }

    [Header("Resources")]
    public float bonusHeal = 0;
    public float bonusHealSpeed=4;
    public float
        amberFuelConsumption = 0.2f;
    public float
        timberHealConsumption = 0.1f,
        healSpeed=3;

    public int NewWagonCost=25, WagonUpgradeHealthCost = 10, WagonUpgradeSlotCost = 10;

    public float Amber
    {
        get
        {
            return inventory.Amber;
        }
        set
        {
            inventory.Amber = value;
        }
    }
    public float Timber
    {
        get
        {
            return inventory.Timber;
        }
        set
        {
            inventory.Timber = value;
        }
    }

    public bool inNightfall = false;
    public float 
        NightAmberDrain, 
        NightDamageDrain;

    [Header("Movement Variables")]
    public bool isSprinting = false;
    [Range(0, 2)] public float speedForce = 1;
    [Range(0, 1)] public float sprintForce = 1;
    public AnimationCurve sprintBoostCurve;
    public float sprintFuel;
    [HideInInspector]private float sprintTime;
    public float maxSprintFuel
    {
        get
        {
            return inventory.wagons[0].bonusSlots + 5;
        }
    }
    public bool sprintFuelSufficient
    {
        get
        {
            return (isSprinting && sprintFuel > 0) || (!isSprinting && sprintFuel > maxSprintFuel* 0.2f);
        }
    }

    public float wagonMinDistance = 1.4f;
    public Vector3 wagonMaxDistance = new Vector3(0.023f, 0.07f, 0.12f);
    public float
        minimalMovementSpeed = 100,
        minimalRotationSpeed = 45;
    public float
        normalMovementSpeed = 300,
        normalRotationSpeed = 90;
    public float
        sprintMovementSpeed = 550,
        sprintRotationSpeed = 40;

    Vector3 lastEnginePosition;
    [HideInInspector]public bool isMoving;

    private void Awake()
    {
        main = this;

        position = Wagons[0].position;

        RefreshActiveWagons();
        inventory.RefreshKeywords();

            if (activeAbility)
                activeAbility.Reset(this);
    }
    private void Start()
    {
        inventory.SetUp();
    }
    public void RefreshActiveWagons()
    {
        activeWagons = new List<Wagon>();
        int lastWagon = 0;
        for (int i = 0; i < Wagons.Length; i++)
        {
            CaravanInventory.WagonProfile p = inventory.wagons[i];
            Wagons[i].SetWagonProfile(p);

            Wagons[i].caravan = this;
            Wagons[i].gameObject.SetActive(p.unlocked&&p.alive);
            if (p.unlocked && p.alive)
            {
                activeWagons.Add(Wagons[i]);
                lastWagon = i;
                Wagons[i].isLast = false;
            }

            Wagons[i].position = Wagons[0].position;
        }

        Wagons[lastWagon].isLast = true;
    }

    #region Update
    private void FixedUpdate()
    {
        if (isAlive && Globe.isCounting)
        {
            Movement();
            ActiveCombat();

            ResourceManagement();
            AdditionalFixedUpdates();

            if (inNightfall)
                NightHit();
        }
        else
            activeWagons[0].rb.velocity = Vector3.zero;
    }

    public void ResourceManagement()
    {
        float change = Mathf.Clamp(Globe.fixedDeltaTime * (isSprinting ? -sprintBoostCurve.Evaluate(sprintTime) : 1), -sprintFuel, maxSprintFuel - sprintFuel);
        if (Amber > 0)
        {
            if (change > 0)
                Amber -= change * amberFuelConsumption;
        }
        else
            change = Mathf.Min(change,0);
        sprintFuel = Mathf.Clamp(sprintFuel + change, 0, maxSprintFuel);

        change = 0;
        float bonusHealAmount = Globe.fixedDeltaTime * bonusHealSpeed;
        if (bonusHeal > 0)
        {
            bonusHeal = Mathf.Max(bonusHeal - bonusHealAmount, 0);

            Wagon lowest = GetLowestWagon();

            change = bonusHealAmount;
            if (lowest.health < lowest.TrueMaxHealth())
            {
                Debug.Log(change);
                lowest.Heal(change);
            }
        }
        //else
        //if (Timber > 0 && !inNightfall)
        //{
        //    Wagon lowest = GetLowestWagon();

        //    change = Mathf.Min(Globe.fixedDeltaTime * healSpeed, lowest.TrueMaxHealth() - lowest.unitHealth);
        //    if (change > 0)
        //    {
        //        Timber -= change * timberHealConsumption;
        //        lowest.Heal(change);
        //    }
        //}
    }
    public void Movement()
    {
        float
            verticalInput = isBulldozing?1: Input.GetAxisRaw("Vertical"),
            horizontalInput = Input.GetAxisRaw("Horizontal")* (isBulldozing?0.1f:1);
        bool
            speedBoost = Input.GetKey(KeyCode.W)||isBulldozing,
            speedBreak = Input.GetKey(KeyCode.S) && !speedBoost;
        float
            thisMovementSpeed = 0,
            thisRotationSpeed = 0;

        //SpeedForce
        if (verticalInput != 0)
            speedForce = Mathf.Clamp(
                speedForce + verticalInput * Globe.fixedDeltaTime * (1 - Wagons[0].speedMalus), //alter speedforce to match vertical input
                (speedBreak ? 0 : 1),//allow speed under 1 only if actively breaking
                sprintFuelSufficient || isBulldozing ? (speedBoost ? 2 : 1.25f) : 1);//allow speed over 1.25 only if fuel is sufficient and if actively sprinting or if Bulldozing
        else if (Mathf.Abs(speedForce - 1) > Globe.fixedDeltaTime)//if no speed input is given, slowly normalize it
            speedForce = Mathf.Clamp(speedForce - Globe.fixedDeltaTime * Mathf.Sign(speedForce - 1), 0.5f, 1.5f);
        else speedForce = 1;

        isSprinting = speedForce > 1.25f && speedBoost;
        sprintTime = isSprinting ? sprintTime + Globe.fixedDeltaTime : 0;

        //Determine Variables
        sprintForce = sprintForce = Mathf.Clamp01(sprintForce + (isSprinting ? 1 : -1) * Globe.fixedDeltaTime);
        float
            easedForce = sprintForce * (2 - sprintForce);

        //if(sprintForce>0.1f&&sprintForce<0.8f)
        //    Debug.Log("At sprintTime "+sprintForce+", SprintSpeed is "+ 
        //        Mathf.LerpUnclamped(normalMovementSpeed, sprintMovementSpeed, sprintBoostCurve.Evaluate(sprintTime))+ " instead of "+ 
        //        Mathf.LerpUnclamped(normalMovementSpeed, sprintMovementSpeed, sprintForce) + " because boost is "+ sprintBoostCurve.Evaluate(sprintForce));

        thisMovementSpeed = Mathf.Lerp(minimalMovementSpeed, Mathf.LerpUnclamped(normalMovementSpeed, sprintMovementSpeed, isSprinting? sprintBoostCurve.Evaluate(sprintTime): sprintForce), speedForce);
        thisRotationSpeed = Mathf.Lerp(minimalRotationSpeed, Mathf.Lerp(normalRotationSpeed, sprintRotationSpeed, easedForce), speedForce);

        thisRotationSpeed = Mathf.Lerp(thisRotationSpeed-20, thisRotationSpeed+10,1-(float)activeWagons.Count/8);

        #region Execute Movement
        if (!Wagons[0].isAlive)
        {
            Death();
            return;
        }

        //Rotation
        if (horizontalInput != 0)
            activeWagons[0].transform.Rotate(new Vector3(0, horizontalInput, 0) * thisRotationSpeed * Globe.fixedDeltaTime);

        //Movement
        activeWagons[0].rb.velocity = activeWagons[0].transform.forward * thisMovementSpeed * Globe.fixedDeltaTime;

        Transform curWagon, prevWagon = activeWagons[0].transform;
        for (int i = 1; i < activeWagons.Count; i++)
        {
            curWagon = activeWagons[i].transform;
            if (!curWagon.gameObject.activeSelf)
            {
                activeWagons.Remove(activeWagons[i]);
                i--;
                continue;
            }

            Vector3 direction = prevWagon.position - curWagon.position;
            float distance = direction.magnitude;

            float T = Mathf.Min(
               Globe.fixedDeltaTime * distance / wagonMinDistance * thisMovementSpeed,
               /*Mathf.Lerp(wagonMaxDistance.x, Mathf.Lerp(wagonMaxDistance.y, wagonMaxDistance.z, sprintForce), speedForce)*/1);

            if (distance > wagonMinDistance)
            {
                Vector3 targetPos = Vector3.Lerp(curWagon.position, prevWagon.position, T), targetDirect = targetPos - curWagon.position;
                curWagon.position += targetDirect.normalized * Mathf.Min(thisMovementSpeed * Time.fixedDeltaTime, targetDirect.magnitude) * (distance / wagonMinDistance - 1);
            }
            if (distance > wagonMinDistance * 0.5f)
            {
                Vector3
                    rotTarget = prevWagon.position - prevWagon.forward * 0.7f,
                    rotTargetDirection = (rotTarget - curWagon.position).normalized;

                Debug.DrawRay(curWagon.position, curWagon.forward, Color.green, 0.1f);
                Debug.DrawRay(curWagon.position, rotTargetDirection, Color.cyan, 0.1f);

                //float angleDifference =
                //    Vector3.SignedAngle(curWagon.forward, rotTargetDirection, curWagon.up);
                //Vector3 eulers= curWagon.rotation.eulerAngles;
                //eulers.y += angleDifference * T;
                //eulers.y %= 360;
                //curWagon.rotation= Quaternion.Euler(eulers);
                curWagon.LookAt(rotTarget);
                Globe.Orientate(curWagon);

                //curWagon.Rotate(curWagon.up,angleDifference);

                //curWagon.rotation = Quaternion.Slerp(curWagon.rotation, Quaternion.FromToRotation(Vector3.forward, rotTargetDirection), T);
            }

            prevWagon = curWagon;
        }

        #endregion
    }
    public void ActiveCombat()
    {
        if (activeAbility)
            activeAbility.FixedRefresh(this, Input.GetMouseButton(1));
    }

    public void AdditionalFixedUpdates()
    {
        bool wasMoving = isMoving;
        Vector3 currentEnginePosition = Wagons[0].position;
        isMoving = Vector3.Distance(lastEnginePosition,currentEnginePosition)>3*Time.fixedDeltaTime;//Use TIME CLASS so it works in menus
        lastEnginePosition = currentEnginePosition;
        if (wasMoving&&!isMoving)
            EngineAudioSource.Stop();
        if (isMoving && !wasMoving)
            EngineAudioSource.Play();

        SoundManager.main.BlizzardPlays = inNightfall || Mathf.Abs(position.x)>65f;

        Collider[] hitCol = Physics.OverlapSphere(Wagons[0].position, 8, LMOcean);
        SoundManager.main.OceanPlays = hitCol.Length>0;
    }

    #endregion

    #region Events

    public void Death()
    {
        isAlive = false;
        foreach (Wagon w in Wagons)
        {
            if (w.isAlive)
            {
                w.health = 0;
                w.Die(null,false);
            }
        }
        if (deathMenu)
            deathMenu.TriggerOpen();
    }
    public void Respawn()
    {
        Debug.Log("RESPAWN CARAVAN!");
    }
    public float NightDamageBuildup;
    public void NightHit()
    {
        if (!Globe.isCounting)
            return;

        Debug.Log("Night Hit");

        if (Amber > 0)
            Amber -= NightAmberDrain * Globe.fixedDeltaTime;
        else
        {
            int dmgPart = 5;
            NightDamageBuildup += NightDamageDrain * Globe.fixedDeltaTime;
            if (NightDamageBuildup > dmgPart)
            {
                NightDamageBuildup -= dmgPart;
                foreach (Wagon w in Wagons)
                {
                    if (w.isAlive)
                        w.Hit(null, dmgPart);
                }
            }
        }
            
    }
    public void RamEnemy(Enemy enemy)
    {
        Debug.Log(enemy.name+" rammed!");

        if(isBulldozing)
            SoundManager.main.PlayOneShot(BulldozeHitAudioClip);

        enemy.Hit(Wagons[0],RamEnemyDamage);
        Wagons[0].Hit(null,RamEnemySelfDamage);

        RamEnemyEffect.Play();
    }
    public void RamWall()
    {
        Debug.Log("Wall Rammed");
        Globe.main.mainCamControl.Shake();

        Wagons[0].Hit(null,RamWallDamage);

        RamWallEffect.Play();
    }

    public void ReconciderLoadout()
    {
        inventory.ReconciderActiveAbility();
        foreach (Wagon w in Wagons)
        {
            w.caravan = this;
            if(w.gameObject.activeSelf)
            w.ReconciderState();
        }
    }
    #endregion

    #region Utility
    public AttackInfo SampleAttackInfo(Wagon wagon, Card masterCard, AttackInfo baseInfo)
    {
        return baseInfo;
    }



    public Wagon GetClosestWagon(Vector3 pos)
    {
        Wagon closest = null;
        float closestDistance = float.PositiveInfinity;

        foreach (Wagon w in activeWagons)
        {
            float distance = Vector3.Distance(w.transform.position, pos);
            if (w.isAlive && w.gameObject.activeSelf && (closest == null || closestDistance > distance))
            {
                closest = w;
                closestDistance = distance;
            }
        }

        return Wagons[0];
    }
    public Wagon GetFurthestWagon(Vector3 pos)
    {
        Wagon furthest = null;
        float furthestDistance = 0;

        foreach (Wagon w in activeWagons)
        {
            float distance = Vector3.Distance(w.transform.position, pos);
            if (w.isAlive&& (furthest == null || furthestDistance < distance))
            {
                furthest = w;
                furthestDistance = distance;
            }
        }

        return furthest;
    }
    public Wagon GetLowestWagon()
    {
        Wagon lowest = null, engine = Wagons[0];
        for (int i = 1; i < activeWagons.Count; i++)
        {
            if (lowest == null || activeWagons[i].unitHealth <= lowest.unitHealth)
                lowest = activeWagons[i];
        }
        if ((lowest == null || (engine.unitHealth / engine.TrueMaxHealth()) - 0.3f < lowest.unitHealth / lowest.TrueMaxHealth()) && engine.unitHealth != engine.TrueMaxHealth())
            lowest = engine;
        return lowest;
    }

    //public void AddWagon()
    //{
    //    Debug.Log("AddWagon");
    //    Wagon body = Instantiate(WagonPrefab, Wagons[Wagons.Count - 1].transform.position, Wagons[Wagons.Count - 1].transform.rotation, transform);
    //    Wagons.Add(body);
    //    body.caravan = this;
    //}


    #endregion
}
