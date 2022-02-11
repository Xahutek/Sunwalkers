using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projection : MonoBehaviour
{
    Canvas c;
    Camera cam;
    public virtual void Start()
    {
        cam = Camera.main;
        if (cam==null)
            cam = CameraController.main.GetComponent<Camera>();

        c= GetComponent<Canvas>();
        if (c)
        c.worldCamera=cam;
    }
    public virtual void Update()
    {
        transform.rotation = cam.transform.rotation;
    }
}
