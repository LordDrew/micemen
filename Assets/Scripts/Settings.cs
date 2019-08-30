using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public static string bluePlayerKey = "bluePlayer";
    public static string redPlayerKey = "redPlayer";
    public static string bluePlayerBudgetKey = "bluePlayerBudget";
    public static string redPlayerBudgetKey = "redPlayerBudget";
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
        GetPlayer(bluePlayerKey, bluePlayerBudgetKey, bluePlayer, 0);
        GetPlayer(redPlayerKey, redPlayerBudgetKey, redPlayer, 1);

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
    
    private void GetPlayer(string playerKey, string budgetKey, Dropdown dropdown, int defaultValue)
    {
        int value = PlayerPrefs.GetInt(playerKey, defaultValue);
        switch(value)
        {
            case 0:
            case 1:
                dropdown.value = value;
                break;
            case 2:
                int budget = PlayerPrefs.GetInt(budgetKey, 5);
                switch (budget)
                {
                    case 1:
                        dropdown.value = 2;
                        break;
                    case 3:
                        dropdown.value = 3;
                        break;
                    case 5:
                        dropdown.value = 4;
                        break;
                    case 10:
                        dropdown.value = 5;
                        break;
                    case 30:
                        dropdown.value = 6;
                        break;
                }
                break;
        }
    }
    private void UpdatePlayer(string playerKey, string budgetKey, int value)
    {
        switch (value)
        {
            case 0:
            case 1:
                PlayerPrefs.SetInt(playerKey, value);
                break;
            case 2:
                PlayerPrefs.SetInt(playerKey, 2);
                PlayerPrefs.SetInt(budgetKey, 1);
                break;
            case 3:
                PlayerPrefs.SetInt(playerKey, 2);
                PlayerPrefs.SetInt(budgetKey, 3);
                break;
            case 4:
                PlayerPrefs.SetInt(playerKey, 2);
                PlayerPrefs.SetInt(budgetKey, 5);
                break;
            case 5:
                PlayerPrefs.SetInt(playerKey, 2);
                PlayerPrefs.SetInt(budgetKey, 10);
                break;
            case 6:
                PlayerPrefs.SetInt(playerKey, 2);
                PlayerPrefs.SetInt(budgetKey, 30);
                break;
        }
    }
    public void OnBluePlayerChanged(int value)
    {
        UpdatePlayer(bluePlayerKey, bluePlayerBudgetKey, value);
    }
    public void OnRedPlayerChanged(int value)
    {
        UpdatePlayer(redPlayerKey, redPlayerBudgetKey, value);
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
