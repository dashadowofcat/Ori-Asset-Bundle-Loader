using Il2Cpp;
using System;
using UnityEngine;



public class LeashConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        HookType HookType;

        Enum.TryParse<HookType>(GetString(Asset, "HookType"), out HookType);

        GameObject hookGameObject = null;

        switch (HookType)
        {
            case HookType.Sticky:
                hookGameObject = GameObject.Instantiate(PrefabCachingManager.GetPrefab("Hook"), Asset.transform);
                break;

            case HookType.Fling:
                hookGameObject = GameObject.Instantiate(PrefabCachingManager.GetPrefab("FlingHook"), Asset.transform);
                break;
        }

        HookFlingPlant Hook = hookGameObject.GetComponent<HookFlingPlant>();
    }

    public enum HookType
    {
        Sticky,
        Fling
    }
}