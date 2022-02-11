using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontRam : MonoBehaviour
{
    LayerMask LMEnemy, LMWalls;
    Caravan caravan;

    public float ramCooldown=1;
    float ramWallCooldownTimer=0;
    private void Start()
    {
        LMEnemy = DataBase.Entities.LMEnemy;
        LMWalls = DataBase.Entities.LMWalls;
        caravan = Caravan.main;
    }
    private void OnTriggerEnter(Collider other)
    {
        GameObject hitObject = other.gameObject;

        if (LMWalls.Contains(hitObject.layer))
        {
            if (Globe.time >= ramWallCooldownTimer)
            {
                ramWallCooldownTimer = Globe.time + ramCooldown;
                caravan.RamWall();
            }
        }
        else if (LMEnemy.Contains(hitObject.layer))
        {
            Enemy hitEnemy = hitObject.GetComponent<Enemy>();

            if (hitEnemy)
                caravan.RamEnemy(hitEnemy);
        }
    }
}
