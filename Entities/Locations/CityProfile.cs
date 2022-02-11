using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "City Profile")]
public class CityProfile : ScriptableObject
{
    public CaravanInventory caravan;

    [SerializeField] string Name;
    public new string name
    {
        get
        {
            return Name;
        }
    }
    [TextArea]public string description;

    public bool isHajabaku, isKorLibet;
    public int BasePassagePrice;
    public int PassagePrice
    {
        get
        {
            return Mathf.Max(1,BasePassagePrice + costModifier);
        }
    }
    
    public Sprite Background;
    public AudioClip idleAudio,idleSecondaryAudio;

    public AudioClip IdleAudio
    {
        get
        {
            if (isKorLibet && ReputationLerp <= 0.25f)
                return idleSecondaryAudio;
            return idleAudio;
        }
    }

    public Color
        colMain, colDark;

    [SerializeField] Vector2 sunwalkerReputation, reputationGrowth;
    [System.Serializable]public class ReputationLevel
    {
        public string title, description;
        public int costModifier;
    }
    public ReputationLevel[] reputationLevels;
    public ReputationLevel reputationLevel
    {
        get
        {
            int index = Mathf.RoundToInt(ReputationLerp * 3);
            return reputationLevels[Mathf.Clamp(index,0,reputationLevels.Length-1)];
        }
    }
    public int costModifier
    {
        get
        {
            return reputationLevel.costModifier;
        }
    }

    public Vector2 caravanRepair;
    public bool repairUnlocked=true;

    public float Reputation
    {
        get
        {
            return sunwalkerReputation.x;
        }
        set
        {
            sunwalkerReputation.x = Mathf.Clamp(value, 0, sunwalkerReputation.y);
        }
    }
    public float MaxReputation
    {
        get
        {
            return sunwalkerReputation.y;
        }
    }
    public float NextReputation
    {
        get
        {
            return reputationGrowth.x;
        }
        set
        {
            reputationGrowth.x = value;
        }
    }
    public float ReputationLerp
    {
        get
        {
            return sunwalkerReputation.x / sunwalkerReputation.y;
        }
    }
    public float NextReputationLerp
    {
        get
        {
            return Mathf.Max(0,sunwalkerReputation.x + NextReputation) / sunwalkerReputation.y;
        }
    }



    [Header("Shop Offers")]

    public Card[] CardOffersOptions;
    public Dictionary<Card, Vector2Int> CardOffers;
    public bool[] bought = new bool[3] { false, false, false };

    public int RerollOffersPrice;
    public int RerollCardOffersPrice
    {
        get
        {
            return Mathf.Max(1, RerollOffersPrice +costModifier);
        }
    }

    [Range(0, 2)]
    public float
        TimberToAmberExchangeRate;
    [Range(0, 2)]
    public float
        AmberToTimberExchangeRate;
    public float WagonHealAmount = 30;

    public int WagonHealCost
    {
        get
        {
            return Mathf.RoundToInt(Mathf.Max(1, Caravan.main.timberHealConsumption * WagonHealAmount + costModifier));
        }
    }


    public void RandomizeCardOffer()
    {
        List<Card> options = new List<Card>();
        options.AddRange(CardOffersOptions);

        CardOffers = new Dictionary<Card, Vector2Int>();

        while (CardOffers.Count < 3)
        {
            if (options.Count == 0)
                break;

            Card c = options[Random.Range(0, options.Count)];

            if (CardOffers.ContainsKey(c))
                continue;

            int rarity = CardOffers.Count == 0 ? 1 : 0;
            CardOffers.Add(c, new Vector2Int(rarity, Mathf.Max(1, Random.Range(3, 5) * (rarity == 1 ? 2 : 1) + costModifier)));

            options.Remove(c);
        }
        
        bought = new bool[3] { false, false, false };
    }
    public Vector2 CheckCardStats(int index)
    {
        int i = 0;
        foreach (Card card in CardOffers.Keys)
        {
            if (i == index)
                return CardOffers[card];
            i++;
        }
        return Vector2.zero;
    }
    public Card CheckCardOffer(int index)
    {
        int i = 0;
        foreach (Card card in CardOffers.Keys)
        {
            if (i == index)
                return card;
            i++;
        }
        return null;
    }
    public bool BuyCardOffer(int index)
    {
        Card newCard = null;
        int i = 0;
        foreach (Card card in CardOffers.Keys)
        {
            if (i == index)
            {
                newCard = card;
                break;
            }
            i++;
        }

        if (!newCard)
        {
            Debug.LogWarning("City Shop Card offer index " + index + " out of range!");
            return false;
        }

        Vector2Int details = CardOffers[newCard];

        if (caravan.Amber < details.y)
        {
            return false;
        }

        caravan.Amber -= details.y;
        NextReputation += details.y;

        caravan.AddCard(newCard, details.x);
        bought[i] = true;

        return true;
    }
    public void SellAmber(int amount)
    {
        NextReputation += amount;
    }

    public void Repair(Caravan caravan)
    {
        //if (!caravan)
        //    return;

        //foreach (Wagon w in caravan.Wagons)
        //{
        //    w.Heal(w.TrueMaxHealth() * Mathf.Lerp(caravanRepair.x, caravanRepair.y, ReputationLerp));
        //}
        //repairUnlocked = false;
    }

    public void Reset()
    {
        Reputation = 20;
        ResetCycle(false);
    }
    public void ResetCycle(bool alterReputation=true)
    {
        if (alterReputation)
            Reputation += NextReputation;
        NextReputation = reputationGrowth.y;
        repairUnlocked = true;

        RandomizeCardOffer();

        Debug.Log("Reset "+name);
    }
}
