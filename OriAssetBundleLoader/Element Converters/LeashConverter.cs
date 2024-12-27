using Il2Cpp;
using Il2CppMoon;
using System;
using MelonLoader;
using System.Linq;
using UnityEngine;
using UniverseLib;
using static MantisConverter;



public class LeashConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        hookType HookType;

        Enum.TryParse<hookType>(GetString(Asset, "HookType"), out HookType);

        GameObject hookGameObject = null;

        switch (HookType)
        {
            case hookType.Sticky:
                hookGameObject = GameObject.Instantiate(PrefabCachingManager.GetPrefab("Hook"), Asset.transform);
                break;

            case hookType.Fling:
                hookGameObject = GameObject.Instantiate(PrefabCachingManager.GetPrefab("FlingHook"), Asset.transform);
                break;
        }

        HookFlingPlant Hook = hookGameObject.GetComponent<HookFlingPlant>();
    }

    public enum hookType
    {
        Sticky,
        Fling
    }
}