using UnityEngine;

namespace Osiris
{
    [CreateAssetMenu(fileName = "ActorDefinition_SKU", menuName = "Osiris/Content/ActorDefinition")]
    public class ActorDefinition : ScriptableObject
    {
        [SerializeField] private ActorId id = default;
        [Space]
        [SerializeField] private float hp = 1000f;
        [SerializeField, Min(1)] private int range = 1;
        [SerializeField] private float speed = 1f;
        [SerializeField] private float attackSpeed = 1f; // attacks per second
        [Space]
        [SerializeField] private ActorAnimationConfig animationConfig = default;

        public ActorId Id => id;
        public float Hp => hp;
        public int Range => range;
        public float Speed => speed;
        public float AttackSpeed => attackSpeed;
        public ActorAnimationConfig AnimationConfig => animationConfig;
    }
}