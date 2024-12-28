using Il2Cpp;
using System.Linq;
using UnityEngine;
using UniverseLib;


public class BashConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        GameObject bashGameObject = GameObject.Instantiate(PrefabCachingManager.GetPrefab("BashLantern"), Asset.transform.position, Quaternion.identity, Asset.transform);

        bashGameObject.SetActive(true);

    }
}