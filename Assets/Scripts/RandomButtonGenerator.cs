using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using TMPro;

public class RandomButtonGenerator : GenericSingleton<RandomButtonGenerator>
{
    public bool gameIsRunning
    {
        get => gameIsPlaying && useTank && !needsTutorial;
    }
    public TMP_Text levelDescriptionText;
    public ReactionButton shark,fish,fishA,sharkA;
    [SerializeField]
    bool useRandomValues;
    [SerializeField]
    LevelsData levelsData;
    [Range(0f,10f)]
    public float timeBetweenEachRespawn = 2f;
    [SerializeField]
    int [] translatedStateData; 
    public int currentLevel,stateCounter;
    public float visualTimer,audioTimer;
    public Bubble bubble;
    public Level level;
    [SerializeField]
    GameObject startGameButton,gameplay;
    [SerializeField] GameObject videoplayer;
    [SerializeField] float videoPrepareTimeout = 10f;
    [SerializeField] float videoOverrunSlack = 3f;
    bool videoEnded,videoFailed,videoRunning,videoSkipped;

    // Hooked to the Skip button on the video canvas.
    public void SkipVideo()
    {
        if(!videoRunning) return;
        videoSkipped = true;
    }

    IEnumerator WaitForVideo()
    {
        videoplayer.SetActive(true);
        var player = videoplayer.GetComponentInChildren<VideoPlayer>(true);
        if(player == null)
        {
            StartGame();
            yield break;
        }

        // Play-on-awake starts the clip the instant the object is enabled, before the
        // decoder has the first frame ready — that is what leaves it stuck on black.
        // Prepare first, then play, and end on the clip's own event instead of a fixed wait.
        player.playOnAwake = false;
        player.Stop();
        videoEnded = false;
        videoFailed = false;
        videoSkipped = false;
        player.errorReceived += OnVideoError;
        player.loopPointReached += OnVideoEnded;

        player.Prepare();
        var prepareDeadline = Time.realtimeSinceStartup + videoPrepareTimeout;
        while(!player.isPrepared && !videoFailed && !videoSkipped && Time.realtimeSinceStartup < prepareDeadline)
            yield return null;

        if(player.isPrepared && !videoFailed && !videoSkipped)
        {
            player.Play();
            // Never trap the player on a stalled decoder: give the clip its own length
            // plus a little slack, then carry on regardless.
            var playDeadline = Time.realtimeSinceStartup + (float)player.length + videoOverrunSlack;
            while(!videoEnded && !videoFailed && !videoSkipped && Time.realtimeSinceStartup < playDeadline)
                yield return null;
        }
        else
        {
            ScreenDebug.Instance?.Debug("Video was not ready, skipping to the game");
        }

        player.errorReceived -= OnVideoError;
        player.loopPointReached -= OnVideoEnded;
        player.Stop();
        videoRunning = false;
        StartGame();
    }

    void OnVideoEnded(VideoPlayer source)
    {
        videoEnded = true;
    }

