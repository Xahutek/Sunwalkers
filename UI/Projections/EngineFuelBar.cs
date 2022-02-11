using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class EngineFuelBar : Projection
{
    public Wagon wagon;
    public Image bar;
    public Animator anim;

    private void FixedUpdate()
    {
        if (wagon)
        {
            float fill = (float)wagon.caravan.sprintFuel / (float)wagon.caravan.maxSprintFuel;
            bar.fillAmount = fill;
            anim.SetBool("Invulnerable", fill==1);
            anim.SetBool("Invisible", fill>=1);
        }
    }
}
