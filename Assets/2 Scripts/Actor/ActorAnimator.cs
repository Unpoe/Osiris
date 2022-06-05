using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Osiris
{
    [System.Serializable]
    public struct ActorAnimator
    {
        private PlayableGraph graph;

        public void Configure(Animator animator, ActorAnimationConfig config) {
            graph = PlayableGraph.Create();
            graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

            var clip = AnimationClipPlayable.Create(graph, config.Move);

            var output = AnimationPlayableOutput.Create(graph, "Enemy", animator);
            output.SetSourcePlayable(clip);
        }

        public void Destroy() {
            graph.Destroy();
        }

        public void Play(float speed) {
            graph.GetOutput(0).GetSourcePlayable().SetSpeed(speed);
            graph.Play();
        }

        public void Stop() {
            graph.Stop();
        }

        public void GameUpdate(float dt) {
            graph.Evaluate(dt);
        }
    }
}