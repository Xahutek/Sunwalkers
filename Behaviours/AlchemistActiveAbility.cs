using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Active Abilities/Alchemist")]
public class AlchemistActiveAbility : ActiveAbility
{
    public Landmine landminePrefab;
    public float mineSplashRadius, mineActiveDuration;
    public int mineDamage;

    public override void Press(Caravan c)
    {
        base.Press(c);

        if (Globe.time >= reloadTimer)
            Attack(c, Charge);
    }
    public override void Attack(Caravan c, float power)
    {
        charge.x -= minChargeCost;
        reloadTimer = Globe.time + minReload;
        Vector3 pos = Globe.Ground(c.position + new Vector3(Random.value, Random.value, Random.value));
        AttackInfo shot = new AttackInfo();
        shot.SetUpAll(null,
            DataBase.Entities.LMEnemy, pos, pos, null,
            Mathf.RoundToInt(mineDamage * (1 + 0.5f * ((float)powerBonus - 1))), 0, mineSplashRadius, mineActiveDuration, 0, 0, mineSplashRadius);

        SoundManager manager = SoundManager.main;
        shot.SetUpAudio(manager.GetOneShot(keyword, true), manager.GetOneShot(keyword));

        shot.windUpTime = 0.35f;

        Landmine p = Projectile.Pool.Placable(landminePrefab) as Landmine;

        shot.PlayEffect(true);
        p.SetUp(shot);
    }
}