    void OnVideoError(VideoPlayer source, string message)
    {
        videoFailed = true;
        ScreenDebug.Instance?.Debug("Video error: " + message);
    }
    void Start()
    {
        starButtonSFX.playOnAwake = false;
        gameplay.SetActive(false);
        bubble.gameObject.SetActive(false);
        image = bubble.GetComponent<UnityEngine.UI.Image>();
        defualtBubbleSprite = image.sprite;

    }
    UnityEngine.UI.Image image;
    public void StartVideoGame()
    {
        if(videoRunning) return;
        videoRunning = true;
        StartCoroutine(WaitForVideo());
    }
    public void StartGame()
    {
        videoplayer.SetActive(false);
        ReactionManager.Instance.RestServerLevelData();
        ReadLevel(false);
        gameIsPlaying = true;
        startGameButton.SetActive(false);
        gameplay.SetActive(true);
        if(SettingsManager.Instance.useCustomTimer)
        {
            visualTimer = SettingsManager.Instance.visualTimer;
            audioTimer = SettingsManager.Instance.audioTimer;
        }
        if(firstTutorialDelay > 0f)
            Invoke("RandomizeButtons",firstTutorialDelay);
        else
            RandomizeButtons();
    }
    // Both of these were hard-coded: 2s here and 3s in GetTutorial, so the first
    // voice-over panel sat on a blank screen for five seconds and every later level's
    // panel for three. Serialized and zero by default; raise them if a pause is wanted.
    [SerializeField] float firstTutorialDelay = 0f;
    [SerializeField] float tutorialPanelDelay = 0f;
    bool gameIsPlaying , useTank;
    public bool GameIsPlaying => gameIsPlaying;
    void RandomizeButtons()
    {
        GetTutorial();
    }
    bool showed2ndTut;
    public int numberOfTriesBeforeTutEnd;
    public void OnStartMission()
    {
        if(tutorialAudioSource.isPlaying)
        {
            numberOfTriesBeforeTutEnd ++;
            return;
        }
        starButtonSFX.Play();
        if(levelsData.levels[currentLevel].hasSecondTutorial && !showed2ndTut)
        {
            SetUpSecondTutorial();
            showed2ndTut = true;
        }
        else 
        {
            ReactionManager.Instance.numberOfTriesBeforeTutEnd = numberOfTriesBeforeTutEnd;
            numberOfTriesBeforeTutEnd = 0;
            ReactionManager.Instance.progressBar.SliderAnimationState(false);
            ReadLevel(false);
            tutorialPanel.SetActive(false);
            tutorialAudioSource.Stop();
            needsTutorial = false;
            Invoke("StartWiths",2f);
        }
    }
    void StartWiths()
    {
        StartCoroutine("RandomizeButtonsCR");

    }
    public void StopCR()
    {
        StopAllCoroutines();
        SetUpMiniTutorial();
    }
    Sprite defualtBubbleSprite;
    public void GetTutorial(bool wait = false)
    {
        if(tutorialPanelDelay > 0f)
            Invoke("SetUpTutorial",tutorialPanelDelay);
        else
            SetUpTutorial();
    }

    // True while any of the three tutorial panels is on screen. Read by the
    // editor-only TutorialSkipButton, which cannot see the panel from where it lives.
    public bool TutorialPanelShowing => tutorialPanel != null && tutorialPanel.activeInHierarchy;

    // Hooked to the editor-only skip button on the tutorial panel. OnStartMission
    // refuses to advance while the voice-over is still playing - that is what stops a
    // child skipping ahead by tapping - so silence it first and then take the same path
    // a real tap takes, second tutorial and all.
    public void SkipTutorial()
    {
        if(!TutorialPanelShowing) return;
        // One press has to clear the whole tutorial, not just its first page. Level 1
        // sets hasSecondTutorial, so OnStartMission answers the first press by opening
        // the second panel - another scroll with another voice-over, which looks exactly
        // like the button doing nothing. Mark it shown so we take the exit branch.
        showed2ndTut = true;
        tutorialAudioSource.Stop();
        OnStartMission();
    }
    [SerializeField] AudioSource starButtonSFX;
    void SetUpMiniTutorial()
    {
        tutorialAudioSource.clip = levelsData.levels[currentLevel].miniTutorialClip;
        tutorialImage.sprite = levelsData.levels[currentLevel].miniTutorialImage;
        if(!image)
        image = bubble.GetComponent<UnityEngine.UI.Image>();
        if(levelsData.levels[currentLevel].bubbleImage)
            image.sprite = levelsData.levels[currentLevel].bubbleImage;
        else 
            image.sprite = defualtBubbleSprite;
        tutorialPanel.SetActive(true);
        tutorialAudioSource.Play();
    }
    void SetUpTutorial()
    {
        ReactionManager.Instance.progressBar.SliderAnimationState(true);
        UIAnimationController.Instance.ClearCombo();
        ReactionManager.Instance.ClearCombo();
        tutorialAudioSource.clip = levelsData.levels[currentLevel].tutorialClip;
        tutorialImage.sprite = levelsData.levels[currentLevel].tutorialImage;
        if(!image)
        image = bubble.GetComponent<UnityEngine.UI.Image>();
        if(levelsData.levels[currentLevel].bubbleImage)
            image.sprite = levelsData.levels[currentLevel].bubbleImage;
        else 
            image.sprite = defualtBubbleSprite;
        tutorialPanel.SetActive(true);
        tutorialAudioSource.Play();
    }
    void SetUpSecondTutorial()
    {
        tutorialAudioSource.clip = levelsData.levels[currentLevel].tutorialClip2;
        tutorialImage.sprite = levelsData.levels[currentLevel].tutorialImage2;
        if(!image)
        image = bubble.GetComponent<UnityEngine.UI.Image>();
        if(levelsData.levels[currentLevel].bubbleImage)
            image.sprite = levelsData.levels[currentLevel].bubbleImage;
        else 
            image.sprite = defualtBubbleSprite;
        tutorialPanel.SetActive(true);
        tutorialAudioSource.Play();
    }
    [SerializeField]
    AudioSource tutorialAudioSource;
    [SerializeField]

