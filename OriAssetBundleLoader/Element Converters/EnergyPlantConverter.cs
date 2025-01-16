using Il2Cpp;
using MelonLoader;
using System.Linq;
using UnityEngine;
using UniverseLib;

public class EnergyPlantConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        GameObject Plant = GameObject.Instantiate(PrefabCachingManager.GetPrefab("EnergyPlant"), Asset.transform);

        Plant.transform.localPosition = Vector3.zero;

        Plant.GetComponent<DisableGameObjectWhenOutOfFrustrum>().OnFrustumEnter();

        Plant.transform.eulerAngles = new Vector3(0, 0, 357);

        GameObject PlantVisual = Plant.transform.Find("mediumEnergyPlantVisuals/plant").gameObject;

        PlantVisual.transform.Find("energyOrbTargetA").GetComponent<OrbSpawner>().NumberOfEnergyOrbs = GetInt(Asset, "NumberOfEnergyOrbs");


        PlantVisual.transform.Find("energyOrbTargetA").GetComponent<OrbSpawner>().ShouldSpawnLoot = false;
        PlantVisual.transform.Find("energyOrbTargetB").GetComponent<OrbSpawner>().ShouldSpawnLoot = false;
        PlantVisual.transform.Find("energyOrbTargetC").GetComponent<OrbSpawner>().ShouldSpawnLoot = false;
    }
}