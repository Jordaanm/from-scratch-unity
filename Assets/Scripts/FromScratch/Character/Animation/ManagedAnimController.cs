using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace FromScratch.Character.Animation
{
    public class ManagedAnimController: ManagedAnimState
    {
        protected ManagedAnimController(AvatarMask avatarMask,
            int layer,
            float time, float speed, float weight)
            : base(avatarMask, layer, time, speed, weight) { }
        
        
        public static ManagedAnimController Create<TInput0, TOutput>(
            RuntimeAnimatorController runtimeController, AvatarMask avatarMask, int layer,
            double startTime, float fade, float speed, float weight,
            ref PlayableGraph graph, ref TInput0 input0, ref TOutput output,
            params Parameter[] parameters)
            where TInput0 : struct, IPlayable
            where TOutput : struct, IPlayable {
            
            ManagedAnimController state = new ManagedAnimController(
                avatarMask, layer,
                fade, speed, weight
            );

            AnimatorControllerPlayable input1 = AnimatorControllerPlayable.Create(
                graph,
                runtimeController
            );

            if (parameters != null)
            {
                foreach (Parameter parameter in parameters) {
                    input1.SetFloat(parameter.id, parameter.value);
                }
            }

            input1.SetTime(startTime);
            input1.SetSpeed(speed);

            state.SpliceIntoGraph(ref graph, ref input0, ref input1, ref output);
            return state;
        }
        
        
        public static ManagedAnimController CreateAfter(
            RuntimeAnimatorController runtimeController, AvatarMask avatarMask, int layer,
            double startTime, float fade, float speed, float weight,
            ref PlayableGraph graph, ManagedAnimBase previous,
            params Parameter[] parameters) {
            
            ManagedAnimController state = new ManagedAnimController(
                avatarMask, layer,
                fade, speed, weight
            );

            AnimatorControllerPlayable input1 = AnimatorControllerPlayable.Create(
                graph,
                runtimeController
            );

            foreach (Parameter parameter in parameters) {
                input1.SetFloat(parameter.id, parameter.value);
            }

            input1.SetTime(startTime);
            input1.SetSpeed(speed);

            state.SpliceIntoGraph(ref graph, previous, ref input1);
            return state;
        }

        public static ManagedAnimController CreateBefore(
            RuntimeAnimatorController runtimeController, AvatarMask avatarMask, int layer,
            double startTime, float fade, float speed, float weight,
            ref PlayableGraph graph, ManagedAnimBase next,
            params Parameter[] parameters) {
            ManagedAnimController state = new ManagedAnimController(
                avatarMask, layer,
                fade, speed, weight
            );

            AnimatorControllerPlayable input1 = AnimatorControllerPlayable.Create(
                graph,
                runtimeController
            );

            foreach (Parameter parameter in parameters) {
                input1.SetFloat(parameter.id, parameter.value);
            }

            input1.SetTime(startTime);
            input1.SetSpeed(speed);

            state.SpliceIntoGraph(ref graph, ref input1, next);
            return state;
        }
    }
}