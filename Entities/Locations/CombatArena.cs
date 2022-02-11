using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatArena : MonoBehaviour, ResettableObject
{
    public static List<CombatArena> currentArenas= new List<CombatArena>();
    public static bool CaravanInCombat
    {
        get
        {
            foreach (CombatArena a in currentArenas)
            {
                if (a.CombatInProgress)
                    return true;
            }
            return false;
        }
    }
    public bool isActive = false, allDead = false;

    public bool CombatInProgress
    {
        get
        {
            return isActive && !allDead;
        }
    }

    public PointOfInterest AmberDrop, TimberDrop, EventDrop;
    public PointOfInterest[] Additional;

    public float maxActiveEnemies;
    public Enemy[] myEnemies;
    public Enemy[] enabeledEnemies
    {
        get
        {
            List<Enemy> activeEnemies = new List<Enemy>();
            foreach (Enemy e in myEnemies)
            {
                if (e.EntityEnabeled)
                    activeEnemies.Add(e);
            }
            return activeEnemies.ToArray();
        }
    }
    public Dictionary<Enemy, Vector3> RespawnPoints;

    private void Start()
    {
        List<Enemy> collectedE = new List<Enemy>();
        foreach (Transform t in transform.parent)
        {
            if (t != transform)
            {
                Enemy e = t.GetComponent<Enemy>();
                if (e && e.gameObject.activeSelf)
                    collectedE.Add(e);
            }
        }
        myEnemies = collectedE.ToArray();

        if (AmberDrop) AmberDrop.arenaLock.Add(this);
        if (TimberDrop) TimberDrop.arenaLock.Add(this);
        if (EventDrop)
        {
            EventDrop.arenaLock.Add(this);
            EventDrop.gameObject.SetActive(false);
        }
        foreach (PointOfInterest p in Additional)
        {
            p.arenaLock.Add(this);
        }

        RefreshRespawn();
        Respawn();
    }
    public void RefreshRespawn()
    {
        RespawnPoints = new Dictionary<Enemy, Vector3>();

        foreach (Enemy e in myEnemies)
        {
            RespawnPoints.Add(e,e.respawnPoint);
        }
    }
    public Enemy[] RefreshRoster()
    {
        for (int i = 0; i < myEnemies.Length; i++)
        {
            Enemy e = myEnemies[i];
            e.EntityEnabeled = i < maxActiveEnemies;
        }
        Enemy[] actives = enabeledEnemies;

        return actives;
    }

    public void Respawn()
    {
        allDead = false;

        Enemy[] actives = RefreshRoster();

        foreach (Enemy e in actives)
        {
            e.Respawn();
        }

        RandomizeResources();
    }

    public void RandomizeResources()
    {
        bool dropsAmber = AmberDrop?(TimberDrop? Random.value < 0.5f : true):false;

        if (AmberDrop)
        {
            AmberDrop.gameObject.SetActive(dropsAmber);        
        }
        if (TimberDrop)
        {
            TimberDrop.gameObject.SetActive(!dropsAmber);
        }
    }

    private void FixedUpdate()
    {
        bool wasActive = isActive;
        isActive = currentArenas.Contains(this);

        if (isActive)
        {
            allDead = true;
            foreach (Enemy e in myEnemies)
            {
                if (e.gameObject.activeSelf)
                {
                    allDead = false;
                    break;
                }
            }
        }

        if (!wasActive && isActive)
        {
            Enemy[] actives = enabeledEnemies;
            foreach (Enemy e in actives)
            {
                e.InCombat = true;
            }
        }
        else if (wasActive && !isActive)
        {
            Enemy[] actives = enabeledEnemies;
            foreach (Enemy e in actives)
            {
                e.InCombat = false;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (Caravan.main.Wagons[0].gameObject != other.gameObject)
            return;

        if (!isActive)
            currentArenas.Add(this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (Caravan.main.Wagons[0].gameObject != other.gameObject)
            return;

        if (isActive)
            currentArenas.Remove(this);
    }

    public void OnReset()
    {
        Respawn();
    }
}
