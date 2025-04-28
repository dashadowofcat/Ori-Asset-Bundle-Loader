using UnityEngine;

public class CameraSettingsConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        LevelInstanceSettings.CameraPosition = new Vector3(GetFloat(Asset, "X"), GetFloat(Asset, "Y"), GetFloat(Asset, "Z"));
        LevelInstanceSettings.CameraFoV = GetFloat(Asset, "FoV");
    }
}

