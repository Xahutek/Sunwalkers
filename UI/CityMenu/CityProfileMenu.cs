using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class CityProfileMenu : CityMenuModule
{
    public Image
        CityBackground;
    public TMP_Text
        CityNameTop,
        CityNameProfile,
        CityDescription;

    public CityReputationBar
        BarTop,
        BarDescription;
    //public TMP_Text
    //    CityReputation;
    public TMP_Text
        //CityReputationReputationTitle,
        CityReputationDescription;

    public override void OnOpen()
    {
        BarTop.OnOpen();
        BarDescription.OnOpen();
        base.OnOpen();
    }
    public override void Refresh()
    {
        CityBackground.sprite = city.Background;

        CityNameTop.text = city.name;
        CityNameProfile.text = city.name;
        CityDescription.text = city.description;
        CityNameProfile.color = city.colMain;

        //CityReputation.text = city.Reputation+"/"+city.MaxReputation;
        //CityReputationReputationTitle.text = city.reputationLevel.title;
        CityReputationDescription.text = city.reputationLevel.description;

        RefreshReputationBars();
    }
    public void RefreshReputationBars()
    {
        BarTop.Refresh();
        BarDescription.Refresh();
    }

    public void ExitCity()
    {
        menu.PlayClip(menu.BasicClick);
        UIManager.main.CloseMenu(menu);
    }
    public void ExitCitySpecial()
    {
        if (inventory.Amber >= city.PassagePrice && 
            (city.isHajabaku || city.isKorLibet))
            UIManager.main.Confirm(
                ExecuteSpecialExit,
                "Pay " + city.PassagePrice + " for save passage " + 
                (city.isHajabaku ? "across the Ocean." : "through the Caves."));
    }

    public void ExecuteSpecialExit()
    {
        if (city.isHajabaku)
        {
            menu.PlayClip(menu.BoatPurchase);
            FastTravelManager.main.TeleportCaravanToHajabimbim();
        }
        else if (city.isKorLibet)
        {
            menu.PlayClip(menu.CavePurchase);
            FastTravelManager.main.TeleportCaravanToKorNabel();
        }

        inventory.Amber -= city.PassagePrice;

        UIManager.main.CloseMenu(menu);
    }
}
