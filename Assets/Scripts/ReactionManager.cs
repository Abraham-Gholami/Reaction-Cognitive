using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class ReactionManager : GenericSingleton<ReactionManager>
{
    [SerializeField]
    float magn, rough, fadeIn, fadeOut;
    public void Shake()
    {
        CameraShaker.Instance.ShakeOnce(magn, rough, fadeIn, fadeOut);
    }
    public void RestServerLevelData()
    {
        serverLevelData.stimulusData = new List<StimulusData>();
    }
    #region Mono
    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        data = new GyroData();
        data.ResetData();
        SettingsManager.Instance.settingPageClosed += OnSettingMenuClosed;
        serverLevelData.stimulusData = new List<StimulusData>();
        serverLevelData.stimiulusGeneralData = new List<string>();
        if(progressBar)
            progressBar.SetMaxFill(RandomButtonGenerator.Instance.TotalTrialSeconds());
        UpdateText();
        Input.gyro.enabled = true;

    }
    #endregion
    #region GatheringReactionData
    [SerializeField]
    Transform fishHolder,sharkHolder,diamondHolder;
    public void RecevieStimulusGeneralData(string stimulusData)
    {
        serverLevelData.stimiulusGeneralData.Add(stimulusData);
    }
    public void RecevieLevelDescription(string levelDescription)
    {
        serverLevelData.levelDescription = levelDescription;
    }
    public void AddInputData(TouchType touchType)
    {
        switch (touchType)
        {
            case TouchType.upperLeft:
            serverLevelData.upperLeft ++;
            break;
            case TouchType.middleLeft:
            serverLevelData.middleLeft ++;
            break;
            case TouchType.lowerLeft:
            serverLevelData.lowerLeft ++;
            break;
            case TouchType.upperMiddle:
            serverLevelData.upperMiddle ++;
            break;
            case TouchType.middleMiddle:
            serverLevelData.middleMiddle ++;
            break;
            case TouchType.lowerMiddle:
            serverLevelData.lowerMiddle ++;
            break;
            case TouchType.upperRight:
            serverLevelData.upperRight ++;
            break;
            case TouchType.middleRight:
            serverLevelData.middleRight ++;
            break;
            case TouchType.lowerRight:
            serverLevelData.lowerRight ++;
            break;
        }
    }
    Vector3 gyroData,acc;
    int maxSampleCount = 25,currentSampleCount;
    [SerializeField]
    GyroData data;
    private void Update() 
    {
        if(!RandomButtonGenerator.Instance.GameIsPlaying) return;
        if(timer < maxTimer)
        {
            timer += Time.deltaTime;
            if(currentSampleCount < maxSampleCount)
            {
                gyroData = Input.gyro.userAcceleration;
                acc = Input.acceleration;
                data.acceleration.Add(acc);
                data.gyroscope.Add(gyroData);
                currentSampleCount ++;
            }
            
        }
        else
        {
            currentSampleCount = 0;
            data.second ++;
            // Reset first, and never let a writer fault strand the timer above maxTimer:
            // that turned a single bad second into an exception on every later frame.
            timer = 0;
            try
            {
                CSVBuilder.Instance.GatherGyroscopeData(data);
                CSVBuilder.Instance.AutoSaveGyro();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Gyro write failed: " + e.Message);
            }
            data.ResetData();
        }
    }
    float maxTimer = 1,timer;
    public int numberOfTriesBeforeTutEnd;

    public void RecevieStimulusFocuesdData(StimulusData stimulus)
    {
        if(serverLevelData.stimulusData != null)
        {
            stimulus.tryNumber = RandomButtonGenerator.Instance.stateCounter;
            // The audio was taken by a call at some point after this trial opened, so
            // the child may have answered without hearing the stimulus.
            stimulus.interrupted =
                CallInterruptionGuard.LastInterruptionTime >= RandomButtonGenerator.Instance.TrialStartTime;
            stimulus.levelDescription = serverLevelData.levelDescription;
            stimulus.upperLeft = serverLevelData.upperLeft;
            stimulus.middleLeft = serverLevelData.middleLeft;
            stimulus.lowerLeft = serverLevelData.lowerLeft;
            stimulus.upperMiddle = serverLevelData.upperMiddle;
            stimulus.middleMiddle = serverLevelData.middleMiddle;
            stimulus.lowerMiddle = serverLevelData.lowerMiddle;
            stimulus.upperRight = serverLevelData.upperRight;
            stimulus.middleRight = serverLevelData.middleRight;
            stimulus.lowerRight = serverLevelData.lowerRight;
            stimulus.starTaps = numberOfTriesBeforeTutEnd;
            serverLevelData.ResetCounter();
            gyroData = Vector3.zero;
            currentSampleCount = 0;
            acc = Vector3.zero;
            serverLevelData.stimulusData.Add(stimulus);
            CSVBuilder.Instance.ToCSV(stimulus);

        }
        if(stimulus.answer == Answer.Right && stimulus.wasClickedOn)
        {
            if(posCounter < maxCombo)
                posCounter ++;
        }
        else if(stimulus.answer == Answer.Wrong && stimulus.wasClickedOn || stimulus.answer == Answer.Right && !stimulus.wasClickedOn && RandomButtonGenerator.Instance.level.isTraining )
        {
            negCounter ++;
            posCounter = 0;
        }
        if(negCounter >= 3 && RandomButtonGenerator.Instance.level.isTraining)
        {
            negCounter = 0;
            RandomButtonGenerator.Instance.StopCR();
        }
        //if(posCounter >= 10)
        //{
        //    var clip = AudioManager.Instance.clips.posEnforcer;
        //    AudioManager.Instance.PlayClip(clip);
        //    posCounter = 0;
        //}
        AnimateState(stimulus);
        //AnimateSlider(stimulus);
    }
    public void ClearCombo()
    {   
        posCounter = 0;
    }
    public ProgressBar progressBar;
    int sliderValue;
    void AnimateSlider(StimulusData stimulus)
    {
        if(stimulus.answer == Answer.Right && stimulus.wasClickedOn || stimulus.answer == Answer.Wrong && !stimulus.wasClickedOn)
        {
            sliderValue ++;
        }
        else if(stimulus.answer == Answer.Wrong && stimulus.wasClickedOn ||  stimulus.answer == Answer.Right && !stimulus.wasClickedOn)
        {
            sliderValue --;
        }
        progressBar.Animate(sliderValue);

    }
    [SerializeField] AudioSource diamondSFX;
    void AnimateState(StimulusData stimulus)
    {
       if(stimulus.answer == Answer.Right && stimulus.wasClickedOn)
       {
           void Completed()
           {
                diamondCounter ++;
                diamondCounterText.text = diamondCounter.ToString();
                if(SettingsManager.Instance.diamondSFX) diamondSFX.Play();
           }
           if(!SettingsManager.Instance.useCombo)  fishCounter += 1;
           else fishCounter += posCounter * 1;
           UIAnimationController.Instance.Animate(true,fishHolder.position,posCounter);
           if(stimulus.reactionTimer < 0.6 && SettingsManager.Instance.useDiamondPrize)
           {
              
               
               UIAnimationController.Instance.AnimateDiamond(diamondHolder.position ,
               ()=> Completed()
                   
               );
               
           }
       }
       else if(stimulus.answer == Answer.Right && !stimulus.wasClickedOn)
       {
           posCounter = 0;
           UIAnimationController.Instance.Animate(true,fishHolder.position,posCounter,stimulus.wasClickedOn);
           
       }
       else if(stimulus.answer == Answer.Wrong && stimulus.wasClickedOn)
       {
           sharkCounter ++;
           UIAnimationController.Instance.Animate(false,sharkHolder.position);
       }
    }
    public void UpdateText()
    {
        if(fishCounterText)
        fishCounterText.text = fishCounter.ToString();
        if(sharkCounterText)
        sharkCounterText.text = sharkCounter.ToString();
    }
    [SerializeField]
    int maxCombo;
    [SerializeField]
    TMPro.TMP_Text fishCounterText,sharkCounterText,diamondCounterText;
    int posCounter,negCounter,sharkCounter,fishCounter,comboCounter,diamondCounter;
    [SerializeField]
    ServerLevelData serverLevelData;
    [SerializeField]
    ServerSideData serverSideData;
    public void SaveThisLevelData()
    {
        // errors are a per-block measure: without this, false alarms from earlier levels
        // triggered the training block's 3-error mini-tutorial on the first mistake.
        negCounter = 0;
        serverSideData.serverLevelDatas.Add(serverLevelData);
        serverLevelData = new ServerLevelData();
        serverLevelData.stimulusData = new List<StimulusData>();
        serverLevelData.stimiulusGeneralData = new List<string>();
    }
    void OnSettingMenuClosed()
    {
        var isActive = SettingsManager.Instance.useDiamondPrize;
        diamondHolder.gameObject.SetActive(isActive);
    }
    public override void OnDestroy() 
    {
        base.OnDestroy();
        // Never resurrect the settings singleton during teardown just to unsubscribe.
        var settings = SettingsManager.Instance;
        if (settings != null) settings.settingPageClosed -= OnSettingMenuClosed;
    }
    public BubbleController bubbleController;
    void StringToFloat()
    {
        string stringValue = "12.3";
        float value = 0;
        float.TryParse(stringValue,out value);
    }
    public void QuitGame() => Application.Quit();

    #endregion
}

