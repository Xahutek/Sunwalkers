using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapMarkerHolder : MonoBehaviour
{
    LayerMask LMGlobe;
    public Vector3 position
    {
        get
        {
            return transform.position;
        }
    }

    private void Start()
    {
        LMGlobe = DataBase.Entities.LMGlobe;
    }

    
}
