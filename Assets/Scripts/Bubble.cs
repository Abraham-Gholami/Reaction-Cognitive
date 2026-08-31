using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public float reactionTimer, timer;
    bool stopped;
    public StateData stateData;
    [SerializeField]
    public Answer answer;
    [SerializeField]
    AudioSource audioSource;
    private void OnEnable() 
    {
        ResetTimer();
    }

    void Update()
    {
        if(!stopped)
            timer += Time.deltaTime;
    }
    
    void ResetTimer()
    {
        wasClickedOn = false;
        stopped = false;
        timer = 0;
    }

    // One row per presented trial, guaranteed. The row used to be a side effect of
    // OnDisable alone, so any path where that hook did not fire - the object already
    // inactive because a tap had burst it, a parent switched off, a coroutine killed
    // between the two SetActive calls - dropped a trial from the export without a
    // trace. StartTest now arms the bubble when it opens the trial and calls
    // EnsureRecorded when it closes it, so OnDisable stays the fast path (a tap still
    // credits the counter immediately) and this is the backstop.
    public bool Recorded { get; private set; }

    public void ArmForTrial()
    {
        Recorded = false;
        ResetTimer();
    }

    public void EnsureRecorded()
    {
        if(Recorded) return;
        SaveStimulusData();
    }
    void SaveStimulusData()
    {
        Recorded = true;
        StimulusData stimulusData = new StimulusData();
        stimulusData.answer = answer;
        stimulusData.stateData = stateData;
        stimulusData.reactionTimer = timer;
        stimulusData.wasClickedOn = wasClickedOn;
        stimulusData.startingTimer = RandomButtonGenerator.Instance.thisTryTimer;
        ReactionManager.Instance.RecevieStimulusFocuesdData(stimulusData);
    }
    bool wasClickedOn;
    public void OnButtonClicked()
    {
        if(SettingsManager.Instance.useBubbleSFX)
        {
            var clip = AudioManager.Instance.clips.bubblePopped;
            AudioManager.Instance.PlayClip(clip);
        }
        ReactionManager.Instance.Shake();
        wasClickedOn = true;
        stopped = true;
        ReactionManager.Instance.bubbleController.BurstBubble();
    }
    private void OnDisable() {
        // RandomButtonGenerator.Awake and scene teardown both deactivate this object.
        // Awake ordering between it and ReactionManager is undefined, so that first
        // SetActive(false) sometimes wrote a phantom trial row and sometimes did not.
        // Only a bubble closed during an actual test is a trial.
        var generator = RandomButtonGenerator.Instance;
        if(generator == null || !generator.GameIsPlaying) return;
        SaveStimulusData();
    }
}
