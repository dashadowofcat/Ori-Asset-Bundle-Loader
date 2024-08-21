using Il2Cpp;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RotatingSpikeHazardConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        MelonLogger.Warning("non-completed convertion (RotatingSpikeHazard), you can either ignore it or re-visit the development");

        return;

        GameObject rotatingHazard = GameObject.Instantiate(PrefabCachingManager.GetPrefab("LifePlant"), Asset.transform);

        rotatingHazard.transform.localPosition = Vector3.zero;

        //rotatingHazard.GetComponentInChildren<AutoRotate>().Speed = this.GetFloat(Asset, "Speed");
    }
}

