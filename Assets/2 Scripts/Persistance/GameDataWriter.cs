using System.IO;
using UnityEngine;

namespace Osiris.Persistance
{
    public class GameDataWriter
    {
        private BinaryWriter writer;

        public GameDataWriter(BinaryWriter writer) {
            this.writer = writer;
        }

        public void Write(float value) {
            writer.Write(value);
        }

        public void Write(int value) {
            writer.Write(value);
        }

        public void Write(bool value) {
            writer.Write(value ? 0 : 1);
        }

        public void Write(Quaternion value) {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
            writer.Write(value.w);
        }

        public void Write(Vector3 value) {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
        }

        public void Write(HexCoordinates value) {
            writer.Write(value.x);
            writer.Write(value.z);
        }
    }
}