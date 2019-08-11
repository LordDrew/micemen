using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public static string bluePlayerKey = "bluePlayer";
    public static string redPlayerKey = "redPlayer";
    public Dropdown bluePlayer;
    public Dropdown redPlayer;
    void Start()
    {
        bluePlayer.value = PlayerPrefs.GetInt(bluePlayerKey, 0);
        redPlayer.value = PlayerPrefs.GetInt(redPlayerKey, 1);
    }
    
    public void OnBluePlayerChanged(int value)
    {
        PlayerPrefs.SetInt(bluePlayerKey, value);
    }
    public void OnRedPlayerChanged(int value)
    {
        PlayerPrefs.SetInt(redPlayerKey, value);
    }
    public void OnGameStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
