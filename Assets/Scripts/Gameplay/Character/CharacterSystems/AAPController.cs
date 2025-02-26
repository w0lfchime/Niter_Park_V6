using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using System.Collections.Generic;
using System;


public enum STDAnimState
{
    Other,
    GroundedIdle,
    GroundedLocomotion,
    Jump,
    IdleAirborne,
    NearGrounding,
    OnGrounding,
}

public class AAPController
{
    private Character owner;

    public Animator animator;
    private PlayableGraph playableGraph;
    private AnimationPlayableOutput playableOutput;
    private AnimationClipPlayable clipPlayable;

    private float ups;
    private bool useFrameStepping = false;
    private float currentFrame = 0;
    private AnimationClip currentClip;

    public STDAnimState currentAnimatorState;
    public Dictionary<string, (int, float)> animData = new Dictionary<string, (int, float)>();

    public AAPController(Character character)
    {
        this.owner = character;
        this.animator = owner.animator;
        this.ups = owner.logicUPS;
    }

    // Animator Setup
    public void Setup()
    {
        if (animator == null || animator.runtimeAnimatorController == null)
        {
            Debug.LogWarning("Animator or Animator Controller is missing!");
            return;
        }

        // Get all animation clips from the Animator Controller
        RuntimeAnimatorController controller = animator.runtimeAnimatorController;
        AnimationClip[] clips = controller.animationClips;

        foreach (AnimationClip clip in clips) //TODO: COMBINE
        {
            if (clip == null) continue;

            float clipLength = clip.length;    // Duration in seconds
            float frameRate = clip.frameRate;  // Frames per second (FPS)
            int frameDuration = Mathf.RoundToInt(clipLength * frameRate); // Total frame count

            // Populate the dictionary
            animData[clip.name] = (frameDuration, clipLength);
        }

    }
    public void PlayInFrames(STDAnimState anim, int frames)
    {
        // Retrieve animation clip length and frame count from dictionary
        (int clipFrames, float clipLength) = animData[anim.ToString()];

        if (clipFrames <= 0 || clipLength <= 0)
        {
            Debug.LogWarning($"Invalid animation data for '{anim}'.");
            return;
        }

        // Convert the target frame duration to seconds (assuming 60 FPS logic)
        float targetDuration = frames / ups;

        // Calculate the speed multiplier to match the desired frame duration
        float speedMultiplier = clipLength / targetDuration;

        // Play animation with adjusted speed
        //animator.CrossFade(anim, owner.stdFade); // Smooth transition (adjust fade time if needed)
        SetAnimatorState(anim, owner.stdFade);
        animator.speed = speedMultiplier;
    }

    public void SetAnimatorState(STDAnimState state)
    {
        currentAnimatorState = state;
        animator.CrossFadeInFixedTime(state.ToString(), owner.stdFade);
    }
    public void SetAnimatorState(STDAnimState state, float cfTime)
    {
        currentAnimatorState = state;
        animator.CrossFadeInFixedTime(state.ToString(), cfTime);
    }
    public void SoftSetAnimatorState(STDAnimState state)
    {
        if (state == currentAnimatorState)
        {
            return;
        }
        SetAnimatorState(state);
    }
    public bool IsPlaying(STDAnimState stateName)
    {
        //return animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
        return false; //TODO: USING THIS ?
    }



    //Playables
    public void PlayFrameStepped(AnimationClip clip)
    {
        if (clip == null) return;

        currentClip = clip;
        currentFrame = 0;
        useFrameStepping = true;

        if (!playableGraph.IsValid())
        {
            SetupPlayables();
        }

        clipPlayable = AnimationClipPlayable.Create(playableGraph, clip);
        playableOutput.SetSourcePlayable(clipPlayable);
        playableGraph.Play();
    }

    private void SetupPlayables()
    {
        playableGraph = PlayableGraph.Create("FSA_Playables");
        playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", animator);
    }

    public void StepFrame(int frames = 1)
    {
        if (!useFrameStepping || currentClip == null) return;

        float frameRate = currentClip.frameRate;
        currentFrame += frames;

        float normalizedTime = (currentFrame / frameRate) / currentClip.length;
        normalizedTime = Mathf.Clamp01(normalizedTime);

        clipPlayable.SetTime(normalizedTime * currentClip.length);
        playableGraph.Evaluate(); // Apply manual frame stepping
    }


    public void Cleanup()
    {
        if (playableGraph.IsValid())
        {
            playableGraph.Destroy();
        }
    }
}
