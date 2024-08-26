using Il2Cpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;



    public class KeystoneConverter : ElementConverter
    {
        public override void ConvertElement(GameObject Asset)
        {
            GameObject keystone = GameObject.Instantiate(PrefabCachingManager.GetPrefab("Keystone"), Asset.transform);

            keystone.transform.localPosition = Vector3.zero;
        }
    }

