using System.Collections.Generic;
using UnityEngine;

namespace Osiris
{
    [CreateAssetMenu(fileName = "ActorTable", menuName = "Osiris/Content/ActorTable")]
    public class ActorTable : ScriptableObject
    {
        // TODO: find a way to serialize definitions differently, because this way in inspector we don't know what are we assigning
        [EnumLabelArray(typeof(ActorId), (int)ActorId.None, (int)ActorId.Count)]
        [SerializeField] private ActorDefinition[] definitions = default;

        public void AddDefinition(ActorDefinition actorDefinition) {
            definitions[(int)actorDefinition.Id] = actorDefinition;
        }

        public IReadOnlyList<ActorDefinition> GetDefinitions() {
            return definitions;
        }

        public ActorDefinition GetDefinition(ActorId id) {
            int index = (int)id;
            if(index < definitions.Length && index >= 0) {
                return definitions[index];
            }

            return null;
        }
    }
}