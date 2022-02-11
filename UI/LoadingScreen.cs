using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen main;

    public CanvasGroup can;

    public TMP_Text Tip;

    [TextArea] public string[] Tips;

    private void Awake()
    {
        main = this;
        can.alpha = 0;
    }
    public void Open()
    {
        can.alpha = 1;
        Tip.text= Tips[Random.Range(0, Tips.Length)];
    }
}
