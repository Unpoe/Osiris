using System.Collections.Generic;
using UnityEngine;

namespace Osiris
{
    [CreateAssetMenu(fileName = "ActorTable", menuName = "Osiris/Content/ActorTable")]
    public class ActorTable : ScriptableObject
    {
        [SerializeField] private ActorDefinition mockDefinition = default;

        [EnumLabelArray(typeof(ActorId), (int)ActorId.None, (int)ActorId.Count)]
        [SerializeField] private ActorDefinition[] definitions = default;

        private List<ActorDefinition> filteredDefinitions = new List<ActorDefinition>();

        // Used only in editor
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

            return mockDefinition;
        }
    }
}