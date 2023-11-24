using System.Collections.Generic;
using UnityEngine;

namespace CardinalDirections
{
    public enum CardinalDirection
    {
        North = 0,
        East = 1,
        South = 2,
        West = 3
    }

    public static class CardinalDirectionMethods
    {
        private static readonly Vector2[] _vector2Conversions = new Vector2[]
        { 
            new Vector2(0, 1), 
            new Vector2(1, 0), 
            new Vector2(0, -1), 
            new Vector2(-1, 0) 
        };
        private static readonly Vector2Int[] _vector2IntConversions = new Vector2Int[]
        { 
            new Vector2Int(0, 1), 
            new Vector2Int(1, 0), 
            new Vector2Int(0, -1), 
            new Vector2Int(-1, 0) 
        };
        private static readonly float[] _degreesConversion = new float[]
        {
            0, 
            90, 
            180, 
            270
        };
        private static readonly float[] _radiansConversion = new float[]
        {
            Mathf.Deg2Rad * 0,
            Mathf.Deg2Rad * 90,
            Mathf.Deg2Rad * 180,
            Mathf.Deg2Rad * 270,
        };

        private static readonly CardinalDirection[] _nextClockwise = new CardinalDirection[]
        {
            CardinalDirection.East,
            CardinalDirection.South,
            CardinalDirection.West,
            CardinalDirection.North,
        };

        private static readonly CardinalDirection[] _nextCounterclockwise = new CardinalDirection[]
        {
            CardinalDirection.West,
            CardinalDirection.South,
            CardinalDirection.East,
            CardinalDirection.North
        };
        
        private static readonly Quaternion[] _quaternionConversion = new Quaternion[]
        {
            Quaternion.identity, 
            Quaternion.AngleAxis(90, Vector3.up),
            Quaternion.AngleAxis(180, Vector3.up),
            Quaternion.AngleAxis(270, Vector3.up),
        };

        /// <summary>
        /// For a given Cardinal direction, returns the cardinal direction 90 degrees from this one.
        /// </summary>
        /// <param name="direction">Cardinal direction to rotate from.</param>
        /// <param name="counterClockwise">Specifies whether to rotate clockwise or counterclockwise.</param>
        /// <returns>The cardinal direction 90 degrees from this one.</returns>
        public static CardinalDirection Rotate(this CardinalDirection direction, bool counterClockwise = false) => counterClockwise ? _nextCounterclockwise[(int)direction] : _nextClockwise[(int)direction];
        /// <summary>
        /// Returns the Vector2 representation of the Cardinal Direction.
        /// </summary>
        /// <param name="direction">Cardinal Direction to convert to Vector2</param>
        /// <returns>
        /// North: <c>(0,1)</c>
        /// East: <c>(1,0)</c>
        /// South: <c>(0,-11)</c>
        /// West: <c>(-1,0)</c>
        /// </returns>
        public static Vector2 ToVector2(this CardinalDirection direction) => _vector2Conversions[(int)direction];
        /// <summary>
        /// Returns the Vector2Int representation of the Cardinal Direction.
        /// </summary>
        /// <param name="direction">Cardinal Direction to convert to Vector2Int</param>
        /// <returns>
        /// North: <c>(0,1)</c>
        /// East: <c>(1,0)</c>
        /// South: <c>(0,-11)</c>
        /// West: <c>(-1,0)</c>
        /// </returns>
        public static Vector2Int ToVector2Int(this CardinalDirection direction) => _vector2IntConversions[(int)direction];
        /// <summary>
        /// Returns the degrees past North clockwise for the given Cardinal Direction.
        /// </summary>
        /// <param name="direction">Cardinal Direction to convert to Degrees past north</param>
        /// <returns>
        /// North: <c>0</c>
        /// East: <c>90</c>
        /// South: <c>180</c>
        /// West: <c>270</c>
        /// </returns>
        public static float ToDegreesPastNorth(this CardinalDirection direction) => _degreesConversion[(int)direction];
        /// <summary>
        /// Returns the radians past North clockwise for the given Cardinal Direction.
        /// </summary>
        /// <param name="direction">Cardinal Direction to convert to Radians past north</param>
        /// <returns>
        /// North: <c>0</c>
        /// East: <c>π/2</c>
        /// South: <c>π</c>
        /// West: <c>3π/2</c>
        /// </returns>
        public static float ToRadiansPastNorth(this CardinalDirection direction) => _radiansConversion[(int)direction];
        
        /// <summary>
        /// Returns the Quaternion representing the rotation of the Cardinal Direction in relation to North.
        /// </summary>
        /// <param name="direction">Cardinal Direction to convert to Radians past north</param>
        /// <returns>
        /// North: <c>Quaternion.identity</c>
        /// East: <c>Quaternion.AngleAxis(90, Vector3.up)</c>
        /// South: <c>Quaternion.AngleAxis(180, Vector3.up)</c>
        /// West: <c>Quaternion.AngleAxis(270, Vector3.up</c>
        /// </returns>
        public static Quaternion ToRotationFromNorth(this CardinalDirection direction) => _quaternionConversion[(int)direction];





    }
}