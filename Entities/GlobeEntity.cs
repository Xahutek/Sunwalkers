using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GlobeEntityInEditor))]
public class GlobeEntity : MonoBehaviour
{
    [Header("Globe Entity")]
    [SerializeField] bool ReorientateAtRuntime = false;
    [SerializeField] bool GroundAtRuntime = true;

    public Vector3 position
    {
        get
        {
            return transform.position;
        }
        set
        {
            transform.position = value;
        }
    }
    public Quaternion rotation
    {
        get
        {
            return transform.rotation;
        }
        set
        {
            transform.rotation = value;
        }
    }

    protected virtual void Awake()
    {
    }
    protected virtual void Start()
    {
        Globe.Orientate(transform,GroundAtRuntime);
    }

    protected virtual void Update()
    {

    }
    protected virtual void FixedUpdate()
    {
        if(ReorientateAtRuntime)
        Globe.Orientate(transform);
    }
}
