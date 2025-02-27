﻿using UnityEngine;

public static class Player_Shortcuts
{
    public const string shortcutsKey = "SHORTCUTS"; // 0 = zqsd shortcuts, 1 = wasd shortcuts, 2 = directional arrows shortcuts.

    public static void SetShortCuts(int index)
    {
        if (index == 0)
        {
            PlayerPrefs.SetInt(shortcutsKey, 0);
        }
        else if (index == 1)
        {
            PlayerPrefs.SetInt(shortcutsKey, 1);
        }
        else
        {
            PlayerPrefs.SetInt(shortcutsKey, 2);
        }
    }

    public static int GetShortCuts()
    {
        if (PlayerPrefs.HasKey(shortcutsKey))
        {
            return PlayerPrefs.GetInt(shortcutsKey);
        }

        // Debug.Log("No key set in PlayerPrefs for shortcuts. Default are zqsd combinaison.");

        SetShortCuts(0);

        return 0;      
    }
}

