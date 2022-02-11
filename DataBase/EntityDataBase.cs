using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/EntityDataBase")]
public class EntityDataBase : ScriptableObject
{
    public LayerMask
        LMHittable, LMWagon, LMEnemy, LMGlobe, LMEnvironment, LMOcean, LMWalls;

    public HittableGlobeEntity GetClosestHittable(Vector3 origin, float radius) => GetClosestHittableMask(origin, radius, LMHittable);
    public Wagon GetClosestWagon(Vector3 origin, float radius)
    {
        HittableGlobeEntity target = GetClosestHittableMask(origin, radius, LMWagon);
        return target == null ? null : target as Wagon;
    }
    public Enemy GetClosestEnemy(Vector3 origin, float radius)
    {
        HittableGlobeEntity target = GetClosestHittableMask(origin, radius, LMEnemy);
        return target == null ? null : target as Enemy;
    }
    private HittableGlobeEntity GetClosestHittableMask(Vector3 origin, float radius, LayerMask LM)
    {
        Collider[] closeEnemies = Physics.OverlapSphere(origin, radius, LM);
        if (closeEnemies.Length == 0)
            return null;

        Collider closestCol = null;
        float closestDistance = radius + 1;
        foreach (Collider col in closeEnemies)
        {
            Vector3 distance = col.transform.position - origin;
            if (!closestCol || distance.magnitude < closestDistance)
            {
                closestDistance = distance.magnitude;
                closestCol = col;
            }
        }
        return closestCol.GetComponent<HittableGlobeEntity>();
    }
}
