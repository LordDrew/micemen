using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public static string bluePlayerKey = "bluePlayer";
    public static string redPlayerKey = "redPlayer";
    public static string symmetricFildKey = "symmetricField";
    public static string symmetricTileOffsetKey = "symmetricTileOffset";
    public static string symmetricMicePositionKey = "symmetricMicePosition";

    public Dropdown bluePlayer;
    public Dropdown redPlayer;
    public Toggle fieldSymmetry;
    public Toggle offsetSymmetry;
    public Toggle miceSymmetry;
    void Start()
    {
        bluePlayer.value = PlayerPrefs.GetInt(bluePlayerKey, 0);
        redPlayer.value = PlayerPrefs.GetInt(redPlayerKey, 1);
        fieldSymmetry.isOn = PlayerPrefs.GetInt(symmetricFildKey, 0) == 1;
        offsetSymmetry.isOn = PlayerPrefs.GetInt(symmetricTileOffsetKey, 0) == 1;
        miceSymmetry.isOn = PlayerPrefs.GetInt(symmetricMicePositionKey, 0) == 1;
    }

    void EnableToggles()
    {
        if (fieldSymmetry.isOn)
        {
            offsetSymmetry.interactable = true;
            if (offsetSymmetry.isOn)
            {
                miceSymmetry.interactable = true;
            }
            else
            {
                miceSymmetry.interactable = false;
            }
        }
        else
        {
            offsetSymmetry.interactable = false;
            miceSymmetry.interactable = false;
        }
    }
    
    public void OnBluePlayerChanged(int value)
    {
        PlayerPrefs.SetInt(bluePlayerKey, value);
    }
    public void OnRedPlayerChanged(int value)
    {
        PlayerPrefs.SetInt(redPlayerKey, value);
    }
    public void OnSymmetricFieldChanged(bool value)
    {
        PlayerPrefs.SetInt(symmetricFildKey, value ? 1 : 0);
        EnableToggles();
    }
    public void OnSymmetricTileOffsetChanged(bool value)
    {
        PlayerPrefs.SetInt(symmetricTileOffsetKey, value ? 1 : 0);
        EnableToggles();
    }
    public void OnSymmetricMicePositionChanged(bool value)
    {
        PlayerPrefs.SetInt(symmetricMicePositionKey, value ? 1 : 0);
    }
    public void OnGameStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
