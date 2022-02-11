using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class CityShopMenu : CityMenuModule
{
    [Header("Sub-Modules")]
    public CityShopCardTrade CardTrade;
    public CityShopResourceTrade ResourceTrade;

    public override void OnOpen()
    {
        base.OnOpen();
        ResourceTrade.blockPurchase = false;
    }
    public override void Refresh()
    {
        CardTrade.Refresh();
        ResourceTrade.Refresh();
    }
}
