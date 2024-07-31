using Il2Cpp;
using Il2CppMoon;
using MelonLoader;
using System.Linq;
using UnityEngine;
using UniverseLib;

public class HornBugConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        GameObject hornBug = GameObject.Instantiate(PrefabManager.spring, Asset.transform);

        hornBug.transform.localPosition = Vector3.zero;
    }
}
