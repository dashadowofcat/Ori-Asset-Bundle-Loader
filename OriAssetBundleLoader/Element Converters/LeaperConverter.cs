using Il2Cpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LeaperConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        GameObject leaper = GameObject.Instantiate(PrefabCachingManager.GetPrefab("YellowLeaper"), Asset.transform); ;


        leaper.transform.localPosition = Vector3.zero;

        TurtlePlaceholder PlaceHolder = leaper.GetComponent<TurtlePlaceholder>();

        // settings

        if (GetFloat(Asset, "MaxHealth") != -1) PlaceHolder.Settings.MaxHealth = GetFloat(Asset, "MaxHealth");

        if (GetFloat(Asset, "MaxSensorRadius") != -1) PlaceHolder.Settings.MaxSensorRadius = GetFloat(Asset, "MaxSensorRadius");
        if (GetFloat(Asset, "LoseSightRadius") != -1) PlaceHolder.Settings.LoseSightRadius = GetFloat(Asset, "LoseSightRadius");

        PlaceHolder.Settings.ShouldSpawnLoot = GetBool(Asset, "ShouldSpawnLoot");

        if (GetInt(Asset, "EnergyOrbsNumber") != -1) PlaceHolder.Settings.NumberOfEnergyOrbs = GetInt(Asset, "EnergyOrbsNumber");
        if (GetInt(Asset, "HealthOrbsNumber") != -1) PlaceHolder.Settings.NumberOfHealthOrbs = GetInt(Asset, "HealthOrbsNumber");

        PlaceHolder.Settings.ShouldSpawnExpOrbs = GetBool(Asset, "SpawnsExpOrbs");

        if (GetInt(Asset, "ExpOrbNumber") != -1) PlaceHolder.Settings.NumberOfExperienceOrbs = GetInt(Asset, "ExpOrbNumber");

        // spawner settings

        PlaceHolder.RespawnOnScreen = GetBool(Asset, "RespawnOnScreen");
        if (GetFloat(Asset, "RespawnTime") != -1) PlaceHolder.RespawnTime = GetFloat(Asset, "RespawnTime");


        // initialize for level

        EntityPlaceholderScalingLink ScalingLink = PlaceHolder.Prefab.GetComponent<EntityPlaceholderScalingLink>();

        ScalingLink.ScalingData = null;

        PlaceHolder.SpawnOn = EntityPlaceholder.SpawnMode.AutoSpawn;

        try
        {
            GameObject.Destroy(PlaceHolder.PooledEntity);
            GameObject.Destroy(PlaceHolder.CurrentEntity);
        }
        catch { }

        PlaceHolder.SpawnOnGround = false;


        // spawn

        PlaceHolder.Invoke("Spawn", 0.01f);
    }
}
