using Il2Cpp;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class WaterZoneConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        BoxCollider2D boxCollider2D = Asset.GetComponent<BoxCollider2D>();

        Rect waterBounds = new Rect();
        waterBounds.Set(Asset.transform.position.x + boxCollider2D.offset.x - boxCollider2D.size.x / 2f,
            Asset.transform.position.y + boxCollider2D.offset.y - boxCollider2D.size.y / 2f,
            boxCollider2D.size.x,
            boxCollider2D.size.y);

        Asset.transform.localPosition = new Vector2(Asset.transform.localPosition.x, Asset.transform.localPosition.y) + boxCollider2D.offset + waterBounds.size / 2f;
        Asset.transform.localScale = waterBounds.size;

        WaterZone waterZoneComp = Asset.AddComponent<WaterZone>();
        waterZoneComp.Bounds = waterBounds;
        waterZoneComp.DamageCondition = WaterZone.DamageApplyType.NeverApplyDamage;
    }
}
