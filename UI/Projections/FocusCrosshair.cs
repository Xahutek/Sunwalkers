using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusCrosshair : MonoBehaviour
{
    public LayerMask LMGlobe, LMEnemy;
    public CanvasGroup crosshair;
    protected Animator anim;

    public Camera cam;
    Caravan c;
    Enemy e;

    public float sensitivity = 1;
    bool shoot;

    private void Start()
    {
        c = Caravan.main;
        anim = GetComponent<Animator>();
        crosshair.alpha = 0;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
            shoot = true;

        crosshair.alpha = e ? 1 : 0;

        if (e)
        {
            crosshair.transform.position = e.body.position;
            if (!e.isAlive)
                e = null;
        }
    }
    private void FixedUpdate()
    {
        if (shoot)
        {
            shoot = false;
            LeftClick();
        }
    }

    public void LeftClick()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        c.focusedEnemy = null;
        e = null;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LMGlobe))
        {
            Debug.DrawLine(cam.transform.position, hit.point, Color.cyan,0.3f);
            Collider[] enemiesAround = Physics.OverlapSphere(hit.point, sensitivity, LMEnemy);
            if (enemiesAround.Length > 0)
            {
                e = enemiesAround[0].GetComponent<Enemy>();
                anim.SetTrigger("Pop");
            }
        }
        c.focusedEnemy = e;
    }
}
