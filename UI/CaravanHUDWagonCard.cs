using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaravanHUDWagonCard : MonoBehaviour
{
    public Sprite
        EngineSprite,
        NormalSprite,
        LastSprite;
    public Sprite
        MarkerEngineSprite,
        MarkerNormalSprite,
        MarkerLastSprite;
    [SerializeField]
    Image
        HealthBar,
        ShieldBar,
        ChargeBar,
        Picture,
        Marker;
    [SerializeField ]Image[] ClassIcons;

    public bool Marked
    {
        set
        {
            Marker.gameObject.SetActive(value);
        }
    }

    public void Refresh(Wagon w, bool chargeFillsUp = false)
    {
        HealthBar.fillAmount = w.unitHealth / w.TrueMaxHealth();
        ShieldBar.fillAmount = w.shieldHealth / w.maxShieldHealth;

        //ChargeBar.fillAmount = chargeFillsUp ? w.Charge : 1 - w.Charge;

        Keyword[] k = w.MasterCard.keywords.ToArray();

        Picture.sprite = w.myProfile.type == WagonType.Engine ? EngineSprite :
            (w.isLast ? LastSprite : NormalSprite);
        Marker.sprite = w.myProfile.type == WagonType.Engine ? MarkerEngineSprite :
            (w.isLast ? MarkerLastSprite : MarkerNormalSprite);

        for (int i = 0; i < 2; i++)
        {
            Image icon = ClassIcons[i];
            bool hasKeyowrd = k.Length > i;
            icon.gameObject.SetActive(hasKeyowrd);
            if (hasKeyowrd)
                icon.sprite = DataBase.Keywords.GetIcon(k[i]);
        }
    }
}