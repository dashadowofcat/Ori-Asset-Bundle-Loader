using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OriAssetBundleLoader
{
    public static class Constants
    {
        public static Color EnvironmentColor = new Color(1.8f, 1.6f, 1.4f);

        public static Material EnvironmentMaterial => GameObject.Find("seinCharacter/ori3D/mirrorHolder/rigHolder/oriRig/Model_GRP/body_MDL").GetComponent<SkinnedMeshRenderer>().material;

        public static Vector3 LevelSpawnPosition = new Vector3(-333, -2313, 0);

        public static Vector2 PlayerSpawnPosition = new Vector2();

        public static float LevelBoundsEndlargeAmount = 5000;

        public static Vector3 CachedPrefabsLocation = new Vector3(0, 999, 0);
    }
}
