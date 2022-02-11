using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class MainMenu : MonoBehaviour
{
    public string SceneName;
    public GameObject Credits;
    public void OpenGameScene()
    {
        OpenGameScene(SceneName);
    }
    public void OpenGameScene(string scene)
    {
        StartCoroutine(LoadScene(scene));
    }

    IEnumerator LoadScene(string name)
    {
        LoadingScreen.main.Open();

        AsyncOperation op = SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
        while(!op.isDone)
        {
            yield return null;
        }
        yield return new WaitForEndOfFrame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenCredits()
    {
        Credits.SetActive(true);
    }
}