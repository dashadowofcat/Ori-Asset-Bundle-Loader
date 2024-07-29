using Il2Cpp;
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
        GameObject lifePlant = GameObject.Instantiate(PrefabManager.lifePlant, Asset.transform);

        lifePlant.transform.localPosition = Vector3.zero;

        OrbSpawner orbSpawner = lifePlant.GetComponent<OrbSpawner>();

        orbSpawner.IdealOrbCount = this.GetInt(Asset, "IdealOrbs");

        orbSpawner.NumberOfHealthOrbs = this.GetInt(Asset, "NumberOfHealthOrbs");
    }
}


