using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/CaravanInventory")]
public class CaravanInventory : ScriptableObject
{
    public bool CheatMode = false, resetOnStart;
    public Card StartCard;

    public static CityProfile lastCity;

    [Header("Cards & Wagons")]
    public Card BasicCard;
    public List<Card> allCards;

    public ActiveAbility[] allActiveAbilities;
    public ActiveAbility activeAbility;

    public WagonProfile[] wagons = new WagonProfile[5];
    Dictionary<Keyword, int> keywords = new Dictionary<Keyword, int>();
    [System.Serializable] public class WagonProfile
    {
        public bool unlocked = true, alive=true;
        public WagonType type;
        [Range(0, 3)] public int bonusSlots = 0;
        [Range(0, 8)] public int bonusHealthBuffs = 0;
        public int healthBuffAmount;
        [SerializeField]private Card[] cards = new Card[3];
        public Card[] Cards
        {
            get
            {
                return cards;
            }
        }
        public void ClearCards()=>cards= new Card[3];
        public int CardSlots
        {
            get
            {
                return 1 + bonusSlots;
            }
        }
        public int TotalHealthBuff
        {
            get
            {
                return (healthBuffAmount * bonusHealthBuffs);
            }
        }

        public bool AllCardSlotsUnlocked
        {
            get
            {
                return bonusSlots >= 2;
            }
        }
        public bool AllHealthBuffsUnlocked
        {
            get
            {
                return bonusHealthBuffs >= 3;
            }
        }

        public bool AddCard(Card c, int index)
        {
            if (!c || index < 0 || index >= Cards.Length)
                return false;

            if (Cards[index])
                Cards[index].UnSubscribeEvents();

            c.SubscribeEvents();
            Cards[index] = c;

            return true;
        }
    }

    [Header("Resources")]

    [SerializeField] float amber;

    [SerializeField] float timber;

    public float Amber
    {
        get
        {
            return amber;
        }
        set
        {
            amber = Mathf.Max(0, value);
        }
    }
    public float Timber
    {
        get
        {
            return timber;
        }
        set
        {
            timber = Mathf.Max(0, value);
        }
    }

    public void SetUp()
    {
        //foreach (WagonProfile w in wagons)
        //{
        //    if (w.unlocked&&w.alive)
        //    {
        //        foreach (Card c in w.Cards)
        //        {
        //            if (c != null)
        //                c.SubscribeEvents();
        //        }
        //    }
        //}
        if (resetOnStart)
            Reset();
        RefreshKeywords();
        ReconciderActiveAbility();
    }

    public bool CardIsEquipped(Card card)
    {
        if (card == BasicCard)
            return false;

        foreach (WagonProfile w in wagons)
        {
            if (w.unlocked && w.alive)
                foreach (Card c in w.Cards)
                {
                    if (c == card)
                        return true;
                }
        }
        return false;
    }
    public void AddCard(Card card, int rarity=0)
    {
        rarity = Mathf.Clamp(rarity, 0, 3);

        foreach (Card c in allCards)
        {
            if (c == card)
            {
                c.RarityOwned[rarity]++;
                RefreshKeywords();
                return;
            }
        }

        Debug.LogError("ERROR - Card " + card.name + " has not been added to allCards in the CaravanInventory and can therefore not be manipulated.");
    }
    public void RemoveCard(Card card, int rarity)
    {
        rarity = Mathf.Clamp(rarity, 0, 3);

        foreach (Card c in allCards)
        {
            if (c == card)
            {
                c.RarityOwned[rarity]--;
                RefreshKeywords();
                return;
            }
        }

        Debug.LogError("ERROR - Card " + card.name + " has not been added to allCards in the CaravanInventory and can therefore not be manipulated.");
    }

    public void ReconciderActiveAbility()
    {
        if(!activeAbility||!HasKeywordEquipped(activeAbility.keyword))
            foreach (ActiveAbility a in allActiveAbilities)
            {
                if (HasKeywordEquipped(a.keyword))
                {
                    activeAbility = a;
                    activeAbility.Reset(Caravan.main);
                }
            }
    }
    public void SetActiveAbility(int index)
    {
        ActiveAbility newActive = allActiveAbilities[index];
        if (HasKeywordEquipped(newActive.keyword))
        {
            activeAbility = newActive;
            activeAbility.Reset(Caravan.main);
        }
    }
    public bool HasKeywordEquipped(Keyword keyword)
    {
        int amount = 0;
        return HasKeywordEquipped(keyword,out amount);
    }
    public bool HasKeywordEquipped(Keyword keyword, out int amount)
    {
        bool equipped = false;
        amount = 0;
        foreach (WagonProfile w in wagons)
        {
            if (w.type != WagonType.Engine && w.unlocked && w.alive && w.Cards.Length > 0 && w.Cards[0])
            {
                foreach (Keyword k in w.Cards[0].keywords)
                {
                    if (k == keyword)
                    {
                        equipped = true;
                        amount++;
                    }
                }
            }
        }
        return equipped;
    }

    public WagonProfile GetInactiveWagon(out int index)
    {
        index = 0;

        for (int i = 0; i < wagons.Length; i++)
        {
            WagonProfile profile = wagons[i];
            if (!profile.unlocked)
            {
                index = i;
                return profile;
            }
        }

        return null;
    }

    public void RefreshKeywords()
    {
        keywords = new Dictionary<Keyword, int>();

        foreach (Keyword keyword in System.Enum.GetValues(typeof(Keyword)))
        {
            keywords.Add(keyword, 0);
        }

        foreach (WagonProfile w in wagons)
        {
            if (w.alive && w.unlocked)
            {
                foreach (Card card in w.Cards)
                {
                    if (card)
                        foreach (Keyword k in card.keywords)
                        {
                            keywords[k]++;
                        }
                }
            }
        }
    }

    public void Reset()
    {
        amber = 15;
        timber = 15;

        foreach (Card c in allCards)
        {
            c.AnyRarity = 0;
        }
        StartCard.BronzeRarity = 1;

        WagonProfile w = wagons[0];
        w.unlocked = true;
        w.alive = true;
        w.healthBuffAmount = 0;
        w.ClearCards();

        w = wagons[1];
        w.unlocked = true;
        w.alive = true;
        w.healthBuffAmount = 0;
        w.ClearCards();
        w.AddCard(StartCard,0);

        for (int i = 2; i < wagons.Length; i++)
        {
            w = wagons[i];
            w.unlocked = false;
            w.alive = false;
            w.healthBuffAmount = 0;
            w.ClearCards();
        }

        activeAbility = allActiveAbilities[0];

        Caravan.main.RefreshActiveWagons();
    }
}

public enum WagonType
{
    Normal, Engine
}