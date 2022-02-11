using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Active Abilities/Shaman")]
public class ShamanActiveAbility : ActiveAbility
{
    public Totem totemPrefab;
    public BurstProjectile shotPrefab;
    public float totemSplashRadius, totemReload;
    public int totemDamage;
    public float totemMaxTime;

    public override void Activate(Caravan c)
    {
        base.Activate(c);

        Vector3 pos = c.position;
        AttackInfo shot = new AttackInfo();
        shot.SetUpAll(null,
            DataBase.Entities.LMEnemy, pos, pos, null,
            Mathf.RoundToInt((float)totemDamage * (1f + 0.5f * ((float)powerBonus - 1f))), totemSplashRadius,
            totemSplashRadius, totemMaxTime * Charge, 0, totemReload, totemSplashRadius, 0.35f, false, shotPrefab);

        SoundManager manager = SoundManager.main;
        shot.SetUpAudio(manager.GetOneShot(keyword, true), manager.GetOneShot(keyword));

        Totem t = Projectile.Pool.Placable(totemPrefab) as Totem;
        t.SetUp(shot);

        charge.x = 0;
    }
}
