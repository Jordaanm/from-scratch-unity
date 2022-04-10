using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace FromScratch.Character.Animation
{        
    public struct Parameter
    {
        public int id;
        public float value;

        public Parameter(int id, float value) {
            this.id = id;
            this.value = value;
        }

        public Parameter(string parameter, float value)
            : this(Animator.StringToHash(parameter), value) { }
    }
    
    public abstract class ManagedAnimBase
    {
        protected float fadeIn;
        protected float fadeOut;

        protected float speed;
        protected float weight;
    
        protected AvatarMask avatarMask;
        protected AnimationLayerMixerPlayable mixer;
    
        public Playable Mixer => mixer;
        public Playable Input0 => mixer.GetInput(0);
        public Playable Input1 => mixer.GetInput(1);
        public Playable Output => mixer.GetOutput(0);
        
        protected ManagedAnimBase(AvatarMask mask, float fadeIn, float fadeOut, float speed, float weight = 1.0f)
        {
            avatarMask = mask;

            this.fadeIn = fadeIn;
            this.fadeOut = fadeOut;

            this.speed = speed;
            this.weight = weight;
        }
        
        
        public void Destroy(CharacterAnimation characterAnimation)
        {
            Playable output = Output;
            Playable input0 = Input0;

            output.DisconnectInput(0);
            mixer.DisconnectInput(0);

            output.ConnectInput(0, input0, 0);

            switch (output.GetInputCount())
            {
                case 1:
                    output.SetInputWeight(0, 1f);
                    break;

                case 2:
                    float outputWeight = mixer.GetInputWeight(0);
                    output.SetInputWeight(0, outputWeight);
                    break;
            }

            IEnumerator destroy = DestroyNextFrame();
            characterAnimation.StartCoroutine(destroy);
        }
        
        private IEnumerator DestroyNextFrame()
        {
            yield return null;

            if (Input1.IsValid() && Input1.CanDestroy()) Input1.Destroy();
            if (Mixer.IsValid()  && Mixer.CanDestroy())  Mixer.Destroy();
        }
        
        
        public virtual void Stop(float pFadeOut) {
            fadeOut = pFadeOut;
        }
        
        public abstract bool Update();
        
            
        protected void SpliceIntoGraph<TInput0, TInput1, TOutput>(
            ref PlayableGraph graph, ref TInput0 input0, ref TInput1 input1, ref TOutput output)
            where TInput0 : struct, IPlayable
            where TInput1 : struct, IPlayable
            where TOutput : struct, IPlayable {

            //Detach from graph
            input0.GetOutput(0).DisconnectInput(0);
            output.DisconnectInput(0);
            input1.SetSpeed(speed);

            // Create new mixer
            mixer = AnimationLayerMixerPlayable.Create(graph, 2);

            // Connect previous mixer + new clip
            mixer.ConnectInput(0, input0, 0, 0f);
            mixer.ConnectInput(1, input1, 0, 1f);

            if (avatarMask != null) {
                mixer.SetLayerMaskFromAvatarMask(1, avatarMask);
            }

            //Hook new mixer in where old mixer was
            output.ConnectInput(0, mixer, 0, 1f);

            UpdateMixerWeights(fadeIn > CharacterAnimation.EPSILON
                ? 0f
                : weight
            );
        }
        
        protected void SpliceIntoGraph<TInput1>(
            ref PlayableGraph graph, ManagedAnimBase previous, ref TInput1 input1)
            where TInput1 : struct, IPlayable {
            Playable input0 = previous.Mixer;
            Playable output = previous.Output;
            previous.Output.DisconnectInput(0);

            SpliceIntoGraph(ref graph, ref input0, ref input1, ref output);
        }
        
        protected void SpliceIntoGraph<TInput1>(
            ref PlayableGraph graph, ref TInput1 input1, ManagedAnimBase next)
            where TInput1 : struct, IPlayable {
            Playable input0 = next.Input0;
            Playable output = next.Mixer;
            output.DisconnectInput(0);

            SpliceIntoGraph(ref graph, ref input0, ref input1, ref output);
        }


        protected void UpdateMixerWeights(float newWeight) {
            float weight0 = 1f;
            float weight1 = newWeight;

            mixer.SetInputWeight(0, weight0);
            mixer.SetInputWeight(1, weight1);
        }
    }
}