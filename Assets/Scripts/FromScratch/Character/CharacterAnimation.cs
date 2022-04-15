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
        private List<ManagedAnimState> states;

        private List<string> activeGUIDs;
        
        #region Initialization
        public void Setup(Animator animator)
        {
            activeGUIDs = new List<string>();
            
            // //Destroy Existing Graph for Idempotency
            if (animator.playableGraph.IsValid())
            {
                animator.playableGraph.Destroy();
            }
            if (graph.IsValid()) graph.Destroy();

            //Create the Graph
            graph = PlayableGraph.Create(GraphName);
            graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            //Create Mixer, Bind to the animator
            var playableOutput = AnimationPlayableOutput.Create(graph, GraphName, animator);
            
            SetupDefaultState(); // Setup Default Anim Controller
            SetupAnimStates(); // Setup Overriding AnimController
            SetupAnimClips(); // Setup Anim Clips

            playableOutput.SetSourcePlayable(mixerGesturesOutput, 0);
            // Play the graph
            graph.Play();
        }

        private void SetupAnimClips()
        {
            clips = new List<ManagedAnimClip>();

            mixerGesturesInput = AnimationMixerPlayable.Create(graph, 1);
            mixerGesturesInput.ConnectInput(0, mixerStatesOutput, 0, 1f);
            mixerGesturesInput.SetInputWeight(0, 1f);

            mixerGesturesOutput = AnimationMixerPlayable.Create(graph, 1);
            mixerGesturesOutput.ConnectInput(0, mixerGesturesInput, 0, 1f);
            mixerGesturesOutput.SetInputWeight(0, 1f);
        }

        private void SetupDefaultState()
        {
            runtimeControllerPlayable = AnimatorControllerPlayable.Create(
                graph,
                runtimeController
            );
            
            mixerStatesInput = AnimationMixerPlayable.Create(graph, 1);
            mixerStatesInput.ConnectInput(0, runtimeControllerPlayable, 0, 1f);
            mixerStatesInput.SetInputWeight(0, 1f);
        }

        private void SetupAnimStates()
        {
            states = new List<ManagedAnimState>();
            mixerStatesOutput = AnimationMixerPlayable.Create(graph, 1);
            mixerStatesOutput.ConnectInput(0, mixerStatesInput, 0, 1f);
            mixerStatesOutput.SetInputWeight(0, 1f);
        }

        #endregion
        
        #region Clips
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
        #endregion
        
        #region States
        
        public void SetState(RuntimeAnimatorController rtc, AvatarMask avatarMask,
            float weight = 1.0f, float transition = 0.0f, float speed = 1.0f, int layer = 0, bool syncTime = false,
            params Parameter[] parameters) {
            ManagedAnimState prevPlayable;
            ManagedAnimState nextPlayable;

            int insertIndex = GetSurroundingStates(layer,
                out prevPlayable,
                out nextPlayable
            );

            if (prevPlayable == null && nextPlayable == null) {
                states.Add(ManagedAnimController.Create(
                    rtc, avatarMask, layer,
                    syncTime ? runtimeControllerPlayable.GetTime() : 0f,
                    transition, speed, weight,
                    ref graph,
                    ref mixerStatesInput,
                    ref mixerStatesOutput,
                    parameters
                ));
            } else if (prevPlayable != null) {
                if (prevPlayable.Layer == layer) {
                    prevPlayable.StretchDuration(transition);
                }

                states.Insert(insertIndex, ManagedAnimController.CreateAfter(
                    rtc, avatarMask, layer,
                    syncTime ? runtimeControllerPlayable.GetTime() : 0f,
                    transition, speed, weight,
                    ref graph,
                    prevPlayable,
                    parameters
                ));
            } else if (nextPlayable != null) {
                states.Insert(insertIndex, ManagedAnimController.CreateBefore(
                    rtc, avatarMask, layer,
                    syncTime ? runtimeControllerPlayable.GetTime() : 0f,
                    transition, speed, weight,
                    ref graph,
                    nextPlayable,
                    parameters
                ));
            }
        }
        
        public void SetState(AnimationClip animationClip, AvatarMask avatarMask,
            float weight = 1.0f, float transition = 0.0f, float speed = 1.0f, int layer = 0) {
            ManagedAnimState prevPlayable;
            ManagedAnimState nextPlayable;

            int insertIndex = GetSurroundingStates(layer,
                out prevPlayable,
                out nextPlayable
            );

            if (prevPlayable == null && nextPlayable == null) {
                states.Add(ManagedAnimStateClip.Create(
                    animationClip, avatarMask, layer, 0f,
                    transition, speed, weight,
                    ref graph,
                    ref mixerStatesInput,
                    ref mixerStatesOutput
                ));
            } else if (prevPlayable != null) {
                if (prevPlayable.Layer == layer) {
                    prevPlayable.StretchDuration(transition);
                }

                states.Insert(insertIndex, ManagedAnimStateClip.CreateAfter(
                    animationClip, avatarMask, layer, 0f,
                    transition, speed, weight,
                    ref graph,
                    prevPlayable
                ));
            } else if (nextPlayable != null) {
                states.Insert(insertIndex, ManagedAnimStateClip.CreateBefore(
                    animationClip, avatarMask, layer, 0f,
                    transition, speed, weight,
                    ref graph,
                    nextPlayable
                ));
            }
        }
        
        private int GetSurroundingStates(int layer, out ManagedAnimState prev, out ManagedAnimState next) {
            prev = null;
            next = null;

            for (int i = 0; i < states.Count; ++i) {
                if (states[i].Layer <= layer) {
                    prev = states[i];
                    return i;
                }

                next = states[i];
            }

            return 0;
        }
        
        
        public void ResetState(float time, int layer) {
            for (int i = 0; i < states.Count; ++i) {
                if (states[i].Layer == layer) {
                    states[i].OnExitState();
                    states[i].Stop(time);
                }
            }
        }

        public void ChangeStateWeight(int layer, float weight) {
            for (int i = states.Count - 1; i >= 0; --i) {
                if (states[i].Layer == layer) {
                    states[i].SetWeight(weight);
                }
            }
        }
        
        #endregion

        public void Update()
        {
            for (int i = states.Count - 1; i >= 0; --i)
            {
                bool remove = states[i].Update();
                if (remove)
                {
                    states[i].Destroy(this);
                    states.RemoveAt(i);
                }
            }
            
            for (int i = clips.Count - 1; i >= 0; --i)
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

        void OnDisable()
        {
            graph.Destroy();
        }

    }
}
