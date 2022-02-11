using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GlobeEntityInEditor : MonoBehaviour
{
    private void Update()
    {
        Globe.Orientate(transform);
    }
}
