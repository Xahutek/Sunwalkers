using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class CityShopResourceTrade : CityMenuModule
{
    public bool blockPurchase = false;

    public int
        offerAmber,
        offerTimber;

    public TMP_InputField
        AmberOfferText,
        TimberOfferText;
    public TMP_Text
        TimberReturnText,
        AmberReturnText;

    public void RefreshByInput()
    {
        int
            amber,
            timber;

        int.TryParse(AmberOfferText.text, out amber);
        int.TryParse(TimberOfferText.text, out timber);

        SetTimberOffer(timber);
        SetAmberOffer(amber);
    }
    public override void Refresh()
    {
        SetTimberOffer(offerTimber);
        SetTimberOffer(offerAmber);
    }

    public void SetTimberOffer(float offer)
    {
        offerTimber = offer <= 0 ? 0 : Mathf.FloorToInt(Mathf.Clamp(offer, 0, inventory.Timber));

        TimberOfferText.text = offerTimber.ToString();
        AmberReturnText.text = AmberReturn(offerTimber).ToString();
    }
    public void SetAmberOffer(float offer)
    {
        offerAmber = offer <= 0 ? 0 : Mathf.FloorToInt(Mathf.Clamp(offer, 0, inventory.Amber));

        AmberOfferText.text = offerAmber.ToString();
        TimberReturnText.text = TimberReturn(offerAmber).ToString();
    }

    public void AlterTimberOffer(int value) => SetTimberOffer(offerTimber + value);
    public void AlterAmberOffer(int value) => SetAmberOffer(offerAmber + value);

    public float TimberReturn(float offer)
    {
        if (offer <= 0)
            return 0;

        float pay = offer * city.AmberToTimberExchangeRate;
        return Mathf.Floor(pay * 10) / 10;
    }
    public float AmberReturn(float offer)
    {
        if (offer <= 0)
            return 0;

        float pay = offer * city.TimberToAmberExchangeRate;
        return Mathf.Floor(pay * 10) / 10;
    }

    public void BuyTimber()
    {
        float 
            Pay= offerAmber,
            Return = TimberReturn(offerAmber);
        if (blockPurchase || Pay <= 0 && Return <= 0)
            return;

        city.SellAmber(Mathf.CeilToInt(Return));

        inventory.Amber -= Pay;
        inventory.Timber += Return;

        menu.PlayClip(menu.TimberPurchase);

        SetAmberOffer(0);
        StartCoroutine(ExecuteBlockPurchase(0.1f));
    }
    public void BuyAmber()
    {
        float
            Pay = offerTimber,
            Return = AmberReturn(offerTimber);
        if (blockPurchase || Pay <= 0 && Return <= 0)
            return;

        city.SellAmber(-Mathf.CeilToInt(Return));

        inventory.Timber -= Pay;
        inventory.Amber += Return;

        menu.PlayClip(menu.AmberPurchase);

        SetTimberOffer(Pay);
        StartCoroutine(ExecuteBlockPurchase(0.5f));
    }

    public IEnumerator ExecuteBlockPurchase(float time)
    {
        blockPurchase = true;

        yield return new WaitForSeconds(time);

        blockPurchase = false;
    }
}
