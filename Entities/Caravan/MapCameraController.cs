using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCameraController : CameraController
{
    public LayerMask LMGlobe;
    public Camera normalCam,CloudCam;
    public float LocusSpeed, ZoomScrollSpeed, CloudTransitionTime;

    public Animator camAnim2, cloundAnim;

    [Range(0, 1)] public float currentZoom,actualZoom;

    [HideInInspector]public bool active = false, inMotion=false;
    protected override void Start()
    {
        mainCam = normalCam;
        mapCam = CloudCam;
        main = mainCam.transform;

        actualZoom = 0.85f;
        currentZoom = 0.85f;
        Invoke("Close",0.1f);
    }

    private void Update()
    {
        if (!inMotion && (Input.GetKeyDown(KeyCode.Tab) && !CombatArena.CaravanInCombat && !UIManager.main.active)|| (active&&Input.GetKeyDown(KeyCode.Escape)))
        {
            StartCoroutine(CloundTransition(CloudTransitionTime,!active));
        }

        if (active)
        {
            currentZoom = 
                Mathf.Clamp01(currentZoom+Input.mouseScrollDelta.y * ZoomScrollSpeed * Time.deltaTime);
            if (currentZoom!=actualZoom)
            {
                float zoomAmount = (actualZoom < currentZoom ? 1 : -1) *
                    Mathf.Min(Mathf.Abs(currentZoom - actualZoom),
                    ZoomScrollSpeed * 0.5f * Time.deltaTime);

                actualZoom = Mathf.Clamp01( actualZoom + zoomAmount);
            }
            camAnim2.SetFloat("Zoom", actualZoom);
        }
    }
    public void Open()
    {
        active = true;
        actualZoom = 0.85f;
        currentZoom = 0.85f;

        Locus.position = Caravan.main.position;

        Globe.isCounting = false;

        Locus.gameObject.SetActive(true);
        mapCam.gameObject.SetActive(true);
        normalCam.gameObject.SetActive(false);
    }
    protected override void FixedUpdate()
    {
        if (!active)
            return;

        Vector3 mousePos = MousePos();
        if (mousePos != Vector3.zero)
        {
            Vector3 newPos= 
                Globe.Ground(
                    Locus.position + (mousePos - Locus.position) * LocusSpeed * Time.fixedDeltaTime );
            if (Globe.PoleDistance(newPos)>5)
                Locus.position = newPos;
            Globe.Orientate(Locus);
        }

        base.FixedUpdate();
    }
    public void Close()
    {
        active = false;
        Globe.isCounting = true;
        Locus.gameObject.SetActive(false);
        mapCam.gameObject.SetActive(false);
        normalCam.gameObject.SetActive(true);
    }
    public IEnumerator CloundTransition(float time, bool open)
    {
        inMotion = true;

        camAnim1.SetBool("Clouds", open);
        camAnim2.SetBool("Clouds", open);
        cloundAnim.SetTrigger("Clouds");

        yield return new WaitForSeconds(time * 0.5f);
        if (open) Open();
        else Close();
        yield return new WaitForSeconds(time * 0.5f + 0.25f);
        inMotion = false;
    }
    public Vector3 MousePos()
    {
        RaycastHit hit;
        Ray ray = mapCam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LMGlobe))
        {
            Debug.DrawLine(mapCam.transform.position, hit.point, Color.cyan, 0.3f);
            return hit.point;
        }
        return Vector3.zero;
    }

}
