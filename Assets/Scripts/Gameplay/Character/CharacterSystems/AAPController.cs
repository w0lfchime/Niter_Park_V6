using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using System.Collections.Generic;

//core animation controller 

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

        foreach (AnimationClip clip in clips)
        {
            if (clip == null) continue;

            float clipLength = clip.length;    // Duration in seconds
            float frameRate = clip.frameRate;  // Frames per second (FPS)
            int frameDuration = Mathf.RoundToInt(clipLength * frameRate); // Total frame count

            // Populate the dictionary
            animData[clip.name] = (frameDuration, clipLength);
        }
    }
    public void PlayInFrames(string anim, int frames)
    {
        if (!animData.ContainsKey(anim))
        {
            Debug.LogWarning($"Animation '{anim}' not found in dictionary.");
            return;
        }

        // Retrieve animation clip length and frame count from dictionary
        (int clipFrames, float clipLength) = animData[anim];

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
        animator.CrossFade(anim, owner.stdFade); // Smooth transition (adjust fade time if needed)
        animator.speed = speedMultiplier;
    }

    public void SetAnimatorState(string state)
    {
        animator.CrossFadeInFixedTime(state, owner.stdFade);
    }
    public void SetAnimatorState(string state, float cfTime)
    {
        animator.CrossFadeInFixedTime(state, cfTime);
    }
    public bool IsPlaying(string stateName)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
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
