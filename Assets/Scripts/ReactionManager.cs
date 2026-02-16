using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class ReactionManager : GenericSingleton<ReactionManager>
{
    [Header("Camera Effects")]
    [SerializeField] float magn;
    [SerializeField] float rough;
    [SerializeField] float fadeIn;
    [SerializeField] float fadeOut;
    
    [Header("UI References")]
    [SerializeField] TMPro.TMP_Text fishCounterText;
    [SerializeField] TMPro.TMP_Text sharkCounterText;
    [SerializeField] TMPro.TMP_Text diamondCounterText;
    [SerializeField] Transform fishHolder;
    [SerializeField] Transform sharkHolder;
    [SerializeField] Transform diamondHolder;
    [SerializeField] AudioSource diamondSFX;
    public ProgressBar progressBar;
    public BubbleController bubbleController;
    
    [Header("Combo Settings")]
    [SerializeField] int maxCombo;
    [SerializeField] float oxygenTankDurationSeconds = 900f; // 15 minutes default
    [SerializeField] TimerEndBehavior timerEndBehavior = TimerEndBehavior.Nothing;
    
    [Header("Data Management")]
    [SerializeField] ServerLevelData serverLevelData;
    [SerializeField] ServerSideData serverSideData;
    [SerializeField] GyroData data;
    
    // Counters
    private int posCounter;
    private int negCounter;
    private int sharkCounter;
    private int fishCounter;
    private int diamondCounter;
    private float maxTimer = 1f;
    private float timer;
    private int maxSampleCount = 25;
    private int currentSampleCount;
    private Vector3 gyroData;
    private Vector3 acc;
    
    public int numberOfTriesBeforeTutEnd;

    #region Unity Lifecycle
    public override void Awake()
    {
        base.Awake();
        InitializeData();
        InitializeProgressBar();
        UpdateText();
        Input.gyro.enabled = true;
        SettingsManager.Instance.settingPageClosed += OnSettingMenuClosed;
    }

    private void Update() 
    {
        if (!GameFlowController.Instance.GameIsPlaying) return;
        
        UpdateGyroscopeData();
    }

    public override void OnDestroy() 
    {
        base.OnDestroy();
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.settingPageClosed -= OnSettingMenuClosed;
    }
    #endregion

    #region Initialization
    private void InitializeData()
    {
        data = new GyroData();
        data.ResetData();
        serverLevelData.stimulusData = new List<StimulusData>();
        serverLevelData.stimiulusGeneralData = new List<string>();
    }

    private void InitializeProgressBar()
    {
        Debug.Log($"InitializeProgressBar called - Setting timer to {oxygenTankDurationSeconds} seconds");
        if (progressBar)
            progressBar.SetMaxFill(oxygenTankDurationSeconds);
    }
    #endregion

    #region Camera Effects
    public void Shake()
    {
        CameraShaker.Instance.ShakeOnce(magn, rough, fadeIn, fadeOut);
    }
    #endregion

    #region Data Management
    public void RestServerLevelData()
    {
        serverLevelData.stimulusData = new List<StimulusData>();
    }

    public void RecevieStimulusGeneralData(string stimulusData)
    {
        serverLevelData.stimiulusGeneralData.Add(stimulusData);
    }

    public void RecevieLevelDescription(string levelDescription)
    {
        serverLevelData.levelDescription = levelDescription;
    }

    public void SaveThisLevelData()
    {
        serverSideData.serverLevelDatas.Add(serverLevelData);
        ResetServerLevelData();
    }

    private void ResetServerLevelData()
    {
        serverLevelData = new ServerLevelData();
        serverLevelData.stimulusData = new List<StimulusData>();
        serverLevelData.stimiulusGeneralData = new List<string>();
    }
    #endregion

    #region Input Data Handling
    public void AddInputData(TouchType touchType)
    {
        switch (touchType)
        {
            case TouchType.upperLeft:
                serverLevelData.upperLeft++;
                break;
            case TouchType.middleLeft:
                serverLevelData.middleLeft++;
                break;
            case TouchType.lowerLeft:
                serverLevelData.lowerLeft++;
                break;
            case TouchType.upperMiddle:
                serverLevelData.upperMiddle++;
                break;
            case TouchType.middleMiddle:
                serverLevelData.middleMiddle++;
                break;
            case TouchType.lowerMiddle:
                serverLevelData.lowerMiddle++;
                break;
            case TouchType.upperRight:
                serverLevelData.upperRight++;
                break;
            case TouchType.middleRight:
                serverLevelData.middleRight++;
                break;
            case TouchType.lowerRight:
                serverLevelData.lowerRight++;
                break;
        }
    }
    #endregion

    #region Gyroscope Data
    private void UpdateGyroscopeData()
    {
        if (timer < maxTimer)
        {
            timer += Time.deltaTime;
            if (currentSampleCount < maxSampleCount)
            {
                gyroData = Input.gyro.userAcceleration;
                acc = Input.acceleration;
                data.acceleration.Add(acc);
                data.gyroscope.Add(gyroData);
                currentSampleCount++;
            }
        }
        else 
        {
            ProcessGyroscopeData();
        }
    }

    private void ProcessGyroscopeData()
    {
        currentSampleCount = 0;
        data.second++;
        CSVBuilder.Instance.GatherGyroscopeData(data);
        timer = 0;
        data.ResetData();
    }
    #endregion

    #region Stimulus Response Handling
    public void RecevieStimulusFocuesdData(StimulusData stimulus)
    {
        if (serverLevelData.stimulusData == null) return;

        PopulateStimulusData(stimulus);
        ProcessStimulusResponse(stimulus);
        AnimateState(stimulus);
    }

    private void PopulateStimulusData(StimulusData stimulus)
    {
        stimulus.tryNumber = GameFlowController.Instance.stateCounter;
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
        ResetGyroscopeCounters();
        serverLevelData.stimulusData.Add(stimulus);
        CSVBuilder.Instance.ToCSV(stimulus);
    }

    private void ResetGyroscopeCounters()
    {
        gyroData = Vector3.zero;
        currentSampleCount = 0;
        acc = Vector3.zero;
    }

    private void ProcessStimulusResponse(StimulusData stimulus)
    {
        bool isCorrectResponse = stimulus.answer == Answer.Right && stimulus.wasClickedOn;
        bool isIncorrectResponse = (stimulus.answer == Answer.Wrong && stimulus.wasClickedOn) || 
                                   (stimulus.answer == Answer.Right && !stimulus.wasClickedOn && GameFlowController.Instance.level.isTraining);

        if (isCorrectResponse)
        {
            if (posCounter < maxCombo)
                posCounter++;
        }
        else if (isIncorrectResponse)
        {
            negCounter++;
            posCounter = 0;
        }

        HandleTrainingMode();
    }

    private void HandleTrainingMode()
    {
        if (negCounter >= 3 && GameFlowController.Instance.level.isTraining)
        {
            negCounter = 0;
            GameFlowController.Instance.StopCR();
        }
    }
    #endregion

    #region Animation and UI Updates
    private void AnimateState(StimulusData stimulus)
    {
        if (stimulus.answer == Answer.Right && stimulus.wasClickedOn)
        {
            HandleCorrectResponse(stimulus);
        }
        else if (stimulus.answer == Answer.Right && !stimulus.wasClickedOn)
        {
            HandleMissedResponse();
        }
        else if (stimulus.answer == Answer.Wrong && stimulus.wasClickedOn)
        {
            HandleIncorrectResponse();
        }
    }

    private void HandleCorrectResponse(StimulusData stimulus)
    {
        fishCounter += SettingsManager.Instance.useCombo ? posCounter * 1 : 1;
        UIAnimationController.Instance.Animate(true, fishHolder.position, posCounter);
        
        if (stimulus.reactionTimer < 0.6f && SettingsManager.Instance.useDiamondPrize)
        {
            UIAnimationController.Instance.AnimateDiamond(diamondHolder.position, OnDiamondCompleted);
        }
    }

    private void HandleMissedResponse()
    {
        posCounter = 0;
        UIAnimationController.Instance.Animate(true, fishHolder.position, posCounter, false);
    }

    private void HandleIncorrectResponse()
    {
        sharkCounter++;
        UIAnimationController.Instance.Animate(false, sharkHolder.position);
    }

    private void OnDiamondCompleted()
    {
        diamondCounter++;
        diamondCounterText.text = diamondCounter.ToString();
        if (SettingsManager.Instance.diamondSFX) 
            diamondSFX.Play();
    }

    public void UpdateText()
    {
        if (fishCounterText != null)
            fishCounterText.text = fishCounter.ToString();
        if (sharkCounterText != null)
            sharkCounterText.text = sharkCounter.ToString();
        if(diamondCounterText != null)
            diamondCounterText.text = diamondCounter.ToString();
    }

    public void ClearCombo()
    {   
        posCounter = 0;
    }
    #endregion

    #region Settings Events
    private void OnSettingMenuClosed()
    {
        bool isActive = SettingsManager.Instance.useDiamondPrize;
        diamondHolder.gameObject.SetActive(isActive);
    }
    #endregion

    #region Utility
    public void HandleTimerEnd()
    {
        Debug.Log($"HandleTimerEnd called with behavior: {timerEndBehavior}");
        switch (timerEndBehavior)
        {
            case TimerEndBehavior.Nothing:
                Debug.Log("Timer end behavior: Nothing - no action taken");
                break;
            case TimerEndBehavior.EndLevel:
                Debug.Log("Timer end behavior: EndLevel - calling ForceAdvanceToNextLevel");
                GameFlowController.Instance.ForceAdvanceToNextLevel();
                break;
        }
    }

    public void QuitGame() => Application.Quit();
    #endregion
}

public enum TimerEndBehavior
{
    Nothing,
    EndLevel
}