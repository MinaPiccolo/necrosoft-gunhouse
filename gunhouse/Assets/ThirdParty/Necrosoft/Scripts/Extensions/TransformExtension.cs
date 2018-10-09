using UnityEngine;

public static class TransformExtension
{
    #region Position

    static Vector3 vec3 = Vector3.zero;
    static Vector3 vec3Abs = Vector3.zero; 

    public static void SetX(this Transform transform, int x)
    {
        vec3 = transform.position;
        vec3.x = x;
        transform.position = vec3;
    }

    public static void SetX(this Transform transform, float x)
    {
        vec3 = transform.position;
        vec3.x = x;
        transform.position = vec3;
    }

    public static void SetY(this Transform transform, int y)
    {
        vec3 = transform.position;
        vec3.y = y;
        transform.position = vec3;
    }

    public static void SetY(this Transform transform, float y)
    {
        vec3 = transform.position;
        vec3.y = y;
        transform.position = vec3;
    }

    public static void SetZ(this Transform transform, int z)
    {
        vec3 = transform.position;
        vec3.z = z;
        transform.position = vec3;
    }

    public static void SetZ(this Transform transform, float z)
    {
        vec3 = transform.position;
        vec3.z = z;
        transform.position = vec3;
    }

    public static void SetXY(this Transform transform, int x, int y)
    {
        vec3.x = x;
        vec3.y = y;
        vec3.z = transform.position.z;
        transform.position = vec3;
    }

    public static void SetXY(this Transform transform, float x, float y)
    {
        vec3.x = x;
        vec3.y = y;
        vec3.z = transform.position.z;
        transform.position = vec3;
    }

    public static void SetXY(this Transform transform, Vector2 position)
    {
        vec3.x = position.x;
        vec3.y = position.y;
        vec3.z = transform.position.z;
        transform.position = vec3;
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

    public static void ScaleX(this Transform transform, int x)
    {
        vec3 = transform.localScale;
        vec3.x = x;
        transform.localScale = vec3;
    }

    public static void ScaleX(this Transform transform, float x)
    {
        vec3 = transform.localScale;
        vec3.x = x;
        transform.localScale = vec3;
    }

    public static void ScaleY(this Transform transform, int y)
    {
        vec3 = transform.localScale;
        vec3.y = y;
        transform.localScale = vec3;
    }

    public static void ScaleY(this Transform transform, float y)
    {
        vec3 = transform.localScale;
        vec3.y = y;
        transform.localScale = vec3;
    }

    public static void ScaleXY(this Transform transform, float v)
    {
        vec3 = transform.localScale;
        vec3.x = v;
        vec3.y = v;
        transform.localScale = vec3;
    }

    public static void ScaleXY(this Transform transform, int x, int y)
    {
        vec3 = transform.localScale;
        vec3.x = x;
        vec3.y = y;
        transform.localScale = vec3;
    }

    public static void ScaleXY(this Transform transform, float scaleX, float scaleY)
    {
        vec3 = transform.localScale;
        vec3.x = scaleX;
        vec3.y = scaleY;
        transform.localScale = vec3;
    }

    public static void ScaleXY(this Transform transform, Vector2 scaleXY)
    {
        vec3 = transform.localScale;
        vec3.x = scaleXY.x;
        vec3.y = scaleXY.y;
        transform.localScale = vec3;
    }

    #endregion

    #region Translate

    public static void TranslateX(this Transform transform, float x)
    {
        transform.SetX(transform.GetX() + x);
    }

    public static void TranslateY(this Transform transform, float y)
    {
        transform.SetY(transform.GetY() + y);
    }

    public static void TranslateXY(this Transform transform, Vector2 xyPosition)
    {
        transform.SetXY(transform.GetXY() + xyPosition);
    }

    #endregion

    public static void FlipPostive(this Transform transform)
    {
        vec3 = transform.localScale;
        vec3Abs.x = Mathf.Abs(vec3.x);
        vec3Abs.y = Mathf.Abs(vec3.y);
        vec3Abs.z = Mathf.Abs(vec3.z);
        transform.localScale = vec3Abs;
    }
}
