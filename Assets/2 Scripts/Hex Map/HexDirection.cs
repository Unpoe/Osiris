namespace Osiris
{
    public enum HexDirection
    {
        NE, E, SE, SW, W, NW
    }

    public static class HexDirectionExtensions
    {
        private static float[] anglesPerDirection = new float[] {
            30, 90, 150, 210, 270, 330
        };

        public static HexDirection Opposite(this HexDirection direction) {
            return (int)direction < 3 ? (direction + 3) : (direction - 3);
        }

        public static HexDirection Previous(this HexDirection direction) {
            return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
        }

        public static HexDirection Next(this HexDirection direction) {
            return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
        }

        public static float GetYAngle(this HexDirection direction) {
            return anglesPerDirection[(int)direction];
        }
    }
}