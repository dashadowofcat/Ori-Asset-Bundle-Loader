using Il2Cpp;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Il2Cpp.XboxOneRichPresence;

public class CheckpointConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        BoxCollider2D boxCollider2D = Asset.GetComponent<BoxCollider2D>();

        Rect checkpointBounds = new Rect();
        checkpointBounds.Set(Asset.transform.position.x + boxCollider2D.offset.x - boxCollider2D.size.x / 2f,
            Asset.transform.position.y + boxCollider2D.offset.y - boxCollider2D.size.y / 2f,
            boxCollider2D.size.x,
            boxCollider2D.size.y);

        InvisibleCheckpoint invisibleCheckpoint = Asset.AddComponent<InvisibleCheckpoint>();
        invisibleCheckpoint.m_bounds = checkpointBounds;
    }
}
