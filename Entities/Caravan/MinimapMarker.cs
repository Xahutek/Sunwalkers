using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapMarker : MonoBehaviour
{
    LayerMask LMGlobe;
    public MapCameraController map;
    public MinimapMarkerHolder objective;
    public Transform marker;

    bool visible = false;

    private void Start()
    {
        LMGlobe = DataBase.Entities.LMGlobe;
    }

    private void FixedUpdate()
    {
        if (objective && map.active && !map.inMotion)
        {
            visible = isVisible();
            marker.gameObject.SetActive(visible);
        }
        else if (marker.gameObject.activeSelf)
        {
            visible = false;            
            marker.gameObject.SetActive(false);
        }
    }
    private void Update()
    {
        if (visible)
        {
            //Vector2 p;
            //RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, map.CloudCam.WorldToScreenPoint(objective.position), map.CloudCam, out p);
            //transform.position = (Vector2)parent.position + p;

            //Vector2 ViewportPosition = cam.WorldToViewportPoint(objective.transform.position);
            //Vector3 WorldObject_ScreenPosition = cam.WorldToScreenPoint(objective.transform.position);
            //    new Vector2(
            //((ViewportPosition.x * parent.sizeDelta.x) - (parent.sizeDelta.x * 0.25f)),
            //((ViewportPosition.y * parent.sizeDelta.y) - (parent.sizeDelta.y * 0.5f)));

            //now you can set the position of the ui element

            Camera cam = map.CloudCam;
            marker.transform.rotation = cam.transform.rotation;
            marker.position= objective.transform.position;
        }
    }

    public bool isVisible()
    {
        RaycastHit hit;
        Camera cam = map.CloudCam;

        if (Physics.Raycast(cam.transform.position,objective.position-cam.transform.position, out hit, Vector3.Distance(cam.transform.position, objective.position) - 1, LMGlobe))
        {
            Debug.DrawLine(cam.transform.position, hit.point, Color.red);
            return false;
        }
        Debug.DrawLine(cam.transform.position, objective.position, Color.yellow);
        return true;
    }
}
