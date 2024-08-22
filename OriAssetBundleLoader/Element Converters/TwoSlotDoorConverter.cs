using Il2Cpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OriAssetBundleLoader.Element_Converters
{
    public class TwoSlotDoorConverter : ElementConverter
    {
        public override void ConvertElement(GameObject Asset)
        {
            GameObject twoSlotDoor = GameObject.Instantiate(PrefabCachingManager.GetPrefab("TwoSlotDoor"), Asset.transform);

            twoSlotDoor.transform.localPosition = Vector3.zero;
        }
    }
}
