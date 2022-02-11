using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class Menu: MonoBehaviour
{
    public bool automaticDeactivation = true;
    public bool Inescapable;
    protected CanvasGroup canvasGroup;
    public float fadeDuration=0.2f;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (automaticDeactivation)
        {
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0;
            gameObject.SetActive(false);
        }
    }

    public virtual void TriggerOpen()
    {
        UIManager.main.OpenMenu(this);
    }
    public virtual void TriggerOpen(PointOfInterest point)
    {
        UIManager.main.OpenMenu(this);
    }

    public virtual void OnOpen()
    {
        Refresh();
        gameObject.SetActive(true);
        UIManager.main.StartCoroutine(ExecuteFade(fadeDuration, true));
    }
    public virtual void OnClose()
    {
        canvasGroup.interactable = false;
        UIManager.main.StartCoroutine(ExecuteFade(fadeDuration, false));
    }

    public virtual void Refresh()
    {

    }

    public IEnumerator ExecuteFade(float time, bool fadeIn)
    {
        gameObject.SetActive(true);

        if (!canvasGroup)
        {
            gameObject.SetActive(fadeIn);
            yield break;
        } 

        float timer = 0, i;
        while (timer<=time)
        {
            timer += Time.deltaTime;
            i = timer / time;

            canvasGroup.alpha = fadeIn ? i : 1 - i;

            yield return null;
        }
        canvasGroup.alpha = fadeIn ? 1 : 0;

        canvasGroup.interactable = fadeIn;

        gameObject.SetActive(fadeIn);

        if (fadeIn)
            StartCoroutine(AfterFade());
    }
    public virtual IEnumerator AfterFade()
    {
        yield return null;
    }
}
