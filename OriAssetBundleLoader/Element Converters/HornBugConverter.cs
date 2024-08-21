using Il2Cpp;
using Il2CppMoon;
using MelonLoader;
using System;
using System.Linq;
using UnityEngine;
using UniverseLib;

public class HornBugConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        GameObject hornBug = GameObject.Instantiate(PrefabCachingManager.hornBug, Asset.transform);

        hornBug.transform.localPosition = Vector3.zero;

        HornBugPlaceholder PlaceHolder = hornBug.GetComponent<HornBugPlaceholder>();

        // settings


        PlaceHolder.VariationType = GetString(Asset, "HornBugType") == "Shelled" ? BugHornEntity.BugHornType.Shelled : BugHornEntity.BugHornType.Naked;

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

        GameObject.Destroy(PlaceHolder.PooledEntity);

        PlaceHolder.SpawnOnGround = false;

        // spawn

        PlaceHolder.Invoke("Spawn", 0f);
    }

    public enum hornBugType
    {
        Shelled,
        NonShelled
    }
}
