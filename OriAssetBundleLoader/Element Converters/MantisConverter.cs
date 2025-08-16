using Il2Cpp;
using System;
using UnityEngine;

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

        float maxHealth = GetFloat(Asset, "MaxHealth");
        if(maxHealth != -1f) PlaceHolder.Settings.MaxHealth = maxHealth;

        float maxSensorRadius = GetFloat(Asset, "MaxSensorRadius");
        if (maxSensorRadius != -1f) PlaceHolder.Settings.MaxSensorRadius = maxSensorRadius;

        float loseSightRadius = GetFloat(Asset, "LoseSightRadius");
        if (loseSightRadius != -1f) PlaceHolder.Settings.LoseSightRadius = loseSightRadius;

        PlaceHolder.Settings.ShouldSpawnLoot = GetBool(Asset, "ShouldSpawnLoot");

        int energyOrbsNumber = GetInt(Asset, "EnergyOrbsNumber");
        if (energyOrbsNumber != -1) PlaceHolder.Settings.NumberOfEnergyOrbs = energyOrbsNumber;

        int healthOrbsNumber = GetInt(Asset, "HealthOrbsNumber");
        if (healthOrbsNumber != -1) PlaceHolder.Settings.NumberOfHealthOrbs = healthOrbsNumber;

        PlaceHolder.Settings.ShouldSpawnExpOrbs = GetBool(Asset, "SpawnsExpOrbs");

        int expOrbNumber = GetInt(Asset, "ExpOrbNumber");
        if (expOrbNumber != -1) PlaceHolder.Settings.NumberOfExperienceOrbs = expOrbNumber;

        // spawner settings

        float minDistanceFromPlayer = GetInt(Asset, "MinDistanceFromPlayer");
        if (minDistanceFromPlayer != -1f) PlaceHolder.MinDistanceFromPlayer = minDistanceFromPlayer;

        PlaceHolder.RespawnOnScreen = GetBool(Asset, "RespawnOnScreen");

        float respawnTime = GetFloat(Asset, "RespawnTime");
        if (respawnTime != -1f)
        {
            PlaceHolder.RespawnOnTimeout = true;
            PlaceHolder.RespawnTime = respawnTime;
        }
        else
        {
            PlaceHolder.RespawnOnTimeout = false;
        }

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

        LevelManager.AddEnemyPlaceholder(PlaceHolder);
    }

    public enum mantisType
    {
        Base,
        Green,
        Electric
    }
}
