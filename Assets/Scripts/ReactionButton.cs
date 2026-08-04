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
        var clip = AudioManager.Instance.clips.bubblePopped;
        AudioManager.Instance.PlayClip(clip);
        ReactionManager.Instance.Shake();
        ReactionManager.Instance.bubbleController.BurstBubble();
        wasClickedOn = true;
        stopped = true;
        StopTime();
    }
    private void OnDisable() 
    {
        DisableThis();
    }
}
