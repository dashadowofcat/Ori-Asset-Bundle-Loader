using Il2Cpp;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LifePlantConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        MelonLogger.Warning("non-completed convertion (LifePlant), you can either ignore it or re-visit the development");

        GameObject lifePlant = GameObject.Instantiate(PrefabCachingManager.GetPrefab("LifePlant"), Asset.transform);

        lifePlant.transform.localPosition = Vector3.zero;

        OrbSpawner orbSpawner = lifePlant.GetComponent<OrbSpawner>();

        orbSpawner.IdealOrbCount = this.GetInt(Asset, "IdealOrbs");

        orbSpawner.NumberOfHealthOrbs = this.GetInt(Asset, "NumberOfHealthOrbs");
    }
}


