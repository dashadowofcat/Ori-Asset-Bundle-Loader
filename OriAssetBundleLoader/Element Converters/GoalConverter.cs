using Il2Cpp;
using MelonLoader;
using UnityEngine;

public class GoalConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        BoxCollider2D boxCollider2D = Asset.GetComponent<BoxCollider2D>();

        Rect goalBounds = new Rect();
        goalBounds.Set(Asset.transform.position.x + boxCollider2D.offset.x - boxCollider2D.size.x / 2f,
            Asset.transform.position.y + boxCollider2D.offset.y - boxCollider2D.size.y / 2f,
            boxCollider2D.size.x,
            boxCollider2D.size.y);

        LevelInstanceSettings.GoalRect = goalBounds;

        MelonLogger.Msg("Goal Bounds: " + goalBounds);
    }
}
