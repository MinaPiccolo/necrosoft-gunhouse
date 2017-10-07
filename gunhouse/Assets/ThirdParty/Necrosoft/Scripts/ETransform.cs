using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class ETransform
{
    #region Position

    public static void SetX(this Transform transform, float x)
    {
        Vector3 position = transform.position;
        position.x = x;
        transform.position = position;
    }

    public static void SetY(this Transform transform, float y)
    {
        Vector3 position = transform.position;
        position.y = y;
        transform.position = position;
    }

    public static void SetZ(this Transform transform, float z)
    {
        Vector3 position = transform.position;
        position.z = z;
        transform.position = position;
    }

    public static void SetXY(this Transform transform, float x, float y)
    {
        Vector3 position = transform.position;
        position = new Vector2(x, y);
        transform.position = position;
    }

    public static void SetXY(this Transform transform, Vector2 position)
    {
        Vector3 currentPosition = transform.position;
        currentPosition = position;
        transform.position = currentPosition;
    }

    public static float GetX(this Transform transform)
    {
        return transform.position.x;
    }

    public static float GetY(this Transform transform)
    {
        return transform.position.y;
    }

    public static float GetZ(this Transform transform)
    {
        return transform.position.z;
    }

    public static Vector2 GetXY(this Transform transform)
    {
        return transform.position;
    }

    #endregion

    #region Scale

    public static void ScaleX(this Transform transform, float scaleX)
    {
        Vector3 scale = transform.localScale;
        scale.x = scaleX;
        transform.localScale = scale;
    }

    public static void ScaleY(this Transform transform, float scaleY)
    {
        Vector3 scale = transform.localScale;
        scale.y = scaleY;
        transform.localScale = scale;
    }

    public static void ScaleXY(this Transform transform, Vector2 scaleXY)
    {
        Vector3 scale = transform.localScale;
        scale.x = scaleXY.x;
        scale.y = scaleXY.y;
        transform.localScale = scale;
    }

    public static void ScaleXYBy(this Transform transform, int x, int y)
    {
        Vector3 scale = transform.localScale;
        scale.x += x;
        scale.y += y;
        transform.localScale = scale;
    }

    #endregion

    #region Translate

    public static void TranslateX(this Transform transform, float x)
    {
        transform.SetX((transform.GetX() + x));
    }

    public static void TranslateY(this Transform transform, float y)
    {
        transform.SetY((transform.GetY() + y));
    }

    public static void TranslateXY(this Transform transform, Vector2 position)
    {
        transform.SetXY(transform.GetXY() + position);
    }

    public static void TranslateXY(this Transform transform, float x, float y)
    {
        transform.SetXY(transform.GetX() + x, transform.GetY() + y);
    }

    #endregion
}
