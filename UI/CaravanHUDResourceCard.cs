using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
public class CaravanHUDResourceCard : MonoBehaviour
{
    public TMP_Text amountText;
    public Image brokenFillBar;
    public void Refresh(float amount)
    {
        int flatAmount = Mathf.FloorToInt(amount);

        amountText.text =flatAmount.ToString();
        brokenFillBar.fillAmount = (amount - (float)flatAmount);
    }
}
