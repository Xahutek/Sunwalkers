using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static Transform main;
    public static Camera mainCam, mapCam;
    public Transform Locus;
    public Transform HorizontalRotator,PolarRotator;
    public Animator camAnim1,camCombatAnim;

    protected virtual void Start()
    {
        Locus = Caravan.main.Wagons[0].transform;
    }
    protected virtual void FixedUpdate()
    {
        Vector3 
            LocusDirection = Globe.Up(Locus),
            LocusHorizontalLocal= HorizontalRotator.InverseTransformPoint(LocusDirection);

        camCombatAnim.SetBool("Combat", Caravan.inCombat);

        HorizontalRotator.localRotation 
            = Quaternion.FromToRotation(Vector3.up, new Vector3(0, LocusDirection.y, LocusDirection.z).normalized);
        PolarRotator.localRotation
            = Quaternion.FromToRotation(Vector3.up, new Vector3(LocusHorizontalLocal.x, LocusHorizontalLocal.y, 0).normalized);
    }
    float ShakeTimer = 0;
    public void Shake()
    {
        if (ShakeTimer < Globe.time)
        {
            camAnim1.SetTrigger("Shake");
            ShakeTimer = Globe.time + 1;
        }
    }
}
