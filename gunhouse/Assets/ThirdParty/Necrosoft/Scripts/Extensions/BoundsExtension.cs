using UnityEngine;

public static class BoundsExtension
{
    public static bool ContainsBounds(this Bounds bounds, Bounds target)
    {
        return bounds.Contains(target.min) && bounds.Contains(target.max);
    }
}
