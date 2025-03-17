using Il2Cpp;
using System.Reflection;
using System;
using UnityEngine;
using MelonLoader;

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

                hookGameObject.SetActive(true);

                hookGameObject.transform.localPosition = Vector3.zero;

                hookGameObject.GetComponent<DisableGameObjectWhenOutOfFrustrum>().OnFrustumEnter();
                break;

            case hookType.Fling:

                hookGameObject = GameObject.Instantiate(PrefabCachingManager.GetPrefab("FlingHook"), Asset.transform.position, Quaternion.identity, Asset.transform);

                hookGameObject.SetActive(true);

                SkinningModifier skinningModifier = hookGameObject.GetComponentInChildren<SkinningModifier>();

                skinningModifier.m_oldMaterial = null;

                

                break;
        }
    }



    public enum hookType
    {
        Sticky,
        Fling
    }
}