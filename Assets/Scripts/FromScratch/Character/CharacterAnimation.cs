using System;
using System.Collections;
using System.Collections.Generic;
using FromScratch.Character.Animation;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace FromScratch.Character
{
    public class CharacterAnimation : MonoBehaviour
    {
        public const float EPSILON = 0.01f;
        private const string GraphName = "FromScratch Character Graph";
        
        private PlayableGraph graph;
        private RuntimeAnimatorController runtimeController;
        private AnimatorControllerPlayable runtimeControllerPlayable;

        private AnimationMixerPlayable mixerGesturesOutput;
        private AnimationMixerPlayable mixerGesturesInput;
        private AnimationMixerPlayable mixerStatesOutput;
        private AnimationMixerPlayable mixerStatesInput;

        private List<ManagedAnimClip> clips = new List<ManagedAnimClip>();
        private List<string> activeGUIDs;
        public void Setup(Animator animator)
        {
            activeGUIDs = new List<string>();
            
            // //Destroy Existing Graph for Idempotency
            if (animator.playableGraph.IsValid())
            {
                animator.playableGraph.Destroy();
            }
            if (this.graph.IsValid()) this.graph.Destroy();
            //
            //Create the Graph
            graph = PlayableGraph.Create(GraphName);
            graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            //Create Mixer, Bind to the animator
            var playableOutput = AnimationPlayableOutput.Create(graph, GraphName, animator);

            
            // Setup Default Anim Controller
            this.runtimeControllerPlayable = AnimatorControllerPlayable.Create(
                this.graph,
                this.runtimeController
            );
            this.mixerStatesInput = AnimationMixerPlayable.Create(this.graph, 1, true);
            this.mixerStatesInput.ConnectInput(0, this.runtimeControllerPlayable, 0, 1f);
            this.mixerStatesInput.SetInputWeight(0, 1f);
            
            // Setup Overriding AnimController
            this.mixerStatesOutput = AnimationMixerPlayable.Create(this.graph, 1, true);
            this.mixerStatesOutput.ConnectInput(0, this.mixerStatesInput, 0, 1f);
            this.mixerStatesOutput.SetInputWeight(0, 1f);
            
            //Setup Anim Clips
            this.clips = new List<ManagedAnimClip>();
            
            this.mixerGesturesInput = AnimationMixerPlayable.Create(this.graph, 1, true);
            this.mixerGesturesInput.ConnectInput(0, this.mixerStatesOutput, 0, 1f);
            this.mixerGesturesInput.SetInputWeight(0, 1f);

            this.mixerGesturesOutput = AnimationMixerPlayable.Create(this.graph, 1, true);
            this.mixerGesturesOutput.ConnectInput(0, this.mixerGesturesInput, 0, 1f);
            this.mixerGesturesOutput.SetInputWeight(0, 1f);
            
            playableOutput.SetSourcePlayable(mixerGesturesOutput, 0);
            // Play the graph
            graph.Play();
        }

        void OnDisable()
        {
            graph.Destroy();
        }

        public Coroutine PlayClip(AnimationClip clip, Action callback = null)
        {
            Debug.Log("CharacterAnimation::PlayClip");
            //Create Self Contained Clip from clip
            ManagedAnimClip managedClip = ManagedAnimClip.Create(clip, null,
                0f, 0f, 1.0f,
                ref graph, ref mixerGesturesInput, ref mixerGesturesOutput
            );

            // Add to tracking list, so we know when to remove it's finished and remove it from the graph
            clips.Add(managedClip);
            activeGUIDs.Add(managedClip.Guid);

            Coroutine animCoroutine = StartCoroutine(TrackAnimation(managedClip.Guid, callback));
            return animCoroutine;

            //Splice into Graph ( currently handled by Create method )
        }

        private IEnumerator TrackAnimation(string guid, Action callback = null)
        {
            yield return new WaitUntil(()=> !activeGUIDs.Contains(guid));
            callback?.Invoke();
        }

        public void Update()
        {
            for (int i = this.clips.Count - 1; i >= 0; --i)
            {
                bool shouldRemove = clips[i].Update();
                if (shouldRemove)
                {
                    activeGUIDs.Remove(clips[i].Guid);
                    clips[i].Destroy(this);
                    clips.RemoveAt(i);
                    
                }
            }
        }
    }
}
