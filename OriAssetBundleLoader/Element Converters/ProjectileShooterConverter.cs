using Il2Cpp;
using Il2CppSwing;
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
        GameObject projectileShooter = GameObject.Instantiate(PrefabManager.spring, Asset.transform);

        projectileShooter.transform.localPosition = Vector3.zero;

        projectileShooter.GetComponentInChildren<ProjectileSpawner>().Gravity = this.GetFloat(Asset, "Gravity");

        projectileShooter.GetComponentInChildren<ProjectileSpawner>().Speed = this.GetFloat(Asset, "Speed");

        //projectileShooter.GetComponentInChildren<ProjectileSpawner>().Direction = this.get(Asset, "Direction");

    }
}

