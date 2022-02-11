using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Active Abilities/Ranger")]
public class RangerActiveAbility : ActiveAbility
{
    public DirectedProjectile volleyPrefab;
    public int shotDamage;
    public float 
        shotRange,
        shotHitbox,
        shotSpeed;
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


        AttackInfo shot = new AttackInfo();
        shot.SetUpAll(null,
            DataBase.Entities.LMEnemy, c.position, MousePos(), null,
            Mathf.RoundToInt(Mathf.Lerp(0, shotDamage * (1+0.5f * ((float)powerBonus - 1)), power)),
            Mathf.Lerp(shotRange*0.5f, shotRange, power),
            shotHitbox, shotSpeed, 0, 0, 0, 0, false, volleyPrefab);

        SoundManager manager = SoundManager.main;
        shot.SetUpAudio(manager.GetOneShot(keyword, true), manager.GetOneShot(keyword));

        shot.windUpTime = 0.2f;

        Projectile p =Projectile.Pool.Projectile(volleyPrefab);
        p.transform.localScale = Vector3.one * power;

        shot.PlayEffect(true);
        p.Shoot(shot);
    }

    //public Vector2 
    //    damage = new Vector2(0,1),
    //    range = new Vector2(2,10);
    //public float projectileSpeed, projectileHitbox;

    //public DirectedProjectile volleyPrefab;
    //public override void Attack(Caravan c, float power)
    //{
    //    Wagon[] wagons = c.activeWagons.ToArray();

    //    float
    //        thisRange = Mathf.Lerp(range.x, range.y, power);
    //    int
    //        thisDamage = Mathf.RoundToInt(Mathf.Lerp(damage.x, damage.y, power));

    //    AttackInfo shot = new AttackInfo();
    //    shot.SetUpAll(DataBase.Entities.LMEnemy, Vector3.zero, Vector3.zero, null, thisDamage,thisRange,projectileHitbox,projectileSpeed,0,0,0,0,false,volleyPrefab);
    //    shot.windUpTime = 0.2f;

    //    for (int i = 1; i < wagons.Length; i++)
    //    {
    //        Wagon w = wagons[i];

    //        if (!w.isAlive)
    //            continue;

    //        shot.origin = w.position;

    //        shot.destination = Globe.Ground(w.position - w.transform.right*thisRange);
    //        Projectile.Pool.Projectile(volleyPrefab).Shoot(shot);

    //        shot.destination = Globe.Ground(w.position + w.transform.right * thisRange);
    //        Projectile.Pool.Projectile(volleyPrefab).Shoot(shot);
    //    }
    //}
}
