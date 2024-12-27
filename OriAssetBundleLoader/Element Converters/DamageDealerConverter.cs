using Il2Cpp;
using System;
using UnityEngine;

public class DamageDealerConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        DamageDealer damageDealer = Asset.AddComponent<DamageDealer>();

        damageDealer.Damage = GetFloat(Asset, "Damage");

        DamageType damageType;

        Enum.TryParse<DamageType>(GetString(Asset, "DamageType"), out damageType);

        damageDealer.DamageLayerMask = Il2CppMoon.DamageLayerMask.PlayerAndEnemy;

        damageDealer.DamageType = damageType;

        damageDealer.m_collider = Asset.GetComponent<Collider>();

        Asset.layer = 16;
    }
}
