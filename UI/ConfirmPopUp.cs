using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ConfirmPopUp : Menu
{
    public delegate void ConfirmedDelegate();

    public ConfirmedDelegate Execute;

    public TMP_Text ActionDescription;

    public void SetUp(ConfirmedDelegate newExecute, string pay, string recieve)
    {
        SetUp(newExecute,"Pay " + pay + " to recieve " + recieve + ".");
    }
    public void SetUp(ConfirmedDelegate newExecute, string action)
    {
        OnOpen();
        Execute = newExecute;
        ActionDescription.text = action;
    }

    public void Denie()
    {
        OnClose();
    }
    public void Accept()
    {
        if(Execute!=null)
            Execute();
        OnClose();
    }
}
