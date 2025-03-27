using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LevelInstanceSettings
{
    public static Vector2 PlayerSpawnPosition = new Vector2();

    public static string LevelTitle = "Example Title";
    public static bool ShowLevelTitle = false;

    public static void ResetSettings()
    {
        PlayerSpawnPosition = new Vector2();
        LevelTitle = "Example Title";
        ShowLevelTitle = false;
    }
}
