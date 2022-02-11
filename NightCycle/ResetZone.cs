using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetZone : MonoBehaviour
{
    public LayerMask LMTriggerBox;

    private void OnTriggerEnter(Collider other)
    {
        if (LMTriggerBox.Contains(other.gameObject.layer))
        {
            ResettableObject obj = other.GetComponent<ResettableObject>();
            if (obj != null)
                obj.OnReset();
        }
    }
}
