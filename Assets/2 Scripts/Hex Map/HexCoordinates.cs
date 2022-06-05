using System;
using UnityEngine;

namespace Osiris
{
    public struct HexCoordinates : IEquatable<HexCoordinates>
    {
        public int x { get; private set; }
        public int y { get { return -x - z; } }
        public int z { get; private set; }

        public HexCoordinates(int x, int z) {
            this.x = x;
            this.z = z;
        }

        public static HexCoordinates FromOffsetCoordinates(int x, int z) {
            return new HexCoordinates(x - z / 2, z);
        }

        public static HexCoordinates FromPosition(Vector3 position) {
            float x = position.x / (HexMetrics.innerRadius * 2f);
            float y = -x;

            float offset = position.z / (HexMetrics.outerRadius * 3f);
            x -= offset;
            y -= offset;

            int iX = Mathf.RoundToInt(x);
            int iY = Mathf.RoundToInt(y);
            int iZ = Mathf.RoundToInt(-x - y);

            if (iX + iY + iZ != 0) {
                float dX = Mathf.Abs(x - iX);
                float dY = Mathf.Abs(y - iY);
                float dZ = Mathf.Abs(-x - y - iZ);

                if (dX > dY && dX > dZ) {
                    iX = -iY - iZ;
                } else if (dZ > dY) {
                    iZ = -iX - iY;
                }
            }

            return new HexCoordinates(iX, iZ);
        }

        public int DistanceTo(HexCoordinates other) {
            return
                ((x < other.x ? other.x - x : x - other.x) +
                (y < other.y ? other.y - y : y - other.y) +
                (z < other.z ? other.z - z : z - other.z)) / 2;

            //int xy =
            //    (x < other.x ? other.x - x : x - other.x) +
            //    (y < other.y ? other.y - y : y - other.y);

            //return (xy + (z < other.z ? other.z - z : z - other.z)) / 2;
        }

        public bool Equals(HexCoordinates other) {
            return x.Equals(other.x) && z.Equals(other.z);
        }

        public override string ToString() {
            return "(" + x.ToString() + ", " + y.ToString() + ", " + z.ToString() + ")";
        }

        public string ToStringOnSeparateLines() {
            return x.ToString() + "\n" + y.ToString() + "\n" + z.ToString();
        }
    }
}