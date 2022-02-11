using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberPopPool : MonoBehaviour
{
    public static NumberPopPool main;

    public List<NumberPop> NumberPool= new List<NumberPop>();
    public NumberPop PopPrefab;

    private void Awake()
    {
        main = this;
    }

    public NumberPop NumberPop()
    {
        foreach (NumberPop num in NumberPool)
        {
            if (!num.isPlaying)
                return num;
        }

        NumberPop newPop = Instantiate(PopPrefab, transform);
        NumberPool.Add(newPop);
        return newPop;
    }
}
