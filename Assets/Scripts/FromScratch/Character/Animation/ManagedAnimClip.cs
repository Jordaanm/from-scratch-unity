using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace FromScratch.Character.Animation
{
    public class ManagedAnimClip: ManagedAnimBase
    {

        private double endFreezeTime = -100.0;
        public string Guid { get; private set; }
        
        private ManagedAnimClip(AnimationClip clip, AvatarMask mask, float fadeIn, float fadeOut, float speed): 
            base(mask, fadeIn, fadeOut, speed)
        {
            Guid = System.Guid.NewGuid().ToString();
        }
    
        public static ManagedAnimClip Create<TInput0, TOutput>(
            AnimationClip animationClip, AvatarMask avatarMask,
            float fadeIn, float fadeOut, float speed,
            ref PlayableGraph graph, ref TInput0 input0, ref TOutput output
        ) where TInput0 : struct, IPlayable
            where TOutput : struct, IPlayable
        {
            ManagedAnimClip managedAnimClip = new ManagedAnimClip(animationClip, avatarMask, fadeIn, fadeOut, speed);

            AnimationClipPlayable clipPlayable = AnimationClipPlayable.Create(graph, animationClip);
        
            clipPlayable.SetTime(0f);
            clipPlayable.SetSpeed(speed);
            clipPlayable.SetDuration(animationClip.length);

            managedAnimClip.SpliceIntoGraph(ref graph, ref input0, ref clipPlayable, ref output);
            return managedAnimClip;

        }

        public override bool Update()
        {
            if (endFreezeTime > 0f && Time.time > endFreezeTime)
            {
                Stop(0f);
                return true;
            }

            if (Input1.IsDone())
            {
                Stop(0f);
                return true;
            }

            float time = (float)Input1.GetTime();
            if (time + fadeOut >= Input1.GetDuration())
            {
                float t = ((float)Input1.GetDuration() - time) / fadeOut;

                t = Mathf.Clamp01(t);
                UpdateMixerWeights(t);
            }
            else if (time <= fadeIn)
            {
                float t = time / fadeIn;

                t = Mathf.Clamp01(t);
                UpdateMixerWeights(t);
            }
            else
            {
                UpdateMixerWeights(1f);
            }

            return false;
        }
    
        public void StretchDuration(float extraTime) {
            if (Input1.GetDuration() - Input1.GetTime() < extraTime) {
                Input1.SetSpeed(0f);
                endFreezeTime = Time.time + extraTime;
            }
        }
        
        public override void Stop(float pFadeOut)
        {
            fadeOut = pFadeOut;

            Input1.SetDuration(Math.Min(
                Input1.GetTime() + pFadeOut,
                Input1.GetDuration())
            );
        }

    }
}
