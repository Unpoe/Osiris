using System.IO;
using UnityEngine;

namespace Osiris.Persistance
{
    public class PersistentStorage
    {
        private string savePath;

        public PersistentStorage() {
            savePath = Path.Combine(Application.persistentDataPath, "saveFile");
        }

        public void Save(IPersistableObject o) {
            using (
                var writer = new BinaryWriter(File.Open(savePath, FileMode.Create))
            ) {
                o.Save(new GameDataWriter(writer));
            }
        }

        public void Load(IPersistableObject o) {
            using (
                var reader = new BinaryReader(File.Open(savePath, FileMode.Open))
            ) {
                o.Load(new GameDataReader(reader));
            }
        }
    }
}