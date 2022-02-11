using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    [SerializeField] private DirectedProjectile DirectedProjectilePrefab;
    [SerializeField] private TargetedProjectile TargetedProjectilePrefab;
    [SerializeField] private SpotProjectile SpotProjectilePrefab;
    [SerializeField] private BurstProjectile BurstProjectilePrefab;

    [HideInInspector] public Dictionary<Projectile, List<Projectile>> PoolProjectiles = new Dictionary<Projectile, List<Projectile>>();
    [HideInInspector] public Dictionary<Placable, List<Placable>> PoolPlacables = new Dictionary<Placable, List<Placable>>();
    private void Awake()
    {
        global::Projectile.Pool = this;
        global::Placable.Pool = this;
    }

    public Placable Placable(Placable prefab)
    {
        if (PoolPlacables.ContainsKey(prefab))
        {
            if (PoolPlacables[prefab] != null)

                foreach (Placable p in PoolPlacables[prefab])
                {
                    if (!p.deployed && !p.active)
                        return p;
                }
            else
                PoolPlacables[prefab] = new List<Placable>();
        }
        else
            PoolPlacables.Add(prefab, new List<Placable>());

        Placable newPlacable = Instantiate(prefab, transform);
        PoolPlacables[prefab].Add(newPlacable);
        return newPlacable;
    }

    public Projectile Projectile(Projectile prefab)
    {
        if (PoolProjectiles.ContainsKey(prefab))
        {
            if (PoolProjectiles[prefab] != null)

                foreach (Projectile p in PoolProjectiles[prefab])
                {
                    if (!p.isShooting)
                        return p;
                }
            else
                PoolProjectiles[prefab] = new List<Projectile>();
        }
        else
            PoolProjectiles.Add(prefab, new List<Projectile>());

        Projectile newProjectile = Instantiate(prefab, transform);
        PoolProjectiles[prefab].Add(newProjectile);
        return newProjectile;
    }
    public DirectedProjectile DirectedProjectile(DirectedProjectile prefab=null)
    {
       return Projectile(prefab ? prefab : DirectedProjectilePrefab) as DirectedProjectile;
    }
    public TargetedProjectile TargetProjectile(TargetedProjectile prefab=null)
    {
        return Projectile(prefab ? prefab : TargetedProjectilePrefab) as TargetedProjectile;
    }
    public SpotProjectile SpotProjectile(SpotProjectile prefab = null)
    {
        return Projectile(prefab ? prefab : SpotProjectilePrefab) as SpotProjectile;
    }
    public BurstProjectile BurstProjectile(BurstProjectile prefab = null)
    {
        return Projectile(prefab ? prefab : BurstProjectilePrefab) as BurstProjectile;
    }
}
