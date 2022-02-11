using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class EnemyHealthBar : Projection
{
    public Enemy enemy;
    public Image bar;
    public Animator anim;

    private void FixedUpdate()
    {
        if (enemy)
        {
            bar.fillAmount = (float)enemy.unitHealth / (float)enemy.TrueMaxHealth();
            anim.SetBool("Invulnerable", enemy.isInvulnerable);
        }
    }
}
