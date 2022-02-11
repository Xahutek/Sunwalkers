using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : ScriptableObject
{
    public Sprite illustration;
    [Header("Description")]
    public new string name;
    [TextArea] public string effect;

    [Header("Stats")]
    public List<Keyword> keywords;
    [SerializeField]
    protected int
        attackDamage,
        attackRange,
        attackReload;
    [HideInInspector]
    protected int
        buffAttackDamage,
        buffAttackRange,
        buffAttackReload;
    [SerializeField] protected float
        attackShotSpeed,
        attackShotHitbox,
        attackSplashRadius;

    public Vector3Int RarityOwned;

    public Projectile ProjectilePrefab;

    public State AttackState
    {
        get
        {
            foreach (Keyword keyword in keywords)
            {
                State attackState = KeywordDataBase.GetAttackState(keyword);
                if (attackState != null)
                    return attackState;
            }
            return KeywordDataBase.GetAttackState(Keyword.Ranger);
        }
    }
    public void SetUpAudio(AttackInfo shot)
    {
        SoundManager manager = SoundManager.main;
        foreach (Keyword keyword in keywords)
        {
            switch (keyword)
            {
                case Keyword.Ranger:
                    shot.SetUpAudio(manager.GetOneShot(keyword,true), manager.GetOneShot(keyword)); 
                    return;
                //case Keyword.Ballista:
                //    break;
                case Keyword.Lancer:
                    shot.SetUpAudio(manager.GetOneShot(keyword, true), manager.GetOneShot(keyword));
                    return;
                case Keyword.Alchemist:
                    shot.SetUpAudio(manager.GetOneShot(keyword, true), manager.GetOneShot(keyword));
                    return;
                case Keyword.Shaman:
                    shot.SetUpAudio(manager.GetOneShot(keyword, true), manager.GetOneShot(keyword));
                    return;
                //case Keyword.Mender:
                //    break;
                //case Keyword.Guardian:
                //    break;
                //case Keyword.Nomad:
                //    break;
                //case Keyword.Trapper:
                //    break;
                default:
                    break;
            }
        }
    }

    public int AttackDamage
    {
        get
        {
            int value = attackDamage*Level+buffAttackDamage;

            return value;
        }
    }
    public float AttackRange
    {
        get
        {
            float value = attackRange + buffAttackRange;

            return value;
        }
    }
    public float AttackReload
    {
        get
        {
            float value = attackReload + buffAttackReload;
            //value = 2/Mathf.Max(0.5f,Random.Range(value*0.9f, value*1.1f));

            return value;
        }
    }
    public float AttackSpeed
    {
        get
        {
            float value = attackShotSpeed;

            return value;
        }
    }
    public float AttackHitbox
    {
        get
        {
            float value = attackShotHitbox;

            return value;
        }
    }
    public float AttackSplashRadius
    {
        get
        {
            float value = attackSplashRadius;

            return value;
        }
    }

    public int AnyRarity
    {
        get
        {
            return BronzeRarity + SilverRarity + GoldRarity;
        }
        set
        {
            BronzeRarity = value;
            SilverRarity = value;
            GoldRarity = value;
        }
    }
    public int BronzeRarity
    {
        get
        {
            int value = RarityOwned[0];

            if (value>=3)
            {
                value -= 3;
                RarityOwned[0] = value;
                RarityOwned[1] += 1;
            }

            return value;
        }
        set
        {
            if (value >= 3)
            {
                value -= 3;
                RarityOwned[1] += 1;
            }

            RarityOwned[0] = value;
        }
    }
    public int SilverRarity
    {
        get
        {
            int value = RarityOwned[1];

            return value;
        }
        set
        {
            if (value >= 3)
            {
                value -= 3;
                RarityOwned[2] += 1;
            }

            RarityOwned[1] = value;
        }
    }
    public int GoldRarity
    {
        get
        {
            int value = RarityOwned[2];

            return value;
        }
        set
        {
            RarityOwned[2] = value;
        }
    }
    public void CutHighest()
    {
        if (GoldRarity > 0)
            GoldRarity -= 1;
        else if (SilverRarity > 0)
            SilverRarity -= 1;
        else if (BronzeRarity > 0)
            BronzeRarity -= 1;
    }
    public int Level
    {
        get
        {
            if (GoldRarity > 0)
                return 3;
            else if (SilverRarity > 0)
                return 2;
            else if (BronzeRarity > 0)
                return 1;
            return 1;
        }
    }

    public void SubscribeEvents()
    {
        CombatEventManager manager = CombatEventManager.main;
        if (!manager)
            return;

        manager.OnAttack += OnAttack;
        manager.OnHit += OnHit;
        manager.OnRam += OnRam;
        manager.OnKill += OnKill;
        manager.OnHeal += OnHeal;
    }
    public void UnSubscribeEvents()
    {
        CombatEventManager manager = CombatEventManager.main;
        if (!manager)
            return;

        manager.OnAttack -= OnAttack;
        manager.OnHit -= OnHit;
        manager.OnRam -= OnRam;
        manager.OnKill -= OnKill;
        manager.OnHeal -= OnHeal;
    }
    public virtual void OnAttack(AttackInfo shot)
    {
    }
    public virtual void OnHit(GlobeEntity target, GlobeEntity attacker, float damage)
    {
        //Debug.Log(target.name +" was hit by "+attacker.name+" for "+damage+" damage!");
    }
    public virtual void OnRam(GlobeEntity target, GlobeEntity attacker, float damage)
    { 
    }
    public virtual void OnKill(GlobeEntity target, GlobeEntity attacker)
    {
    }
    public virtual void OnHeal(GlobeEntity target, float amount)
    {
    }
}
