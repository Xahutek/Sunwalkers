using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public KeywordDataBase keywordData;

    public bool updateRarityGems=true;

    public Card card, baseCard;
    public Image
        illustrationImage;
    public TMP_Text
        nameText,
        effectText,
        attackText,
        rangeText,
        reloadText;
    public GameObject
        secondaryClassDisplay,
        effectBackground,
        markedGlow;
    public Image RarityGem;
    public Sprite[] RarityGemSprites;
    public Image[] classIconImages;

    public bool Marked
    {
        set
        {
            markedGlow.SetActive(value);
        }
    }

    public void Refresh(Card card)
    {
        this.card = card;
        if (card == null)
        {
            Refresh(baseCard);
            return;
        }
        illustrationImage.sprite = card.illustration;
        nameText.text = card.name;
        effectText.text = card.effect;

        effectBackground.SetActive(effectText.text != "");

        attackText.text = card.AttackDamage.ToString();
        rangeText.text = card.AttackRange.ToString();
        reloadText.text = card.AttackReload.ToString();

        if (updateRarityGems)
            RarityGem.sprite = RarityGemSprites[card.Level-1];

        for (int i = 0; i < Mathf.Min(card.keywords.Count,2); i++)
        {
            classIconImages[i].sprite = keywordData.GetIcon(card.keywords[i]);
            secondaryClassDisplay.SetActive(i == 1);
        }
    }

    
}
