using Il2Cpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TerrainDamageDealerConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        GameObject ColliderGameobject = Asset.transform.parent.Find("Collision").gameObject;

        DamageDealer damageDealer = ColliderGameobject.AddComponent<DamageDealer>();

        damageDealer.Damage = GetFloat(Asset, "DamageAmount");

        DamageType damageType;

        Enum.TryParse<DamageType>(GetString(Asset, "DamageType"), out damageType);

        damageDealer.DamageLayerMask = Il2CppMoon.DamageLayerMask.PlayerAndEnemy;

        damageDealer.DamageType = damageType;

        damageDealer.m_collider = ColliderGameobject.GetComponent<Collider>();

        damageDealer.m_hasMeshCollider = true;
    }
}

