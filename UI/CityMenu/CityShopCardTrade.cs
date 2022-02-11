using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class CityShopCardTrade : CityMenuModule
{
    public GameObject[] OfferSlides;
    public CardDisplay[] OfferDisplay;
    public TMP_Text[] OfferPriceTag;

    public TMP_Text RerollTag;

    public override void Refresh()
    {
        RefreshCardOffers();
        base.Refresh();
    }

    public void RefreshCardOffers()
    {
        Dictionary<Card, Vector2Int> CardOffers = city.CardOffers;
        if (CardOffers == null)
            CardOffers = new Dictionary<Card, Vector2Int>();

        int i = 0;
        foreach (Card c in CardOffers.Keys)
        {
            OfferSlides[i].SetActive(!city.bought[i]);

            OfferDisplay[i].Refresh(c);

            OfferPriceTag[i].text = CardOffers[c].y.ToString();

            i++;
        }

        RerollTag.text = city.RerollCardOffersPrice.ToString();
    }
    public void BuyCard(int index)
    {
        Vector2 stats = city.CheckCardStats(index);
        lastIndex = index;
        UIManager.main.Confirm(
            ExecuteBuyCard,
            stats.y.ToString(),
            city.CheckCardOffer(index).name + (stats.x > 0 ? " (Rare)" : ""));
    }
    int lastIndex;
    public void ExecuteBuyCard()
    {
        menu.PlayClip(menu.AmberPurchase);

        city.BuyCardOffer(lastIndex);
        RefreshCardOffers();
    }

    public void Reroll()
    {
        if (city.RerollCardOffersPrice <= inventory.Amber)
            UIManager.main.Confirm(
                        ExecuteReroll,
                        "Pay "+city.RerollCardOffersPrice+" to Reroll card offers and prices.");
    }
    public void ExecuteReroll()
    {
        menu.PlayClip(menu.AmberPurchase);

        inventory.Amber -= city.RerollCardOffersPrice;
        city.NextReputation += city.RerollCardOffersPrice;
        city.RandomizeCardOffer();

        RefreshCardOffers();
    }
}
