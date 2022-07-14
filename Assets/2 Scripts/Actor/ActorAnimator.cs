using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Osiris
{
    [System.Serializable]
    public struct ActorAnimator
    {
        public enum Clip { Idle, Walk, Attack }

        private PlayableGraph graph;
        private AnimationMixerPlayable mixer;

        public Clip CurrentClip { get; private set; }

        public bool IsDone => GetPlayable(CurrentClip).IsDone();

        private float idleAnimationDuration;
        private float attackAnimationDuration;
        public float attackEventNormalizedTime { get; private set; }

        // Transition variables
        private Clip previousClip;
        private float transitionProgress;

        private const float DEFAULT_TRANSITION_SPEED = 5f;

        public void Configure(Animator animator, ActorAnimationConfig config) {
            graph = PlayableGraph.Create();
            graph.SetTimeUpdateMode(DirectorUpdateMode.Manual); // We will update the animator manually with GameUpdate function
            mixer = AnimationMixerPlayable.Create(graph, 3);

            // Configure Idle
            var clip = AnimationClipPlayable.Create(graph, config.Idle);
            mixer.ConnectInput((int)Clip.Idle, clip, 0);
            idleAnimationDuration = config.Idle.length;
            // Configure Walk
            clip = AnimationClipPlayable.Create(graph, config.Walk);
            clip.Pause();
            mixer.ConnectInput((int)Clip.Walk, clip, 0);
            // Configure Attack
            clip = AnimationClipPlayable.Create(graph, config.Attack);
            //clip.SetDuration(config.Attack.length); // For non looping animations, we need to configure the duration
            attackAnimationDuration = config.Attack.length;
            attackEventNormalizedTime = config.AttackEventNormalizedTime;
            //if(config.Attack.events.Length == 0) {
            //    attackEventNormalizedTime = 1f;
            //} else {
            //    attackEventNormalizedTime = config.Attack.events[0].time / config.Attack.length;
            //}
            clip.Pause();
            mixer.ConnectInput((int)Clip.Attack, clip, 0);

            var output = AnimationPlayableOutput.Create(graph, "Actor", animator);
            output.SetSourcePlayable(mixer);

            transitionProgress = -1;
        }

        public void Destroy() {
            graph.Destroy();
        }

        public void Stop() {
            graph.Stop();
        }

        public void GameUpdate(float dt) {
            // Update transition (if any)
            if(transitionProgress >= 0f) {
                transitionProgress += dt * DEFAULT_TRANSITION_SPEED;
                if (transitionProgress >= 1f) {
                    transitionProgress = -1f;
                    SetWeight(CurrentClip, 1f);
                    SetWeight(previousClip, 0f);
                    GetPlayable(previousClip).Pause();
                } else {
                    SetWeight(CurrentClip, transitionProgress);
                    SetWeight(previousClip, 1f - transitionProgress);
                }
            }

            // Update graph
            graph.Evaluate(dt);
        }

        public void PlayIdle(bool blendAnimation) {
            if (blendAnimation) {
                BeginTransition(Clip.Idle);
            } else {
                SetWeight(Clip.Idle, 1f);
                CurrentClip = Clip.Idle;
            }

            var clip = GetPlayable(Clip.Idle);
            clip.SetTime(CustomRandom.Range(0f, idleAnimationDuration));

            graph.Play();
        }

        public void PlayWalk(float speed) {
            GetPlayable(Clip.Walk).SetSpeed(speed);
            BeginTransition(Clip.Walk);
        }

        public void PlayAttack(float speed) {
            var clip = GetPlayable(Clip.Attack);
            clip.SetSpeed(attackAnimationDuration * speed);
            BeginTransition(Clip.Attack);

            // Offset the animation to synchronize the visuals and the logic
            clip.SetTime(attackAnimationDuration * attackEventNormalizedTime);
        }

        private void SetWeight(Clip clip, float weight) {
            mixer.SetInputWeight((int)clip, weight);
        }

        private Playable GetPlayable(Clip clip) {
            return mixer.GetInput((int)clip);
        }

        private void BeginTransition(Clip nextClip) {
            if(nextClip == CurrentClip) {
                return;
            }

            previousClip = CurrentClip;
            CurrentClip = nextClip;
            transitionProgress = 0f;
            GetPlayable(nextClip).Play();
        }
    }
}