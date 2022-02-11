using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaravanHUD : MonoBehaviour
{
    Caravan caravan;

    public CaravanHUDWagonCard wagonCardPrefab;

    public Transform wagonListParent;
    public List<CaravanHUDWagonCard> WagonCards = new List<CaravanHUDWagonCard>();
    public CaravanHUDActiveAbility[] activeCards;
    public CaravanHUDResourceCard
        AmberResourceCard,
        TimberResourceCard;
    private void Start()
    {
        caravan = Caravan.main;
    }
    private void FixedUpdate()
    {
        for (int i = 0; i < caravan.Wagons.Length; i++)
        {
            Wagon w = caravan.Wagons[i];
            if (i>=WagonCards.Count)
                WagonCards.Add(Instantiate(wagonCardPrefab,wagonListParent));
            bool isValid = w.isAlive && w.myProfile.alive;
            WagonCards[i].gameObject.SetActive(isValid);
            if(isValid)
                WagonCards[i].Refresh(w, i==0);
        }

        foreach (CaravanHUDActiveAbility a in activeCards)
        {
            a.Refresh(caravan.activeAbility);
        }

        AmberResourceCard.Refresh(caravan.Amber);
        TimberResourceCard.Refresh(caravan.Timber);
    }
}

