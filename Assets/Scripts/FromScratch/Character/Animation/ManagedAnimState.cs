using UnityEngine;
using UnityEngine.Playables;

namespace FromScratch.Character.Animation
{
    public class ManagedAnimState: ManagedAnimBase
    {
        public int Layer { get; protected set; }
        
        protected bool isDisposing;
        private float currentWeight;
        
        protected ManagedAnimState(AvatarMask avatarMask,
            int layer, float time, float speed, float weight = 1.0f)
            : base(avatarMask, time, 0f, speed, weight) {
            Layer = layer;
        }

        public override bool Update()
        {            
            if (Input1.IsDone()) {
                Stop(0f);
                return true;
            }

            float increment = currentWeight < weight
                ? fadeIn
                : -fadeOut;

            if (Mathf.Abs(increment) < float.Epsilon) currentWeight = weight;
            else currentWeight += Time.deltaTime / increment;

            UpdateMixerWeights(Mathf.Clamp01(currentWeight));
            return isDisposing && currentWeight < float.Epsilon;
        }
        
        public override void Stop(float fadeOut) {
            base.Stop(fadeOut);

            isDisposing = true;
            weight = 0f;
        }

        public void StretchDuration(float freezeTime) {
            double duration = Input1.GetTime() + freezeTime;
            Input1.SetDuration(duration);
            Input1.SetSpeed(1f);
        }

        public void SetWeight(float weight) {
            this.weight = weight;
        }

        public virtual void OnExitState() { }
    }
}