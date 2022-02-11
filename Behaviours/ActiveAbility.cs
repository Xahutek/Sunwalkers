using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAbility : ScriptableObject
{
    public CaravanInventory inventory;
    public Keyword keyword;
    public LayerMask LMGlobe;
    protected int powerBonus = 0;
    public float Charge
    {
        get
        {
            return Mathf.Clamp01(charge.x / charge.y);
        }
    }
    public bool SufficientCharge
    {
        get
        {
            return charge.x>minChargeCost;
        }
    }
    [SerializeField] protected Vector2 charge;
    public float minChargeCost, minReload;
    public float reloadTimer=0;

    protected float timePressed;

    protected bool wasPressed = false;

    public virtual void FixedRefresh(Caravan c, bool pressed)
    {
        if (pressed && !SufficientCharge)
            pressed = false;

        if (pressed && !wasPressed)
        {
            Activate(c);
        }

        if (pressed)
        {
            Press(c);
        }
        else
        {
            if (wasPressed)
                Release(c);
            NotPress(c);
        }
    }
    public virtual void NotPress(Caravan c)
    {
        wasPressed = false;

        charge.x = Mathf.Clamp(charge.x+Globe.fixedDeltaTime,0,charge.y);
    }

    //When the player Starts Pressing
    public virtual void Activate(Caravan c)
    {
        timePressed = 0;
        inventory.HasKeywordEquipped(keyword, out powerBonus);
    }

    //When the player Presses
    public virtual void Press(Caravan c)
    {
        wasPressed = true;
        timePressed += Globe.fixedDeltaTime;
    }

    //When the player Stops Pressing
    public virtual void Release(Caravan c)
    {
        wasPressed = false;
        timePressed = 0;
    }

    //Executing an attack
    public virtual void Attack(Caravan c, float power)
    {

    }

    public virtual void Reset(Caravan c)
    {
        charge.x = 0;
        reloadTimer = 0;
    }

    public Vector3 MousePos()
    {
        Camera cam = CameraController.mainCam;
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LMGlobe))
        {
            Debug.DrawLine(cam.transform.position, hit.point, Color.cyan, 0.3f);
            return hit.point;
        }
        return Globe.Ground(Caravan.main.position + Caravan.main.forward);
    }
}
