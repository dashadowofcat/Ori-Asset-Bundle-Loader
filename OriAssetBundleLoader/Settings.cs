using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OriAssetBundleLoader
{
    public static class Settings
    {
        public static Color EnvironmentColor = new Color(1.8f, 1.6f, 1.4f);

        public static Material EnvironmentMaterial => GameObject.Find("seinCharacter/ori3D/mirrorHolder/rigHolder/oriRig/Model_GRP/body_MDL").GetComponent<SkinnedMeshRenderer>().material;

        public static Transform LevelSpawnPosition => GameObject.Find("seinCharacter").transform;
        
    }
}
