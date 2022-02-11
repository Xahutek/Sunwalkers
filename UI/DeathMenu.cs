using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DeathMenu : Menu
{
    bool respawnPossible=false;

    public TMP_Text
        TimeSurvived,
        BestTime;
    
    public override void Refresh()
    {
        float timeSurvived = Globe.time;

        TimeSurvived.text = "TIME SURVIVED: "+
            System.TimeSpan.FromSeconds(timeSurvived).Hours + ":" +
            System.TimeSpan.FromSeconds(timeSurvived).Minutes + ":" +
            System.TimeSpan.FromSeconds(timeSurvived).Seconds;

        float currentHighscore = PlayerPrefs.GetFloat("Highscore");
        if (timeSurvived > currentHighscore)
        {
            PlayerPrefs.SetFloat("Highscore", timeSurvived);
            currentHighscore = timeSurvived;
        }

        BestTime.text = "BEST TIME: " +
            System.TimeSpan.FromSeconds(currentHighscore).Hours + ":" +
            System.TimeSpan.FromSeconds(currentHighscore).Minutes + ":" +
            System.TimeSpan.FromSeconds(currentHighscore).Seconds;
    }
    public override void OnOpen()
    {
        base.OnOpen();
    }
    public void Restart()
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
    public void Quit()
    {
        Application.Quit();
    }
}
