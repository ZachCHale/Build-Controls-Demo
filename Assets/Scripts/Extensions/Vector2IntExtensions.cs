using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2IntExtensions
{
    public static Vector2Int RotateRadians(this Vector2Int vector, float radians)
    {
        return new Vector2Int(Mathf.RoundToInt(vector.x * Mathf.Cos(radians) + vector.y * Mathf.Sin(radians)),
            Mathf.RoundToInt(-vector.x * Mathf.Sin(radians) + vector.y * Mathf.Cos(radians)));
    }
    public static Vector2Int RotateDegrees(this Vector2Int vector, float degrees) => RotateRadians(vector, Mathf.Deg2Rad * degrees);
}
