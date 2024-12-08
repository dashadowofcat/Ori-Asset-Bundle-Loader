using Il2Cpp;
using Il2CppMoon.Timeline;
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
        GameObject lifePlant = GameObject.Instantiate(PrefabCachingManager.GetPrefab("LifePlant"), Asset.transform);

        lifePlant.transform.localPosition = Vector3.zero;

        MoonTimeline DeathTimeline = lifePlant.GetComponentInChildren<MoonTimeline>();

        DeathTimeline.OnStartEvent += new Action(() => DeathTimeline.OnUpdateEntity(1));

        OrbSpawner orbSpawner = lifePlant.GetComponent<OrbSpawner>();

        lifePlant.GetComponent<Respawner>().Respawn();

        orbSpawner.NumberOfHealthOrbs = this.GetInt(Asset, "NumberOfHealthOrbs");
    }
}


