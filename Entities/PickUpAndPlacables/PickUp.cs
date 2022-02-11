using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PickUp : HittableGlobeEntity
{
    public LayerMask LMTrigger;
    public Image fadeIndicator;
    protected Canvas c;
    protected Camera cam;

    public float fadeTime;
    protected float fadeTimer;

    protected bool UpForGrabs;

    protected override void Awake()
    {
        base.Awake();

        UpForGrabs = true;
        fadeTimer = 0;

    }
    protected override void Start()
    {
        base.Start();

        c = GetComponentInChildren<Canvas>();
        cam = Camera.main.transform.GetChild(0).GetComponent<Camera>();
        if (c)
            c.worldCamera = cam;
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        c.transform.rotation = cam.transform.rotation;

        if (UpForGrabs)
        {
            fadeTimer += Globe.fixedDeltaTime;

            fadeIndicator.fillAmount = 1 - (fadeTimer / fadeTime);

            if (fadeTimer > fadeTime)
            {
                FadeAway();
                StartCoroutine(ExecuteFade(0.4f));
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!LMTrigger.Contains(other.gameObject.layer))
            return;

        UpForGrabs = false;
        StartCoroutine(ExecuteFade(0.4f));
        PickMeUp(other.gameObject);
    }

    public virtual void PickMeUp(GameObject pickingObject)
    {
        
    }
    public virtual void FadeAway()
    {

    }

    IEnumerator ExecuteFade(float time)
    {
        UpForGrabs = false;
        AnimationCurve ACSize = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.5f, 1.33f), new Keyframe(1, 0));
        float timer = 0, i;

        while (timer < time)
        {
            timer += Globe.deltaTime;
            i = timer / time;
            transform.localScale = Vector3.one * ACSize.Evaluate(i);

            yield return null;
        }
        Destroy(gameObject);
    }

    public override void Die(HittableGlobeEntity attacker, bool drop = true)
    {
        base.Die(attacker, drop);
        PickMeUp(attacker.gameObject);
    }
}
