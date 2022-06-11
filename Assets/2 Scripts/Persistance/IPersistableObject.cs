namespace Osiris.Persistance
{
    public interface IPersistableObject
    {
        void Save(GameDataWriter writer);
        void Load(GameDataReader reader);
    }
}