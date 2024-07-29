using Il2Cpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DamageDealerConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        GameObject ColliderGameobject = Asset.transform.parent.Find("Collision").gameObject;

        DamageDealer damageDealer = ColliderGameobject.AddComponent<DamageDealer>();

        damageDealer.Damage = float.Parse(Asset.transform.GetChild(0).name);

        damageDealer.DamageType = DamageType.Spikes;

        damageDealer.m_collider = ColliderGameobject.GetComponent<Collider>();

        damageDealer.m_hasMeshCollider = true;
    }
}

