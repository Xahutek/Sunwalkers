using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class CityCaravanInventory : CityMenuModule
{
    public CityCaravanProfile CaravanProfile;

    [Header("Card Inventory")]
    public Transform cardMother;
    public InventoryCardDisplay cardDisplayPrefab;
    public List<InventoryCardDisplay> cardDisplays = new List<InventoryCardDisplay>();

    public CardDisplay currentDisplay;
    public Button unequipCardButton, equipCardButton;

    public TMP_InputField searchBar;

    public bool keywordFilterActive, rarityFilterActive;
    public Keyword KeywordFilter;
    public int RarityFilter;
    public string NameFilter;
    public GameObject[] FilterMarker, GemMarker;

    [Header("Current Wagon Profile")]
    public CaravanHUDWagonCard[] WagonButtons;

    [HideInInspector] public Wagon currentWagon
    {
        get { return CaravanProfile.currentWagon; }
        set { CaravanProfile.currentWagon = value; }
    }
    [HideInInspector] public int currentWagonIndex 
    { 
        get { return CaravanProfile.currentWagonIndex; }
        set
        {
            CaravanProfile.currentWagonIndex = value;
        }
    }

    public GameObject wagonProfile,wagonList;
    public CardDisplay wagonCardDisplay;
    public Image
        WagonCurrentHealthBar;
    public TMP_Text
        WagonName,
        WagonHealth;

    public override void OnOpen()
    {
        ResetFilters();
        if (!currentWagon||currentWagon.myProfile.type == WagonType.Engine)
            SetCurrentWagonNull();
        base.OnOpen();
    }

    public override void Refresh()
    {
        RefreshWagonList();
        RefreshWagonProfile();
        RefreshInventoryDisplay();
    }

    public void RefreshWagonList()
    {
        for (int i = 0; i < inventory.wagons.Length; i++)
        {
            CaravanInventory.WagonProfile w = inventory.wagons[i];
            WagonButtons[i].gameObject.SetActive(w.unlocked&&w.alive);

            if (w.unlocked&&w.alive)
            {
                WagonButtons[i].Refresh(caravan.Wagons[i]);
            }
        }
        wagonList.SetActive(!currentWagon);
    }
    public void RefreshWagonProfile()
    {
        if (currentWagon)
        {
            WagonName.text = currentWagonIndex == 0 ? "Engine Wagon" : ("Wagon " + currentWagonIndex.ToString());
            WagonCurrentHealthBar.fillAmount = currentWagon.unitHealth / currentWagon.TrueMaxHealth();
            WagonHealth.text = currentWagon.unitHealth + " / " + currentWagon.TrueMaxHealth();

            Card c = currentDisplay ? currentDisplay.card : null;
            bool isEquipped = c == currentWagon.MasterCard;
            wagonCardDisplay.Refresh(c?c:currentWagon.MasterCard);
            unequipCardButton.interactable = isEquipped && currentWagon.MasterCard != inventory.BasicCard;
            equipCardButton.interactable = !isEquipped && c != inventory.BasicCard;
        }
        wagonProfile.SetActive(currentWagon);
        wagonList.SetActive(!currentWagon);

    }

    public void SetUpDisplayList()
    {
        if (cardMother.childCount > 0)
            foreach (Transform t in cardMother)
            {
                Destroy(t.gameObject);
            }

        InventoryCardDisplay basic = Instantiate(cardDisplayPrefab, cardMother);
        basic.motherHUD = this;
        basic.Refresh(inventory.BasicCard, false);

        foreach (Card c in inventory.allCards)
        {
            InventoryCardDisplay dis = Instantiate(cardDisplayPrefab, cardMother);
            cardDisplays.Add(dis);
            dis.motherHUD = this;
            dis.Refresh(c, inventory.CardIsEquipped(c));
        }
    }
    public void RefreshInventoryDisplay()
    {
        if (cardDisplays.Count == 0)
        {
            SetUpDisplayList();
        }
        else for (int i = 0; i < inventory.allCards.Count; i++)
            {
                InventoryCardDisplay c = cardDisplays[i];
                Card card = inventory.allCards[i];
                c.motherHUD = this;
                c.Refresh(card, inventory.CardIsEquipped(card));
                c.display.Marked = currentDisplay && c == currentDisplay.card;
                c.gameObject.SetActive(PassesFilter(card));
            }
    }
    public bool PassesFilter(Card card)
    {
        if (card.AnyRarity == 0)
            return false;

        if (keywordFilterActive)
        {
            bool hasKeyword = false;
            foreach (Keyword k in card.keywords)
            {
                if(k==KeywordFilter)
                {
                    hasKeyword = true;
                    break;
                }
            }
            if (!hasKeyword)
                return false;
        }

        if (rarityFilterActive)
        {
            if (card.Level-1 != RarityFilter)
                return false;
        }

        if (NameFilter!="")
        {
            if (!card.name.ToLower().Contains(NameFilter.ToLower()))
                return false;
        }
        return true;
    }

    public void SetCurrentWagon(int index)
    {
        menu.PlayClip(menu.BasicClick);

        CaravanProfile.SetCurrentWagon(index);

        Card master = currentWagon.MasterCard;
        foreach (InventoryCardDisplay d in cardDisplays)
        {
            if (d.display.card == master)
            {
                SetInventoryCard(d.display);
                return;
            }
        }

        RefreshWagonProfile();
    }
    public void SetCurrentWagonNull()
    {
        CaravanProfile.SetCurrentWagonNull();
        RefreshWagonProfile();
    }

    public void SetInventoryCard(CardDisplay display)
    {
        menu.PlayClip(menu.BasicClick);

        currentDisplay = display;
        RefreshWagonProfile();
    }

    public void SetKeywordFilter(int index)
    {
        Keyword newFilter = (Keyword)index;
        FilterMarker[(int)KeywordFilter].SetActive(false);

        menu.PlayClip(menu.BasicClick);

        if (keywordFilterActive && KeywordFilter == newFilter)
        {
            keywordFilterActive = false;
            RefreshInventoryDisplay();
            return;

        }

        keywordFilterActive = true;
        KeywordFilter = newFilter;

        FilterMarker[(int)KeywordFilter].SetActive(true);

        RefreshInventoryDisplay();
    }
    public void SetRarityFilter(int rarity)
    {
        GemMarker[RarityFilter].SetActive(false);

        menu.PlayClip(menu.BasicClick);

        if (rarityFilterActive && rarity==RarityFilter)
        {
            keywordFilterActive = false;
            RefreshInventoryDisplay();
            return;
        }

        rarityFilterActive = true;
        RarityFilter = rarity;
        GemMarker[rarity].SetActive(true);

        RefreshInventoryDisplay();
    }
    public void SetNameFilter()
    {
        NameFilter = searchBar.text;
        RefreshInventoryDisplay();
    }
    public void ResetFilters()
    {
        NameFilter = "";
        keywordFilterActive = false;
        rarityFilterActive= false;

        searchBar.text = "";
        foreach (GameObject f in FilterMarker)
        {
            f.SetActive(false);
        }
        foreach (GameObject f in GemMarker)
        {
            f.SetActive(false);
        }

        RefreshInventoryDisplay();
    }

    public void EquipCard()
    {
        if (currentDisplay && currentDisplay.card && currentWagon)
        {
            currentWagon.myProfile.AddCard(currentDisplay.card, 0);
            inventory.ReconciderActiveAbility();
            Refresh();

            menu.PlayClip(menu.BasicClick);
        }
    }
    public void UnequipCard()
    {
        if (currentDisplay && currentDisplay.card && currentWagon)
        {
            currentWagon.myProfile.AddCard(inventory.BasicCard, 0);
            Refresh();

            menu.PlayClip(menu.BasicClick);
        }
    }

    public void Done()
    {
        menu.PlayClip(menu.BasicClick);

        SetCurrentWagonNull();
    }
}
