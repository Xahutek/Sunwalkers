using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatEventManager : MonoBehaviour
{
    public static CombatEventManager main;

    private void Awake()
    {
        main = this;
    }

    public delegate void AttackDelegate(AttackInfo shot);
    public event AttackDelegate OnAttack;
    public void Attack(AttackInfo shot)
    {
        OnAttack?.Invoke(shot);
    }

    public delegate void HitDelegate(GlobeEntity target, GlobeEntity attacker, float damage);
    public event HitDelegate OnHit;
    public delegate void RamDelegate(GlobeEntity target, GlobeEntity attacker, float damage);
    public event RamDelegate OnRam;
    public void Hit(GlobeEntity target, GlobeEntity attacker, float damage)
    {
        if (attacker is Wagon && (attacker as Wagon).myProfile.type==WagonType.Engine)
            OnRam?.Invoke(target, attacker, damage);
        else
            OnHit?.Invoke(target, attacker, damage);
    }

    public delegate void KillDelegate(GlobeEntity targvictimet, GlobeEntity attacker);
    public event KillDelegate OnKill;
    public void Kill(GlobeEntity target, GlobeEntity attacker)
    {
        OnKill?.Invoke(target, attacker);
    }

    public delegate void HealDelegate(GlobeEntity target, float amount);
    public event HealDelegate OnHeal;
    public void Heal(GlobeEntity target, float amount)
    {
        OnHeal?.Invoke(target, amount);
    }
}
