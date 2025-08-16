using Il2Cpp;
using System;
using UnityEngine;

public class DamageDealerConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        GameObject colliderGameObject = Asset;

        Transform parentColliderGameObject = colliderGameObject.transform.parent;
        if(parentColliderGameObject != null)
        {
            Collider parentCollider = parentColliderGameObject.GetComponent<Collider>();
            if (parentCollider != null)
            {
                colliderGameObject = parentColliderGameObject.gameObject;
            }
        }

        DamageType damageType;
        Enum.TryParse<DamageType>(GetString(Asset, "DamageType"), out damageType);

        DamageDealer damageDealer = colliderGameObject.AddComponent<DamageDealer>();
        damageDealer.Damage = GetFloat(Asset, "Damage");

        damageDealer.DamageLayerMask = Il2CppMoon.DamageLayerMask.PlayerAndEnemy;
        damageDealer.DamageType = damageType;

        Collider collider = colliderGameObject.GetComponent<Collider>();
        damageDealer.m_collider = collider;

        if (collider is MeshCollider)
            damageDealer.m_hasMeshCollider = true;

        colliderGameObject.layer = 16;
    }
}
