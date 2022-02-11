using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CaravanHUDActiveAbility : MonoBehaviour
{
    public Keyword k;
    public Image chargefill, chargeFillBackground;

    public void Refresh(ActiveAbility a)
    {
        gameObject.SetActive(a.keyword == k);
        if (a.keyword == k)
        {
            chargefill.fillAmount = a.Charge;

            Color c = DataBase.Keywords.GetColor(a.keyword);
            chargefill.color = a.SufficientCharge ? c : Color.Lerp(c, Color.black, 0.5f);
            chargeFillBackground.color = Color.Lerp(c, Color.black, 0.8f);
        }
    }
}
