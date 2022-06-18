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

        public static int GetRotationStepsTo(this HexDirection direction, HexDirection other) {
            // This function makes my eyes cry. Is there any way I can do this better?
            if(direction == other) {
                return 0;
            } else if (direction.Opposite() == other) {
                return 3;
            } else if (direction.Next() == other) {
                return 1;
            } else if (other.Next() == direction) {
                return -1;
            } else if(direction.Next().Next() == other) {
                return 2;
            } else {
                return -2;
            }
        }
    }
}