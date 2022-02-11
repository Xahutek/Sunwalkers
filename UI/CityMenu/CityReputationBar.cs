using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class CityReputationBar : CityMenuModule
{
    public TMP_Text status;
    public Image ReputationBar, ReputationBarFadeBG, ReputationBarFadeFG,
        ReputationBarBackground;

    public override void Refresh()
    {
        ReputationBarBackground.color = city.colDark;
        ReputationBar.color = city.colMain;
        ReputationBarFadeBG.color = city.colMain;
        ReputationBarFadeFG.color = city.colMain;

        float currentRep = city.ReputationLerp, nextRep = city.NextReputationLerp, difference = Mathf.Abs(currentRep - nextRep);
        bool isSinking = nextRep < currentRep;

        ReputationBar.fillAmount = currentRep - (isSinking ? difference : 0);
        ReputationBarFadeBG.fillAmount = isSinking ? 0 : nextRep;
        ReputationBarFadeFG.fillAmount = isSinking ? currentRep : 0;
        status.text = city.reputationLevel.title;
    }
}
