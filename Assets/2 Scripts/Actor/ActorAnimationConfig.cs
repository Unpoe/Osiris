using UnityEngine;

namespace Osiris
{
    [CreateAssetMenu(fileName = "ActorAnimConfig_NAME", menuName = "Osiris/Animation/Actor Animation Config")]
    public class ActorAnimationConfig : ScriptableObject
    {
        [SerializeField] private AnimationClip idle = default;
        [SerializeField] private AnimationClip walk = default;
        [SerializeField] private AnimationClip attack = default;
        [Space]
        [SerializeField] private float attackEventNormalizedTime = 1f;

        public AnimationClip Idle => idle;
        public AnimationClip Walk => walk;
        public AnimationClip Attack => attack;
        public float AttackEventNormalizedTime => attackEventNormalizedTime;

        // Used only in editor
        public void SetEditorDependencies(AnimationClip idle, AnimationClip walk, AnimationClip attack) {
            this.idle = idle;
            this.walk = walk;
            this.attack = attack;
        }
    }
}