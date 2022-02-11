using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager main;
    public ConfirmPopUp popUp;
    public Menu PauseMenu;
    public MapCameraController Map;

    public List<Menu> UILayers = new List<Menu>();

    public bool active;

    public void OpenMenu(Menu newInterface)
    {
        if (newInterface == null)
            return;

        newInterface.OnOpen();
        UILayers.Add(newInterface);
        active = true;

        Globe.isCounting = UILayers.Count == 0;
    }
    public void CloseTopMenu()
    {
        if (UILayers.Count == 0)
            return;

        Menu topMenu = UILayers[UILayers.Count - 1];
        CloseMenu(topMenu);
    }
    public void CloseMenu(Menu newInterface)
    {
        newInterface.OnClose();
        UILayers.Remove(newInterface);

        Globe.isCounting = UILayers.Count==0;

        if (UILayers.Count == 0)
            active = false;
    }

    private void Awake()
    {
        main = this;   
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (UILayers.Count > 0 && !UILayers[UILayers.Count - 1].Inescapable)
                CloseTopMenu();
            else if(!Map.active&&!Map.inMotion)
                OpenMenu(PauseMenu);
        }
    }

    public void Confirm(ConfirmPopUp.ConfirmedDelegate newExecute, string pay, string recieve) => popUp.SetUp(newExecute, pay, recieve);
    public void Confirm(ConfirmPopUp.ConfirmedDelegate newExecute, string action) => popUp.SetUp(newExecute, action);
}
