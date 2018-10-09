using UnityEngine;

public static class SpriteRendererExtension
{
    public static SpriteAlignment GetPivot(this SpriteRenderer spriteRenderer)
    {
        Vector3 center = spriteRenderer.sprite.bounds.center;

        if ((center.x == 0) && (center.y == 0))
        {
            return (SpriteAlignment.Center);
        }
        else if ((center.x > 0f) && (center.y < 0f))
        {
            return (SpriteAlignment.TopLeft);
        }
        else if ((center.x == 0) && (center.y < 0))
        {
            return (SpriteAlignment.TopCenter);
        }
        else if ((center.x) < 0 && (center.y < 0))
        {
            return (SpriteAlignment.TopRight);
        }
        else if ((center.x > 0) && (center.y == 0))
        {
            return (SpriteAlignment.LeftCenter);
        }
        else if ((center.x < 0) && (center.y == 0))
        {
            return (SpriteAlignment.RightCenter);
        }
        else if ((center.x > 0) && (center.y > 0))
        {
            return (SpriteAlignment.BottomLeft);
        }
        else if ((center.x == 0) && (center.y > 0))
        {
            return (SpriteAlignment.BottomCenter);
        }
        else if ((center.x < 0) && (center.y > 0))
        {
            return (SpriteAlignment.BottomRight);
        }

        return SpriteAlignment.Custom;
    }
}
