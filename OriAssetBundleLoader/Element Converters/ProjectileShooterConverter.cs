using Il2Cpp;
using Il2CppSwing;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ProjectileShooterConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        MelonLogger.Warning("non-completed convertion (ProjectileShooter), you can either ignore it or re-visit the development");

        return;

        GameObject projectileShooter = GameObject.Instantiate(PrefabCachingManager.GetPrefab("LifePlant"), Asset.transform);

        projectileShooter.transform.localPosition = Vector3.zero;

        projectileShooter.GetComponentInChildren<ProjectileSpawner>().Gravity = this.GetFloat(Asset, "Gravity");

        projectileShooter.GetComponentInChildren<ProjectileSpawner>().Speed = this.GetFloat(Asset, "Speed");

        //projectileShooter.GetComponentInChildren<ProjectileSpawner>().Direction = this.get(Asset, "Direction");

    }
}

