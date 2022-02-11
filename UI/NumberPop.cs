using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NumberPop : Projection
{
    public bool isPlaying = false;

    public Animator anim;
    public TMP_Text numberText;

    public void Pop(Vector3 pos,int number, Color col)
    {
        isPlaying = true;
        numberText.color = col;
        Pop(pos, number);
    }
    public void Pop(Vector3 pos,int number)
    {
        isPlaying = true;

        numberText.text = number.ToString();
        transform.position = pos + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
        anim.SetTrigger("POP");
    }
}
