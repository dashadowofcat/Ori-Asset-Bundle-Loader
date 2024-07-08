using Il2Cpp;
using System.Linq;
using UnityEngine;
using UniverseLib;


class LeashConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        HookFlingPlant Hook = Asset.AddComponent<HookFlingPlant>();

        Rigidbody rb = Asset.AddComponent<Rigidbody>();

        Hook.HookTarget = Asset.transform;

        Hook.m_rigidbody = rb;

        Hook.IsSticky = this.GetBool(Asset, "Sticky");

        rb.constraints = RigidbodyConstraints.FreezeAll;
    }
}