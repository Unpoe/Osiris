using System.IO;
using UnityEngine;

namespace Osiris.Persistance
{
    public class GameDataReader
    {
        private BinaryReader reader;

        public GameDataReader(BinaryReader reader) {
            this.reader = reader;
        }

        public float ReadFloat() {
            return reader.ReadSingle();
        }

        public int ReadInt() {
            return reader.ReadInt32();
        }

        public bool ReadBool() {
            int value = reader.ReadInt32();
            return value == 0;
        }

        public Quaternion ReadQuaternion() {
            Quaternion value;

            value.x = reader.ReadSingle();
            value.y = reader.ReadSingle();
            value.z = reader.ReadSingle();
            value.w = reader.ReadSingle();

            return value;
        }

        public Vector3 ReadVector3() {
            Vector3 value;

            value.x = reader.ReadSingle();
            value.y = reader.ReadSingle();
            value.z = reader.ReadSingle();

            return value;
        }

        public HexCoordinates ReadHexCoordinates() {
            int x = reader.ReadInt32();
            int z = reader.ReadInt32();

            return new HexCoordinates(x, z);
        }
    }
}