    UnityEngine.UI.Image tutorialImage;
    [SerializeField]
    GameObject tutorialPanel,endPanel;
    void ReadLevel(bool waitForTutorial = true)
    {
        if(currentLevel >= levelsData.levels.Length) return;
        if(levelsData.levels[currentLevel].useTutorial && waitForTutorial)
        {
            needsTutorial = true;
            GetTutorial(true);

            return;
        } 
        level = levelsData.levels[currentLevel];
        levelDescriptionText.text = level.levelDescription;
        SaveLevelDescription(level.levelDescription);
        visualTimer = level.visualTimer;
        audioTimer = level.audioTimer;
        timeBetweenEachRespawn = level.timeBetweenStimulus;
        translatedStateData = TranslateStateDataToState(level.states);
        SaveLevelDataVariables(level.states);
        // useTank used to latch true forever once any level set it, leaving the oxygen
        // bar running through levels that declare useTank: 0.
        useTank = level.useTank;
        showed2ndTut = false;
        // The tank belongs to the levels that ask for it. It used to be switched on
        // and never switched off, so it sat on screen - frozen, then empty - for every
        // level after the one that used it.
        if(level.useTank)
            ReactionManager.Instance.progressBar.Activate();
        else
            ReactionManager.Instance.progressBar.Deactivate();
        
        
        if(!level.useTutorial || !waitForTutorial)
        {
            needsTutorial = false;
        }
        else if(level.useTutorial && gameIsPlaying && waitForTutorial)
        {
            needsTutorial = true;
            GetTutorial(true);
        }

    }
    void RandomBehaviourState()
    {
        int random = Random.Range(0,3);
        State(random);
    }
    void BehaviourState(int state)
    {
        if(currentLevel >= levelsData.levels.Length) return;
        var size = levelsData.levels[currentLevel].states.Length;
        if(!needsTutorial && stateCounter < levelsData.levels[currentLevel].states.Length)
        {
            state = translatedStateData[stateCounter];
            State(state);
            stateCounter ++;

        }
        // The level used to be closed out right here — at the LAST trial's ONSET — so
        // that trial's response landed in the next level's bucket, exported Trial = 0
        // and sometimes an empty description. Defer it until the response is recorded.
        if(stateCounter >= levelsData.levels[currentLevel].states.Length)
            levelBoundaryPending = true;
    }
    bool levelBoundaryPending;

