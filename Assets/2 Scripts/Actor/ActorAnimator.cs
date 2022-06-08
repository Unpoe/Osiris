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

        public void Configure(Animator animator, ActorAnimationConfig config) {
            graph = PlayableGraph.Create();
            graph.SetTimeUpdateMode(DirectorUpdateMode.Manual); // We will update the animator manually with GameUpdate function
            mixer = AnimationMixerPlayable.Create(graph, 3);

            // Configure Idle
            var clip = AnimationClipPlayable.Create(graph, config.Idle);
            mixer.ConnectInput((int)Clip.Idle, clip, 0);
            // Configure Walk
            clip = AnimationClipPlayable.Create(graph, config.Walk);
            clip.Pause();
            mixer.ConnectInput((int)Clip.Walk, clip, 0);
            // Configure Attack
            clip = AnimationClipPlayable.Create(graph, config.Attack);
            //clip.SetDuration(config.Attack.length); // For non looping animations, we need to configure the duration
            clip.Pause();
            mixer.ConnectInput((int)Clip.Attack, clip, 0);

            var output = AnimationPlayableOutput.Create(graph, "Actor", animator);
            output.SetSourcePlayable(mixer);
        }

        public void Destroy() {
            graph.Destroy();
        }

        public void Stop() {
            graph.Stop();
        }

        public void GameUpdate(float dt) {
            graph.Evaluate(dt);
        }

        public void PlayIdle() {
            SetWeight(CurrentClip, 0f);
            SetWeight(Clip.Idle, 1f);
            CurrentClip = Clip.Idle;
            graph.Play();
        }

        public void PlayWalk(float speed) {
            SetWeight(CurrentClip, 0f);
            SetWeight(Clip.Walk, 1f);

            var clip = GetPlayable(Clip.Walk);
            clip.SetSpeed(speed);
            clip.Play();

            CurrentClip = Clip.Walk;
        }

        public void PlayAttack(float speed) {
            SetWeight(CurrentClip, 0f);
            SetWeight(Clip.Attack, 1f);

            var clip = GetPlayable(Clip.Attack);
            clip.SetSpeed(speed);
            clip.Play();

            CurrentClip = Clip.Attack;
        }

        private void SetWeight(Clip clip, float weight) {
            mixer.SetInputWeight((int)clip, weight);
        }

        private Playable GetPlayable(Clip clip) {
            return mixer.GetInput((int)clip);
        }
    }
}