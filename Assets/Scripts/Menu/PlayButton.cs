using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    public ToggleGroup ToggleGroupTeam0;
    public ToggleGroup ToggleGroupTeam1;

    private const string nextSceneName = "SampleScene";
    private Resolution resolution;

    public void Start()
    {
        resolution = Screen.currentResolution;
        Debug.Log("Resolución de pantalla: " + resolution.width + "x" + resolution.height);
    }
    public void OnClick()
    {
        ToggleInfo infoTeam0 = ToggleGroupTeam0.ActiveToggles().First<Toggle>().gameObject.GetComponent<ToggleInfo>();
        ToggleInfo infoTeam1 = ToggleGroupTeam1.ActiveToggles().First<Toggle>().gameObject.GetComponent<ToggleInfo>();

        Debug.Log("Team 0 toggle: " + infoTeam0.BotName);
        Debug.Log("Team 1 toggle: " + infoTeam1.BotName);

        PlayerPrefs.SetString("bot0", infoTeam0.BotName);
        PlayerPrefs.SetString("bot1", infoTeam1.BotName);

        PlayerPrefs.SetInt("screenWidth", resolution.width);
        PlayerPrefs.SetInt("screenHeight", resolution.height);

        LoadScene();
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
