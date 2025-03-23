using UnityEngine;

public class LevelTitleConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        LevelSettings.LevelTitle = GetString(Asset, "Title");
    }
}

