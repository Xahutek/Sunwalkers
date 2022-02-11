using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOfInterest : MonoBehaviour, ResettableObject
{
    public bool unlocked=false, enabeled=false, forceDisable=false;

    [Header("Components")]
    [HideInInspector]public Animator anim;
    public ParticleSystem pickUpParticles;
    public GameObject tooltip;

    [Header("Conditions")]
    public List<CombatArena> arenaLock = new List<CombatArena>();
    public bool InstantPickUp = false;

    [Header("Effects")]
    public CityProfile CityIdentity;
    public Menu openMenu=null;
    public AudioClip interactSound;

    [SerializeField] Vector2Int amberReward, timberReward;
    public int AmberReward
    {
        get
        {
            return Random.Range(amberReward.x,amberReward.y);
        }
    }
    public int TimberReward
    {
        get
        {
            return Random.Range(timberReward.x, timberReward.y);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (Caravan.main.Wagons[0].gameObject != other.gameObject)
            return;

        enabeled = unlocked&&!forceDisable;

        tooltip.gameObject.SetActive(enabeled&&!InstantPickUp);

        anim.SetBool("enabled", enabeled);
    }
    private void OnTriggerExit(Collider other)
    {
        if (Caravan.main.Wagons[0].gameObject != other.gameObject)
            return;

        enabeled = false;

        tooltip.gameObject.SetActive(false);

        anim.SetBool("enabled", enabeled);
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("gone", false);
    }
    private void Start()
    {
        if (CityIdentity)
            CityIdentity.Reset();
    }
    private void Update()
    {
        if (!isClosing&&enabeled && (InstantPickUp || Input.GetKeyDown(KeyCode.E)) && (!openMenu || !UIManager.main.active))
            Interact();
    }
    private void FixedUpdate()
    {
        if (!forceDisable)
        {
            unlocked = false;

            if (arenaLock.Count > 0)
            {
                foreach (CombatArena arena in arenaLock)
                {
                    if (arena.isActive && arena.allDead)
                    {
                        unlocked = true;
                        break;
                    }
                }
            }
            else
                unlocked = true;
        }
        
        anim.SetBool("unlocked", unlocked&&!forceDisable);
    }
    public virtual void Interact()
    {
        Debug.Log("Interacted with " + name);
        enabeled = false;

        if (interactSound)
            SoundManager.main.PlayOneShot(interactSound);

        CaravanInventory inventory = Caravan.main.inventory;
        inventory.Amber += AmberReward;
        inventory.Timber += TimberReward;

        if (CityIdentity)
        {
            CaravanInventory.lastCity = CityIdentity;
            CityIdentity.Repair(Caravan.main);
        }
        else
            StartCoroutine(ExecuteClose());

        if (openMenu != null)
            openMenu.TriggerOpen(this);
    }
    bool isClosing = false;
    public IEnumerator ExecuteClose()
    {
        if (isClosing)
            yield break;
        isClosing = true;
        anim.SetBool("gone", true);
        pickUpParticles.Play();
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
        isClosing = false;
    }
    public void OnEnable()
    {
        anim.SetBool("gone", false);
    }

    public void OnReset()
    {
        if (CityIdentity)
            CityIdentity.ResetCycle();
        anim.SetBool("gone", false);
    }
}
