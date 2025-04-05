using Il2Cpp;
using Il2CppMoon;
using System;
using MelonLoader;
using System.Linq;
using UnityEngine;
using UniverseLib;

public class MantisConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        mantisType MantisType;

        Enum.TryParse<mantisType>(GetString(Asset, "MantisType"), out MantisType);

        GameObject mantis = null;

        switch (MantisType)
        {
            case mantisType.Base:
                mantis = GameObject.Instantiate(PrefabCachingManager.GetPrefab("BaseMantis"), Asset.transform);
                break;

            case mantisType.Green:
                mantis = GameObject.Instantiate(PrefabCachingManager.GetPrefab("GreenMantis"), Asset.transform);
                break;

            case mantisType.Electric:
                mantis = GameObject.Instantiate(PrefabCachingManager.GetPrefab("ElectricMantis"), Asset.transform);
                break;
        }


        mantis.transform.localPosition = Vector3.zero;

        MantisPlaceholder PlaceHolder = mantis.GetComponent<MantisPlaceholder>();

        // settings

        if(GetFloat(Asset, "MaxHealth") != -1) PlaceHolder.Settings.MaxHealth = GetFloat(Asset, "MaxHealth");

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
        GameObject.Destroy(PlaceHolder.CurrentEntity);

        PlaceHolder.SpawnOnGround = false;


        // spawn

        PlaceHolder.Invoke("Spawn", 0.1f);
    }

    public enum mantisType
    {
        Base,
        Green,
        Electric
    }
}
