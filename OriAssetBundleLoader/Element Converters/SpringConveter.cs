using Il2Cpp;
using System.Linq;
using UnityEngine;
using UniverseLib;


class SpringConveter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        Spring spring = Asset.AddComponent<Spring>();

        spring.Height = float.Parse(Asset.transform.Find("Force").GetChild(0).name);

        Asset.transform.forward = new Vector3(0, 0, 1);
    }
}