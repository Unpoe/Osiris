using UnityEngine;

namespace Osiris
{
    [CreateAssetMenu(fileName = "ActorAnimConfig_NAME", menuName = "Osiris/Animation/Actor Animation Config")]
    public class ActorAnimationConfig : ScriptableObject
    {
        [SerializeField] private AnimationClip move = default;
        [SerializeField] private AnimationClip attack = default;

        public AnimationClip Move => move;
        public AnimationClip Attack => attack;
    }
}