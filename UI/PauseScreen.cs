using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreen : Menu
{
    private bool isOnPause = false;

    public override void OnOpen()
    {
        isOnPause = true;
        base.OnOpen();
    }

    public override void OnClose()
    {
        isOnPause = false;
        base.OnClose();
    }

    public void LeaveMenu()
    {
        UIManager.main.CloseMenu(this);
    }

    public void RestartGame()
    {
        StartCoroutine(LoadScene(SceneManager.GetActiveScene().name));
    }

    IEnumerator LoadScene(string name)
    {
        LoadingScreen.main.Open();

        AsyncOperation op = SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
        while (!op.isDone)
        {
            yield return null;
        }
        yield return new WaitForEndOfFrame();
    }

    public void GoToMainMenu()
    {
        LoadingScreen.main.Open();
        SceneManager.LoadScene("MainMenu");
    }
}
