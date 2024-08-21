using Il2Cpp;
using MelonLoader;
using System.Linq;
using UnityEngine;
using UniverseLib;

public class SpringConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        GameObject spring = GameObject.Instantiate(PrefabCachingManager.GetPrefab("Spring"), Asset.transform);

        spring.transform.localPosition = Vector3.zero;

        spring.GetComponentInChildren<Spring>().Height = this.GetFloat(Asset, "Force");
    }
}