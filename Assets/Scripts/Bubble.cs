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
    void SaveStimulusData()
    {
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
        SaveStimulusData();
    }
}
