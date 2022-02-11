using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityMenu : Menu
{
    public static CityMenu main;
    public CityProfile city;
    public Caravan caravan;
    public CaravanInventory inventory;
    Animator animator;

    [Header("Sub-Modules")]
    public CityMenuModule CityProfile;
    public CityMenuModule CaravanProfile;
    public CityMenuModule CardInventory;
    public CityMenuModule CityShop;

    public CaravanHUDResourceCard
        AmberResourceCard,
        TimberResourceCard;

    [Header("Sounds")]
    public AudioClip BasicClick;
    public AudioClip
        NormalPurchase,
        AmberPurchase,
        TimberPurchase,
        WagonPurchase,
        BoatPurchase,
        CavePurchase;
    public CityReputationBar mainRep;
    public GameObject SpecialExitButtonOcean, SpecialExitButtonMountains;

    public void PlayClip(AudioClip clip)
    {
        if(clip &&SoundManager.main)
            SoundManager.main.PlayOneShot(clip);
    }

    protected override void Awake()
    {
        main= this;
        animator= GetComponent<Animator>();
        base.Awake();
    }
    public override void OnOpen()
    {
        if (!main) main = this;
        caravan = Caravan.main;
        inventory = caravan.inventory;
        city = CaravanInventory.lastCity;

        if (city)
        {
            SoundManager.main.EnterCity(CaravanInventory.lastCity);
            base.OnOpen();
            inventory.ReconciderActiveAbility();
            SelectFirstTab();
            SpecialExitButtonOcean.SetActive(city.isHajabaku);
            SpecialExitButtonMountains.SetActive(city.isKorLibet);
        }
    }
    public override void OnClose()
    {
        SoundManager.main.LeaveCity();
        caravan.RefreshActiveWagons();
        caravan.ReconciderLoadout();
        caravan.position = caravan.position;
        base.OnClose();
    }
    public override void Refresh()
    {

    }
    
    public void SelectFirstTab() => SelectModuleTab(0);
    public void SelectModuleTab(int tab)
    {
        if (tab < 0 || tab > 3)
            return;

        animator.SetInteger("Tab", tab);

        PlayClip(BasicClick);

        CityMenuModule m = null;
        switch (tab)
        {
            case 0: m=CityProfile; break;
            case 1: m=CaravanProfile; break;
            case 2: m=CardInventory; break;
            case 3: m=CityShop; break;
            default:
                break;
        }
        if (m)
        {
            m.OnOpen();
        }
    }

    //public void SpendAmber(int amount)
    //{
    //    city.Reputation
    //}

    private void FixedUpdate()
    {
        mainRep.Refresh();
        AmberResourceCard.Refresh(caravan.Amber);
        TimberResourceCard.Refresh(caravan.Timber);
    }
}
