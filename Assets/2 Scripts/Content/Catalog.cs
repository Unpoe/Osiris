using UnityEngine;

namespace Osiris
{
    [CreateAssetMenu(fileName = "Catalog", menuName = "Osiris/Content/Catalog", order = 0)]
    public class Catalog : ScriptableObject
    {
        [SerializeField] private ActorTable actorTable;
        public ActorTable ActorTable => actorTable;
    }
}