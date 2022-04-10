using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace FromScratch.Character.Animation
{
    public class ManagedAnimStateClip: ManagedAnimState
    {
        public AnimationClip AnimationClip { get; protected set; }
        
        private ManagedAnimStateClip(AnimationClip animationClip, AvatarMask avatarMask,
                   int layer, float time, float speed, float weight)
                   : base(avatarMask, layer, time, speed, weight) {
            this.AnimationClip = animationClip;
        }
        
        public static ManagedAnimStateClip Create<TInput0, TOutput>(
            AnimationClip animationClip, AvatarMask avatarMask, int layer,
            double startTime, float fade, float speed, float weight,
            ref PlayableGraph graph, ref TInput0 input0, ref TOutput output)
            where TInput0 : struct, IPlayable
            where TOutput : struct, IPlayable {
            ManagedAnimStateClip state = new ManagedAnimStateClip(
                animationClip, avatarMask,
                layer, fade, speed, weight
            );

            AnimationClipPlayable input1 = AnimationClipPlayable.Create(graph, animationClip);

            input1.SetTime(startTime);
            input1.SetSpeed(speed);

            state.SpliceIntoGraph(ref graph, ref input0, ref input1, ref output);
            return state;
        }

        public static ManagedAnimState CreateAfter(
            AnimationClip animationClip, AvatarMask avatarMask, int layer,
            double startTime, float fade, float speed, float weight,
            ref PlayableGraph graph, ManagedAnimBase previous) {
            ManagedAnimStateClip state = new ManagedAnimStateClip(
                animationClip, avatarMask,
                layer, fade, speed, weight
            );

            AnimationClipPlayable input1 = AnimationClipPlayable.Create(graph, animationClip);
            input1.SetTime(startTime);
            input1.SetSpeed(speed);

            state.SpliceIntoGraph(ref graph, previous, ref input1);
            return state;
        }

        public static ManagedAnimState CreateBefore(
            AnimationClip animationClip, AvatarMask avatarMask, int layer,
            double startTime, float fade, float speed, float weight,
            ref PlayableGraph graph, ManagedAnimBase next) {
            ManagedAnimStateClip state = new ManagedAnimStateClip(
                animationClip, avatarMask,
                layer, fade, speed, weight
            );

            AnimationClipPlayable input1 = AnimationClipPlayable.Create(graph, animationClip);
            input1.SetTime(startTime);
            input1.SetSpeed(speed);

            state.SpliceIntoGraph(ref graph, ref input1, next);
            return state;
        }
    }
}