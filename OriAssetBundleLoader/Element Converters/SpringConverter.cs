﻿using Il2Cpp;
using MelonLoader;
using System.Linq;
using UnityEngine;
using UniverseLib;

public class SpringConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        GameObject spring = GameObject.Instantiate(PrefabManager.spring, Asset.transform);

        spring.transform.localPosition = Vector3.zero;

        spring.GetComponentInChildren<Spring>().Height = this.GetFloat(Asset, "Force");
    }
}