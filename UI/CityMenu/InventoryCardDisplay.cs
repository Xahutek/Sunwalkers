using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryCardDisplay : MonoBehaviour
{
    public CityCaravanInventory motherHUD;
    public CardDisplay display;
    public GameObject isEquippedShade;

    public GameObject[]
        goldCards,
        silverCards,
        bronzeCards;

    public void Refresh(Card card, bool isEquipped)
    {
        bool isFilled = 0 < card.AnyRarity || isEquipped;

        gameObject.SetActive(isFilled);

        display.Refresh(card);

        isEquippedShade.SetActive(isEquipped);
        int
            goldRarity = card.GoldRarity,
            silverRarity = card.SilverRarity,
            bronzeRarity = card.BronzeRarity;

        for (int i = 0; i < bronzeCards.Length; i++)
        {
            bronzeCards[i].SetActive(i < bronzeRarity);
        }
        for (int i = 0; i < silverCards.Length; i++)
        {
            silverCards[i].SetActive(i < silverRarity);
        }
        for (int i = 0; i < goldCards.Length; i++)
        {
            goldCards[i].SetActive(i < goldRarity);
        }
    }

    public void Click()
    {
        motherHUD.SetInventoryCard(display);
    }
}
