using Il2Cpp;
using MelonLoader;
using System.Linq;
using UnityEngine;
using UniverseLib;

public class EnergyPlantMediumConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        GameObject energyPlantMedium = GameObject.Instantiate(PrefabCachingManager.GetPrefab("EnergyPlantMedium"), Asset.transform);

        energyPlantMedium.transform.localPosition = Vector3.zero;
    }
}