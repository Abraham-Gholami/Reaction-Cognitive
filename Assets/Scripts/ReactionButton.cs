using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReactionButton : MonoBehaviour
{
    public StimulusType type;
    public float reactionTimer, timer;
    bool stopped;
    [SerializeField]
    StateData stateData;
    [SerializeField]
    public Answer answer;
    [SerializeField]
    Image image;
    [SerializeField]
    AudioSource audioSource;
    public void Play()
    {
        audioSource.Play();
    }
    // Start is called before the first frame update
    void OnEnable() 
    {
        RandomButtonGenerator.Instance.bubble.answer = answer;
        RandomButtonGenerator.Instance.bubble.stateData = stateData;
        reactionTimer = type == StimulusType.Visual ? RandomButtonGenerator.Instance.visualTimer : RandomButtonGenerator.Instance.audioTimer;
        wasClickedOn = false;
        if(image)
            image.enabled = true;
        ResetTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if(!stopped)
            timer += Time.deltaTime;
        if(timer >= reactionTimer && !stopped)
        {
            timer = 0;
            StopTime();
        }
    }
    public void StopTime()
    {
        stopped = true;
        gameObject.SetActive(false);

    }
    void DisableThis()
    {
        //SaveStimulusData();
        if(image)
            image.enabled = false;
        
    }
    void ResetTimer()
    {
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
        // BubbleGO is the FIRST child of Stimules and this stimulus image is drawn over
        // it, so for the first visualTimer/audioTimer seconds of a trial the tap lands
        // here, not on the bubble. This used to burst the bubble directly without ever
        // setting Bubble.wasClickedOn - and Bubble.OnDisable is what writes the row, so
        // a response fast enough to hit the stimulus itself (under 0.2s on a visual
        // trial, 0.6s on an audio one) was exported as an omission error, scored
        // nothing, and reset the child's combo. Hand the tap to the bubble instead, so
        // there is one code path for a response however it is delivered.
        RandomButtonGenerator.Instance.bubble.OnButtonClicked();
        wasClickedOn = true;
        stopped = true;
        StopTime();
    }
    private void OnDisable() 
    {
        DisableThis();
    }
}