    // Called once the trial's response has been written (bubble deactivated).
    void AdvanceLevelIfPending()
    {
        if(!levelBoundaryPending) return;
        levelBoundaryPending = false;
        currentLevel ++;
        ReactionManager.Instance.SaveThisLevelData();
        ReadLevel();
        stateCounter = 0;
        // goes to next level
    }
    void DisableButtons()
    {
        shark.gameObject.SetActive(false);
        fish.gameObject.SetActive(false);
        fishA.gameObject.SetActive(false);
        sharkA.gameObject.SetActive(false);
    }
    void State(int state)
    {
        switch (state)
        {
            case 0:
                fish.gameObject.SetActive(true);
                shark.gameObject.SetActive(false);
            break;
            case 1:
                fish.gameObject.SetActive(false);
                shark.gameObject.SetActive(true);

            break;
            case 2:
                shark.gameObject.SetActive(false);
                fish.gameObject.SetActive(false);
                fishA.gameObject.SetActive(true);
                sharkA.gameObject.SetActive(false);
            break;
            case 3:
                shark.gameObject.SetActive(false);
                fish.gameObject.SetActive(false);
                sharkA.gameObject.SetActive(true);
                fishA.gameObject.SetActive(false);
            break;
        }
    }
    public float thisTryTimer;
    bool needsTutorial;
    IEnumerator RandomizeButtonsCR()
    {
        StartCoroutine("StartTest");
        yield return null; 
    }
    // ---- phone-call / audio-focus interruption ----
    bool trialInterrupted;
    public float TrialStartTime { get; private set; }

    // The oxygen bar was hard-coded to 804 seconds while the shipped asset totals far
    // less, so it could never empty in step with the test. Derive it from the data.
    //
    // Only the levels that declare useTank count: the bar is shown and animated for
    // those levels alone, so sizing it to the whole session would leave it barely
    // touched by the time the tank disappears again.
    public float TankSeconds()
    {
        var total = 0f;
        if(levelsData == null || levelsData.levels == null) return 1f;
        foreach(var lvl in levelsData.levels)
        {
            if(!lvl.useTank || lvl.states == null) continue;
            foreach(var s in lvl.states)
                total += (s.FA || s.SA) ? lvl.otherTime : lvl.timeBetweenStimulus;
        }
        return total > 0f ? total : 1f;
    }

    IEnumerator HoldWhilePaused()
    {
        while(CallInterruptionGuard.Paused) yield return null;
    }

    // Realtime wait that bails out the moment the OS takes our audio, so an audio
    // stimulus is never left running where the child cannot hear it.
    IEnumerator TrialWait(float seconds)
    {
        var end = Time.realtimeSinceStartup + seconds;
        while(Time.realtimeSinceStartup < end)
        {
            if(CallInterruptionGuard.Paused)
            {
                trialInterrupted = true;
                yield break;
            }
            yield return null;
        }
    }

