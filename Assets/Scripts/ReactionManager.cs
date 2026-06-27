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
    #region Mono
    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        SettingsManager.Instance.settingPageClosed += OnSettingMenuClosed;
        serverLevelData.stimulusData = new List<StimulusData>();
        serverLevelData.stimiulusGeneralData = new List<string>();
        if(progressBar)
            progressBar.SetMaxFill(900);
        UpdateText();
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
    public void RecevieStimulusFocuesdData(StimulusData stimulus)
    {
        if(serverLevelData.stimulusData != null)
        {
            stimulus.tryNumber = RandomButtonGenerator.Instance.stateCounter;
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

            stimulus.gyroScopeData = Input.gyro.userAcceleration;
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
    public void ComboHandler(int comboCounter)
    {

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
    void AnimateState(StimulusData stimulus)
    {
       if(stimulus.answer == Answer.Right && stimulus.wasClickedOn)
       {
           if(!SettingsManager.Instance.useCombo)  fishCounter += 1;
           else fishCounter += posCounter * 1;
           UIAnimationController.Instance.Animate(true,fishHolder.position,posCounter);
           if(stimulus.reactionTimer < 0.6 && SettingsManager.Instance.useDiamondPrize)
           {
               diamondCounter ++;
               diamondCounterText.text = diamondCounter.ToString();
               UIAnimationController.Instance.AnimateDiamond(diamondHolder.position);
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
        SettingsManager.Instance.settingPageClosed -= OnSettingMenuClosed;
    }
    public BubbleController bubbleController;
    void StringToFloat()
    {
        string stringValue = "12.3";
        float value = 0;
        float.TryParse(stringValue,out value);
    }
    

    #endregion
}

