using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LevelInstanceSettings
{
    public static Vector2 PlayerSpawnPosition;

    public static string LevelTitle;
    public static bool ShowLevelTitle;

    public static void ResetSettings()
    {
        PlayerSpawnPosition = new Vector2();
        LevelTitle = "Example Title";
        ShowLevelTitle = false;
    }
}
