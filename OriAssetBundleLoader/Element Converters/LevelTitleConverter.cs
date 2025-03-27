using UnityEngine;

public class LevelTitleConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        LevelInstanceSettings.ShowLevelTitle = true;
        LevelInstanceSettings.LevelTitle = GetString(Asset, "Title");
    }
}

