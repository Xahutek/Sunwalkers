using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class CityCaravanProfile : CityMenuModule
{
    public CaravanHUDWagonCard[] WagonButtons;
    public Button[] ActiveAbilityButtons;
    public GameObject[] ActiveAbilityEquippedMarkers;
    [HideInInspector] public int currentActiveIndex = 0;
    public GameObject newWagonButton;

    [Header("Current Wagon Profile")]
    [HideInInspector]public Wagon currentWagon=null;
    [HideInInspector] public int currentWagonIndex = 0;

    public GameObject wagonProfile, ActiveProfile, wagonNormalImage, EngineWagonImage;
    public CardDisplay wagonCardDisplay;
    public Image
        WagonCurrentHealthBar;
    public TMP_Text
        WagonName,
        WagonHealth,
        HealPrice,
        UpgradeHullPrice;
    public Button
        HealButton,
        UpgradeHullButton;

    public override void OnOpen()
    {
        base.OnOpen();

        if (!currentWagon)
            SetCurrentWagon(0);
    }
    public override void Refresh()
    {
        RefreshWagonList();
        RefreshWagonProfile();
        RefreshActiveProfile();
    }


    public void RefreshWagonList()
    {
        for (int i = 0; i < inventory.wagons.Length; i++)
        {
            CaravanInventory.WagonProfile w = inventory.wagons[i];
            WagonButtons[i].gameObject.SetActive(w.unlocked);
            if (w.unlocked)
            {
                WagonButtons[i].Refresh(caravan.Wagons[i]);
            }
        }
        newWagonProfile = inventory.GetInactiveWagon(out newWagonIndex);
        newWagonButton.SetActive(newWagonProfile != null);
    }
    public void RefreshWagonProfile()
    {
        if (currentWagon)
        {
            WagonName.text = currentWagonIndex == 0 ? "Engine Wagon" : ("Wagon " + currentWagonIndex.ToString());
            WagonCurrentHealthBar.fillAmount = currentWagon.unitHealth / currentWagon.TrueMaxHealth();
            WagonHealth.text = currentWagon.unitHealth + " / " + currentWagon.TrueMaxHealth();

            bool isEngine = currentWagon.myProfile.type == WagonType.Engine;

            wagonNormalImage.SetActive(!isEngine);
            EngineWagonImage.SetActive(isEngine);

            if (!isEngine)
                wagonCardDisplay.Refresh(currentWagon.MasterCard);
            wagonCardDisplay.gameObject.SetActive(!isEngine);
            ActiveProfile.SetActive(isEngine);

            float
                wagonUpgradeCost = Caravan.main.WagonUpgradeHealthCost,
                wagonHealCost = city.WagonHealCost;

            HealPrice.text = wagonHealCost.ToString();
            UpgradeHullPrice.text = wagonUpgradeCost.ToString();

            HealButton.enabled = wagonHealCost <= inventory.Timber;
            UpgradeHullButton.enabled = wagonUpgradeCost <= inventory.Timber;
        }
        wagonProfile.SetActive(currentWagon);
        for (int i = 0; i < WagonButtons.Length; i++)
        {
            WagonButtons[i].Marked = currentWagonIndex == i;
        }
    }
    public void RefreshActiveProfile()
    {
        inventory.ReconciderActiveAbility();

        currentActiveIndex = (int)inventory.activeAbility.keyword;
        for (int i = 0; i < ActiveAbilityButtons.Length; i++)
        {
            if (i == 1)
                continue;
            Button b = ActiveAbilityButtons[i];
            b.gameObject.SetActive(inventory.HasKeywordEquipped((Keyword)i));
            ActiveAbilityEquippedMarkers[i].SetActive(currentActiveIndex == i);
        }
    }

    public void SetCurrentActive(int i)
    {
        currentActiveIndex = i;
        inventory.SetActiveAbility(i);

        RefreshActiveProfile();
    }

    public void SetCurrentWagonNull()
    {
        currentWagon = null;
        RefreshWagonProfile();
    }
    public void SetCurrentWagon(int index)
    {
        Wagon newWagon = GetWagon(index);

        if (!newWagon)
        {
            wagonProfile.SetActive(false);
            return;
        }
        wagonProfile.SetActive(true);

        currentWagonIndex = index;
        currentWagon = newWagon;

        menu.PlayClip(menu.BasicClick);

        if (gameObject.activeSelf)
            RefreshWagonProfile();
    }
    public async void ChangeWagonPosition(int AB)
    {
        int
            a=Mathf.FloorToInt(AB/10),
            b=AB-(a*10);
        Wagon
            WA = GetWagon(a),
            WB = GetWagon(b);
        if (!WA || !WB)
            return;

        WagonButtons[a].GetComponent<Animator>().SetTrigger("SwapRight");
        WagonButtons[b].GetComponent<Animator>().SetTrigger("SwapLeft");

        caravan.Wagons[a] = WB;
        caravan.Wagons[b] = WA;

        inventory.wagons[a] = WB.myProfile;
        inventory.wagons[b] = WA.myProfile;

        await Task.Delay(1);

        if (currentWagonIndex == a)
            SetCurrentWagon(b);
        else if (currentWagonIndex == b)
            SetCurrentWagon(a);

        menu.PlayClip(menu.BasicClick);

        RefreshWagonList();
    }

    public Wagon GetWagon(int index)
    {
        if (index < 0 || index >= inventory.wagons.Length)
        {
            Debug.LogWarning("The Wagon Index was out of bounds");
            return null;
        }

        Wagon newWagon = caravan.Wagons[index];

        if (!newWagon || !newWagon.myProfile.unlocked)
        {
            Debug.LogWarning("The Wagon Index was not unlocked");
            return null;
        }

        return newWagon;
    }

    public void HealWagon()
    {
        float wagonHealCost = city.WagonHealCost;

        if (currentWagon &&
            currentWagon.unitHealth < currentWagon.TrueMaxHealth() &&
            currentWagon && wagonHealCost <= inventory.Timber)
        {
            UIManager.main.Confirm(
                ExecuteHealWagon,
                wagonHealCost + " Timber to repair +" + city.WagonHealAmount + " Wagon Health");
        }
        else
            UIManager.main.Confirm(
                null,
                "You can not afford " + wagonHealCost + " Timber to heal this wagon.");
    }    
    public void ExecuteHealWagon()
    {
        float wagonHealCost = city.WagonHealCost;

        inventory.Timber -= wagonHealCost;
        currentWagon.isAlive = true;
        currentWagon.Heal(city.WagonHealAmount);

        menu.PlayClip(menu.TimberPurchase);

        Caravan.main.RefreshActiveWagons();
        Refresh();
    }

    public void UpgradeWagon()
    {
        float wagonUpgradeCost = Mathf.Max(1, Caravan.main.WagonUpgradeHealthCost + city.costModifier);

        if (currentWagon && wagonUpgradeCost <= inventory.Timber && currentWagon.myProfile.bonusHealthBuffs<8)
        {
            UIManager.main.Confirm(
                ExecuteUpgradeWagon,
                wagonUpgradeCost + " Timber",
                "+"+currentWagon.myProfile.healthBuffAmount+" Max Wagon Health");
        }
        else
            UIManager.main.Confirm(
                null,
                "You can not afford "+ wagonUpgradeCost + " Timber to upgrade your hull.");
    }
    public void ExecuteUpgradeWagon()
    {
        float wagonUpgradeCost = Mathf.Max(1, Caravan.main.WagonUpgradeHealthCost + city.costModifier);

        inventory.Timber -= wagonUpgradeCost;
        currentWagon.myProfile.bonusHealthBuffs++;
        currentWagon.Heal(currentWagon.myProfile.healthBuffAmount);
        currentWagon.isAlive=true;

        menu.PlayClip(menu.TimberPurchase);

        Caravan.main.RefreshActiveWagons();
        Refresh();
    }

    CaravanInventory.WagonProfile newWagonProfile = null;
    int newWagonIndex = 0;
    public void BuyWagon()
    {
        float wagonCost = Mathf.Max(1, Caravan.main.NewWagonCost + city.costModifier);
        newWagonProfile = inventory.GetInactiveWagon(out newWagonIndex);

        if ( newWagonProfile!=null && wagonCost <= inventory.Timber)
        {
            UIManager.main.Confirm(
                ExecuteBuyWagon,
                wagonCost + " Timber",
                " an additional Wagon");
        }
        else
        UIManager.main.Confirm(
            null,
            "You can not afford " + wagonCost + " Timber for a new wagon.");
    }
    public void ExecuteBuyWagon()
    {
        float wagonCost = Mathf.Max(1, Caravan.main.NewWagonCost + city.costModifier);
        inventory.Timber -= wagonCost;

        newWagonProfile.alive = true;
        newWagonProfile.unlocked = true;

        newWagonProfile.bonusSlots = 0;
        newWagonProfile.healthBuffAmount = 0;

        menu.PlayClip(menu.WagonPurchase);

        Caravan.main.RefreshActiveWagons();
        SetCurrentWagon(newWagonIndex);
        RefreshWagonList();
    }

    public void AlterCards()
    {
        menu.SelectModuleTab(2);
    }
}
