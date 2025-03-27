using MelonLoader;
using OriAssetBundleLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SpawnPositionConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        LevelInstanceSettings.PlayerSpawnPosition = Asset.transform.position;
    }
}

