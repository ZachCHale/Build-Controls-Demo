using UnityEngine;

namespace ZHDev.Extensions
{

    public static class Vector2IntExtensions
    {
        /// <summary>
        /// Returns the closest Vector2Int after rotating around (0,0) by the given radians.
        /// </summary>
        /// <param name="vector">Vector2Int to rotate.</param>
        /// <param name="radians"></param>
        /// <returns>The result of the rotation rounded to the nearest Vector2Int.</returns>
        public static Vector2Int RotateRadians(this Vector2Int vector, float radians)
        {
            return new Vector2Int(Mathf.RoundToInt(vector.x * Mathf.Cos(radians) + vector.y * Mathf.Sin(radians)),
                Mathf.RoundToInt(-vector.x * Mathf.Sin(radians) + vector.y * Mathf.Cos(radians)));
        }
        /// <summary>
        /// Returns the closest Vector2Int after rotating around (0,0) by the given degrees.
        /// </summary>
        /// <param name="vector">Vector2Int to rotate.</param>
        /// <param name="degrees"></param>
        /// <returns>The result of the rotation rounded to the nearest Vector2Int.</returns>
        public static Vector2Int RotateDegrees(this Vector2Int vector, float degrees) =>
            RotateRadians(vector, Mathf.Deg2Rad * degrees);
    }
}