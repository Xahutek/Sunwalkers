using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ArenaFix : MonoBehaviour
{
    void Start()
    {
        transform.localScale = transform.parent.localScale;
    }
}
