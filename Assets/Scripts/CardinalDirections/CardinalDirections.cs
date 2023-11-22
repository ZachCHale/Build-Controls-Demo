namespace CardinalDirections
{
    public enum CardinalDirection
    {
        North,
        East,
        South,
        West
    }

    public static class CardinalDirectionMethods
    {
        public static CardinalDirection Rotate(this CardinalDirection prevDir, bool counterClockwise = false)
        {
            switch (prevDir)
            {
                case CardinalDirection.North:
                    return counterClockwise ? CardinalDirection.West : CardinalDirection.East;
                    break;
                case CardinalDirection.East:
                    return counterClockwise ? CardinalDirection.North : CardinalDirection.South;
                    break;
                case CardinalDirection.South:
                    return counterClockwise ? CardinalDirection.East : CardinalDirection.West;
                    break;
                case CardinalDirection.West:
                    return counterClockwise ? CardinalDirection.South : CardinalDirection.North;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(prevDir), prevDir, null);
            }
        }

    }
}
    
