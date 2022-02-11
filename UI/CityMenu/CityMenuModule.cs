using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityMenuModule : Menu
{
    public CityMenu menu;

    public CityProfile city
    {
        get { return menu.city; }
    }
    public Caravan caravan
    {
        get { return menu.caravan; }
    }
    public CaravanInventory inventory
    {
        get { return menu.inventory; }
    }

    public override void OnOpen()
    {
        menu = CityMenu.main;
        base.OnOpen();
    }
}