    IEnumerator StartTest()
    {
        // never open a trial while the audio belongs to a call
        yield return HoldWhilePaused();
        trialInterrupted = false;
        TrialStartTime = Time.realtimeSinceStartup;

        // Capture THIS trial's modality before BehaviourState advances stateCounter.
        // Reading it afterwards picked up the NEXT trial's type, so an audio trial
        // following a visual one (and vice versa) got a response window 400ms wrong.
        var isAudio = translatedStateData[stateCounter] == 2 || translatedStateData[stateCounter] == 3;
        var window = isAudio ? level.otherTime : level.timeBetweenStimulus;
        thisTryTimer = window;

        bubble.gameObject.SetActive(true);
        // activates stimulus
        BehaviourState(stateCounter);
        if(needsTutorial) yield return null;

        yield return TrialWait(window/2);
        // deactivates stimulus

        DisableButtons();
        // waits to deactive bubble

        if(!trialInterrupted) yield return TrialWait(window/2);
        // deactives bubble

        bubble.gameObject.SetActive(false);
        // the response for this trial is now recorded, so it is safe to close the level
        AdvanceLevelIfPending();

        // A call took the audio during this trial. The row has already been written and
        // flagged Interrupted, so do NOT re-present it: rolling stateCounter back cannot
        // restore the level state BehaviourState may have advanced, and doing so skipped
        // whole levels. Wait for the operator, then carry on with the next trial.
        if(trialInterrupted)
        {
            yield return HoldWhilePaused();
            yield return new WaitForSecondsRealtime(1f);
            trialInterrupted = false;
        }

        // waits to start process bubble
        yield return new WaitForSeconds(.1f);
        if(currentLevel >= levelsData.levels.Length)
        {
            Debug.Log("End");
            yield return EndProcess();
             
        }
        // if can proceed starts if not it stops
        else if(!needsTutorial && stateCounter < levelsData.levels[currentLevel].states.Length && currentLevel < levelsData.levels.Length)
        {
            StartCoroutine("StartTest");
        }

    }
    [SerializeField]
    AudioSource endAudio;
    IEnumerator EndProcess()
    {
        UIAnimationController.Instance.ClearCombo();
        endAudio.Play();
        // Wait for the thank-you voice-over to actually finish rather than a hard-coded
        // 6s guess, so the medal/star panel and its crowd-cheering clip land on its end.
        var wait = 6f;
        if(endAudio != null && endAudio.clip != null)
            wait = endAudio.clip.length;
        yield return new WaitForSeconds(wait);
        endPanel.SetActive(true);
    }
    int [] TranslateStateDataToState(StateData [] stateDatas)
    {
        List<int> tempStateData = new List<int>();
        for (int i = 0; i < stateDatas.Length; i++)
        {
           
            if(stateDatas[i].FV)
                tempStateData.Add(0);
            else if(stateDatas[i].SV)
                tempStateData.Add(1);
            else if(stateDatas[i].FA)
                tempStateData.Add(2);
            else if(stateDatas[i].SA)
                tempStateData.Add(3);
            else 
            {
                // Used to `break`, returning a SHORTER array than states.Length while every
                // bounds check still used states.Length — so one bad entry meant an
                // IndexOutOfRangeException mid-session and a dead trial loop. Keep the
                // array aligned and make the bad entry loud instead.
                Debug.LogError("Corrupted Level Data Check Out State Data at " + i + " Of this Level");
                tempStateData.Add(0);
            }
                
        }
        return tempStateData.ToArray();
    }
    void SaveLevelDataVariables(StateData [] stateDatas)
    {
        List<int> useVisualFish = new List<int>();
        List<int> useVisualShark = new List<int>();
        List<int> useAudioFish = new List<int>();
        List<int> useAudioShark = new List<int>();
        List<int> useBothFish = new List<int>();
        List<int> useBothShark = new List<int>();
        for (int i = 0; i < stateDatas.Length; i++)
        {
            if(stateDatas[i].FV)
                useVisualFish.Add(0);
            else if(stateDatas[i].SV)
                useVisualShark.Add(1);
            else if(stateDatas[i].FA)
                useAudioFish.Add(2);
            else if(stateDatas[i].SA)
                useAudioShark.Add(3);
            else 
            {
                Debug.LogError("Corrupted Level Data Check Out State Data at " + i + "Of this Level");
                break;
            } 
        }
        SaveThisStimulusData("useVisualFish",useVisualFish.ToArray());
        SaveThisStimulusData("useVisualShark",useVisualShark.ToArray());
        SaveThisStimulusData("useAudioFish",useAudioFish.ToArray());
        SaveThisStimulusData("useAudioShark",useAudioShark.ToArray());
        SaveThisStimulusData("useBothFish",useBothFish.ToArray());
        SaveThisStimulusData("useBothShark",useBothShark.ToArray());
    }

    public void SaveThisStimulusData(string stimulusDescription,int [] stimulusData)
    {
        string data = "This Level Has "  + stimulusData.Length + " of type " + stimulusDescription;
        ReactionManager.Instance.RecevieStimulusGeneralData(data);
    }
    public void SaveLevelDescription(string levelDescription)
    {
        ReactionManager.Instance.RecevieLevelDescription(levelDescription);

    }
}
public enum GameStates
{
    WarmUp,CoreGamePlay,CoolDown
}