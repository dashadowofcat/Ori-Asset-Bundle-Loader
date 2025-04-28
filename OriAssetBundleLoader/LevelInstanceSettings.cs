using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LevelInstanceSettings
{
    public static Vector3 CameraPosition;
    public static float CameraFoV;

    public static Vector2 PlayerSpawnPosition;

    public static string LevelTitle;
    public static bool ShowLevelTitle;

    public static void ResetSettings()
    {
        CameraPosition = new Vector3(0f, 0f, 29.2708f);
        CameraFoV = 45;

        PlayerSpawnPosition = new Vector2();
        LevelTitle = "Example Title";
        ShowLevelTitle = false;
    }
}
