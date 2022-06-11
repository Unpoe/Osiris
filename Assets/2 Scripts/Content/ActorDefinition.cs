﻿using UnityEngine;

namespace Osiris
{
    [CreateAssetMenu(fileName = "ActorDefinition_SKU", menuName = "Osiris/Content/ActorDefinition")]
    public class ActorDefinition : ScriptableObject
    {
        [SerializeField] private ActorId id = default;
        [Header("Combat")]
        [SerializeField] private float hp = 1000f;
        [SerializeField, Min(1)] private int range = 1;
        [SerializeField] private float speed = 1f;
        [SerializeField] private float attackSpeed = 1f; // attacks per second
        [Header("Animation")]
        [SerializeField] private ActorAnimationConfig animationConfig = default;
        [Header("UI")]
        [SerializeField] private Sprite protrait = default;

        public ActorId Id => id;
        public float Hp => hp;
        public int Range => range;
        public float Speed => speed;
        public float AttackSpeed => attackSpeed;
        public ActorAnimationConfig AnimationConfig => animationConfig;
        public Sprite Portrait => protrait;
    }
}