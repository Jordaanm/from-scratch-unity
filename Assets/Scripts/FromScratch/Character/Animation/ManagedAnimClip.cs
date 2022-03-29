using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace FromScratch.Character.Animation
{
    public class ManagedAnimClip
    {

        protected float fadeIn;
        protected float fadeOut;

        protected float speed;
        protected float weight;
    
        private AvatarMask avatarMask;
        private AnimationLayerMixerPlayable mixer;
    
        private double endFreezeTime = -100.0;
    
    
        public Playable Mixer => this.mixer;
        public Playable Input0 => this.mixer.GetInput(0);
        public Playable Input1 => this.mixer.GetInput(1);
        public Playable Output => this.mixer.GetOutput(0);
    
        private ManagedAnimClip(AnimationClip clip, AvatarMask mask, float fadeIn, float fadeOut, float speed)
        {
            this.avatarMask = mask;

            this.fadeIn = fadeIn;
            this.fadeOut = fadeOut;

            this.speed = speed;
            this.weight = 1.0f;
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
    
        protected void SpliceIntoGraph<TInput0, TInput1, TOutput>(
            ref PlayableGraph graph, ref TInput0 input0, ref TInput1 input1, ref TOutput output)
            where TInput0 : struct, IPlayable
            where TInput1 : struct, IPlayable
            where TOutput : struct, IPlayable {

            //Detach from graph
            input0.GetOutput(0).DisconnectInput(0);
            output.DisconnectInput(0);
            input1.SetSpeed(this.speed);

            // Create new mixer
            this.mixer = AnimationLayerMixerPlayable.Create(graph, 2);

            // Connect previous mixer + new clip
            this.mixer.ConnectInput(0, input0, 0, 0f);
            this.mixer.ConnectInput(1, input1, 0, 1f);

            if (this.avatarMask != null) {
                this.mixer.SetLayerMaskFromAvatarMask(1, this.avatarMask);
            }

            //Hook new mixer in where old mixer was
            output.ConnectInput(0, this.mixer, 0, 1f);

            this.UpdateMixerWeights(this.fadeIn > CharacterAnimation.EPSILON
                ? 0f
                : this.weight
            );
        }

        private void UpdateMixerWeights(float newWeight) {
            float weight0 = 1f;
            float weight1 = newWeight;

            this.mixer.SetInputWeight(0, weight0);
            this.mixer.SetInputWeight(1, weight1);
        }
    
        public bool Update()
        {
            if (this.endFreezeTime > 0f && Time.time > this.endFreezeTime)
            {
                this.Stop(0f);
                return true;
            }

            if (this.Input1.IsDone())
            {
                this.Stop(0f);
                return true;
            }

            float time = (float)this.Input1.GetTime();
            if (time + this.fadeOut >= this.Input1.GetDuration())
            {
                float t = ((float)this.Input1.GetDuration() - time) / this.fadeOut;

                t = Mathf.Clamp01(t);
                this.UpdateMixerWeights(t);
            }
            else if (time <= this.fadeIn)
            {
                float t = time / this.fadeIn;

                t = Mathf.Clamp01(t);
                this.UpdateMixerWeights(t);
            }
            else
            {
                this.UpdateMixerWeights(1f);
            }

            return false;
        }
    
        public void Stop(float pFadeOut)
        {
            this.fadeOut = pFadeOut;

            this.Input1.SetDuration(Math.Min(
                this.Input1.GetTime() + pFadeOut,
                this.Input1.GetDuration())
            );
        }

        public void Destroy(CharacterAnimation characterAnimation)
        {
            Playable output = this.Output;
            Playable input0 = this.Input0;

            output.DisconnectInput(0);
            this.mixer.DisconnectInput(0);

            output.ConnectInput(0, input0, 0);

            switch (output.GetInputCount())
            {
                case 1:
                    output.SetInputWeight(0, 1f);
                    break;

                case 2:
                    float outputWeight = this.mixer.GetInputWeight(0);
                    output.SetInputWeight(0, outputWeight);
                    break;
            }

            IEnumerator destroy = this.DestroyNextFrame();
            characterAnimation.StartCoroutine(destroy);
        }
        private IEnumerator DestroyNextFrame()
        {
            yield return null;

            if (this.Input1.IsValid() && this.Input1.CanDestroy()) this.Input1.Destroy();
            if (this.Mixer.IsValid()  && this.Mixer.CanDestroy())  this.Mixer.Destroy();
        }
    }
}
