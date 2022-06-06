using UnityEngine;

namespace Osiris
{
    [CreateAssetMenu(fileName = "ActorAnimConfig_NAME", menuName = "Osiris/Animation/Actor Animation Config")]
    public class ActorAnimationConfig : ScriptableObject
    {
        [SerializeField] private AnimationClip idle = default;
        [SerializeField] private AnimationClip walk = default;
        [SerializeField] private AnimationClip attack = default;

        public AnimationClip Idle => idle;
        public AnimationClip Walk => walk;
        public AnimationClip Attack => attack;
    }
}