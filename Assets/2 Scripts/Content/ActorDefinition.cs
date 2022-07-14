using UnityEngine;

namespace Osiris
{
    [CreateAssetMenu(fileName = "ActorDefinition_SKU", menuName = "Osiris/Content/ActorDefinition")]
    public class ActorDefinition : ScriptableObject
    {
        [SerializeField] private ActorId id = default;
        [Space]
        [SerializeField] private Actor prefab = default;
        [Header("Combat")]
        [SerializeField] private float hp = 100f;
        [SerializeField, Min(1)] private int range = 1;
        [SerializeField] private float speed = 1f;
        [SerializeField] private float attackSpeed = 1f; // attacks per second
        [SerializeField] private float attackDamage = 10f;
        [Header("Animation")]
        [SerializeField] private ActorAnimationConfig animationConfig = default;
        [Header("UI")]
        [SerializeField] private Sprite portrait = default;

        public ActorId Id => id;
        public Actor Prefab => prefab;
        public float Hp => hp;
        public int Range => range;
        public float Speed => speed;
        public float AttackSpeed => attackSpeed;
        public float AttackDamage => attackDamage;
        public ActorAnimationConfig AnimationConfig => animationConfig;
        public Sprite Portrait => portrait;

        // Used only in editor
        public void SetEditorDependencies(ActorId id, GameObject prefab, ActorAnimationConfig animationConfig, Sprite portrait) {
            this.id = id;
            this.prefab = prefab.GetComponent<Actor>();
            this.animationConfig = animationConfig;
            this.portrait = portrait;
        }
    }
}