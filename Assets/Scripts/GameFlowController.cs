using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Video;

public class GameFlowController : GenericSingleton<GameFlowController>
{
    [Header("Developer Settings")]
    [SerializeField] bool skipTutorialsForTesting = false;
    
    [Header("Video Settings")]
    [SerializeField] GameObject videoplayer;
    [SerializeField] bool skipVideo = false;
    
    [Header("Level Configuration")]
    [SerializeField] LevelsData levelsData;
    [SerializeField] bool useRandomValues;
    [SerializeField] int startingLevel = 0;
    [Range(0f, 10f)]
    public float timeBetweenEachRespawn = 2f;
    
    [Header("UI References")]
    public TMP_Text levelDescriptionText;
    [SerializeField] GameObject startGameButton;
    [SerializeField] GameObject gameplay;
    
    [Header("Tutorial Settings")]
    [SerializeField] GameObject tutorialPanel;
    [SerializeField] GameObject endPanel;
    [SerializeField] UnityEngine.UI.Image tutorialImage;
    [SerializeField] AudioSource tutorialAudioSource;
    
    [Header("Reaction Buttons")]
    public ReactionButton shark;
    public ReactionButton fish;
    public ReactionButton fishA;
    public ReactionButton sharkA;
    
    [Header("Game Objects")]
    public Bubble bubble;
    
    [Header("Audio")]
    [SerializeField] AudioSource starButtonSFX;
    [SerializeField] AudioSource endAudio;
    
    // Game state properties
    public bool gameIsRunning => gameIsPlaying && useTank && !needsTutorial;
    public bool GameIsPlaying => gameIsPlaying;
    
    // Private variables
    private bool gameIsPlaying;
    private bool useTank;
    private bool needsTutorial;
    private bool showed2ndTut;
    private UnityEngine.UI.Image image;
    private Sprite defaultBubbleSprite;
    
    // Level progression
    public int currentLevel;
    public int stateCounter;
    public int numberOfTriesBeforeTutEnd;
    public float visualTimer;
    public float audioTimer;
    public float thisTryTimer;
    public Level level;
    
    // State data
    [SerializeField] int[] translatedStateData;

    #region Unity Lifecycle
    void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        starButtonSFX.playOnAwake = false;
        gameplay.SetActive(false);
        bubble.gameObject.SetActive(false);
        
        image = bubble.GetComponent<UnityEngine.UI.Image>();
        defaultBubbleSprite = image.sprite;
        
        currentLevel = Mathf.Clamp(startingLevel, 0, levelsData.levels.Length - 1);
    }
    #endregion

    #region Video Management
    public void StartVideoGame()
    {
        if (skipVideo)
        {
            StartGame();
        }
        else
        {
            StartCoroutine(WaitForVideo());
        }
    }

    private IEnumerator WaitForVideo()
    {
        videoplayer.SetActive(true);
        var videoPlayerComp = videoplayer.GetComponentInChildren<VideoPlayer>();
        yield return new WaitForSeconds((float)videoPlayerComp.clip.length + 1);
        videoplayer.SetActive(false);
        StartGame();
    }
    #endregion

    #region Game Flow Control
    public void StartGame()
    {
        ReactionManager.Instance.RestServerLevelData();
        ReadLevel(false);
        gameIsPlaying = true;
        startGameButton.SetActive(false);
        gameplay.SetActive(true);
        
        ApplyCustomTimerSettings();
        Invoke(nameof(InitializeTutorial), 2f);
    }

    private void ApplyCustomTimerSettings()
    {
        if (SettingsManager.Instance.useCustomTimer)
        {
            visualTimer = SettingsManager.Instance.visualTimer;
            audioTimer = SettingsManager.Instance.audioTimer;
        }
    }

    private void InitializeTutorial()
    {
        GetTutorial();
    }

    public void StopCR()
    {
        StopAllCoroutines();
        SetUpMiniTutorial();
    }
    #endregion

    #region Tutorial Management
    public void GetTutorial(bool wait = false)
    {
        if (skipTutorialsForTesting)
        {
            // Skip tutorial completely and start mission automatically
            needsTutorial = false;
            Invoke(nameof(AutoStartMissionForTesting), 0.1f);
            return;
        }
        
        Invoke(nameof(SetUpTutorial), 3f);
    }

    private void AutoStartMissionForTesting()
    {
        // Simulate the tutorial completion for testing
        if (ShouldShowSecondTutorial())
        {
            SetUpSecondTutorial();
            showed2ndTut = true;
        }
        else
        {
            StartMissionSequence();
        }
    }

    public void OnStartMission()
    {
        if(tutorialAudioSource.isPlaying)
        {
            numberOfTriesBeforeTutEnd++;
            return;
        }
        
        starButtonSFX.Play();
        
        if (ShouldShowSecondTutorial())
        {
            SetUpSecondTutorial();
            showed2ndTut = true;
        }
        else
        {
            StartMissionSequence();
        }
    }

    private bool ShouldShowSecondTutorial()
    {
        return levelsData.levels[currentLevel].hasSecondTutorial && !showed2ndTut;
    }

    private void StartMissionSequence()
    {
        ReactionManager.Instance.numberOfTriesBeforeTutEnd = numberOfTriesBeforeTutEnd;
        numberOfTriesBeforeTutEnd = 0;
        ReactionManager.Instance.progressBar.SliderAnimationState(false);
        ReadLevel(false);
        tutorialPanel.SetActive(false);
        tutorialAudioSource.Stop();
        needsTutorial = false;
        Invoke(nameof(StartGameplayLoop), 2f);
    }

    private void StartGameplayLoop()
    {
        StartCoroutine(nameof(RandomizeButtonsCR));
    }

    private void SetUpTutorial()
    {
        var currentLevelData = levelsData.levels[currentLevel];
        
        ReactionManager.Instance.progressBar.SliderAnimationState(true);
        UIAnimationController.Instance.ClearCombo();
        ReactionManager.Instance.ClearCombo();
        
        SetupTutorialContent(currentLevelData.tutorialClip, currentLevelData.tutorialImage, currentLevelData.bubbleImage);
    }

    private void SetUpSecondTutorial()
    {
        var currentLevelData = levelsData.levels[currentLevel];
        SetupTutorialContent(currentLevelData.tutorialClip2, currentLevelData.tutorialImage2, currentLevelData.bubbleImage);
    }

    private void SetUpMiniTutorial()
    {
        var currentLevelData = levelsData.levels[currentLevel];
        SetupTutorialContent(currentLevelData.miniTutorialClip, currentLevelData.miniTutorialImage, currentLevelData.bubbleImage);
    }

    private void SetupTutorialContent(AudioClip audioClip, Sprite tutorialSprite, Sprite bubbleSprite)
    {
        tutorialAudioSource.clip = audioClip;
        tutorialImage.sprite = tutorialSprite;
        
        if (!image)
            image = bubble.GetComponent<UnityEngine.UI.Image>();
            
        image.sprite = bubbleSprite ? bubbleSprite : defaultBubbleSprite;
        
        tutorialPanel.SetActive(true);
        tutorialAudioSource.Play();
    }
    #endregion

    #region Level Management
    private void ReadLevel(bool waitForTutorial = true)
    {
        if (currentLevel >= levelsData.levels.Length) return;

        var currentLevelData = levelsData.levels[currentLevel];
        
        if (skipTutorialsForTesting)
        {
            waitForTutorial = false;
        }
        
        if (currentLevelData.useTutorial && waitForTutorial)
        {
            needsTutorial = true;
            GetTutorial(true);
            return;
        }

        LoadLevelData(currentLevelData);
        ConfigureLevelSettings(currentLevelData, waitForTutorial);
    }

    private void LoadLevelData(Level levelData)
    {
        Debug.Log($"LoadLevelData called for level {currentLevel}");
        Debug.Log($"Level description: {levelData.levelDescription}");
        
        level = levelData;
        levelDescriptionText.text = levelData.levelDescription;
        SaveLevelDescription(levelData.levelDescription);
        
        visualTimer = levelData.visualTimer;
        audioTimer = levelData.audioTimer;
        timeBetweenEachRespawn = levelData.timeBetweenStimulus;
        
        translatedStateData = TranslateStateDataToState(levelData.states);
        SaveLevelDataVariables(levelData.states);
        
        Debug.Log($"Level loaded with {levelData.states.Length} states");
    }

    private void ConfigureLevelSettings(Level levelData, bool waitForTutorial)
    {
        useTank = levelData.useTank;
        
        if (useTank)
        {
            ReactionManager.Instance.progressBar.Activate();
        }

        needsTutorial = levelData.useTutorial && waitForTutorial;
        
        if (needsTutorial && gameIsPlaying)
        {
            GetTutorial(true);
        }
    }
    #endregion

    #region State Management
    private void BehaviourState(int state)
    {
        if (currentLevel >= levelsData.levels.Length) return;

        if (!needsTutorial && stateCounter < levelsData.levels[currentLevel].states.Length)
        {
            state = translatedStateData[stateCounter];
            SetGameState(state);
            stateCounter++;
        }

        if (stateCounter >= levelsData.levels[currentLevel].states.Length)
        {
            AdvanceToNextLevel();
        }
    }

    private void AdvanceToNextLevel()
    {
        currentLevel++;
        ReactionManager.Instance.SaveThisLevelData();
        ReadLevel();
        stateCounter = 0;
    }

    public void ForceAdvanceToNextLevel()
    {
        Debug.Log($"ForceAdvanceToNextLevel called - Current level: {currentLevel}");
        
        // Force advance to next level (called by timer end)
        StopAllCoroutines();
        
        Debug.Log("Stopped all coroutines");
        
        AdvanceToNextLevel();
        
        Debug.Log($"Advanced to level: {currentLevel}");
        
        // If we haven't reached the end, start the next level
        if (currentLevel < levelsData.levels.Length)
        {
            Debug.Log("Starting next level gameplay");
            StartCoroutine(nameof(RandomizeButtonsCR));
        }
        else
        {
            Debug.Log("Reached end of all levels - TRIGGERING END PROCESS");
            StartCoroutine(nameof(EndProcess)); // ADD THIS LINE!
        }
    }

    private void SetGameState(int state)
    {
        DisableAllButtons();

        switch (state)
        {
            case 0:
                fish.gameObject.SetActive(true);
                break;
            case 1:
                shark.gameObject.SetActive(true);
                break;
            case 2:
                fishA.gameObject.SetActive(true);
                break;
            case 3:
                sharkA.gameObject.SetActive(true);
                break;
        }
    }

    private void DisableAllButtons()
    {
        shark.gameObject.SetActive(false);
        fish.gameObject.SetActive(false);
        fishA.gameObject.SetActive(false);
        sharkA.gameObject.SetActive(false);
    }
    #endregion

    #region Game Loop Coroutines
    private IEnumerator RandomizeButtonsCR()
    {
        StartCoroutine(nameof(StartTest));
        yield return null;
    }

    private IEnumerator StartTest()
    {
        Debug.Log($"StartTest - Current level: {currentLevel}, Total levels: {levelsData.levels.Length}");
        
        SetupTestTimer();
        bubble.gameObject.SetActive(true);
        
        BehaviourState(stateCounter);
        
        if (needsTutorial) yield break;

        yield return new WaitForSecondsRealtime(thisTryTimer / 2);
        
        DisableAllButtons();
        
        yield return new WaitForSecondsRealtime(thisTryTimer / 2);
        
        bubble.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);

        Debug.Log($"Before end check - Current level: {currentLevel}, Length: {levelsData.levels.Length}");
        
        if (currentLevel >= levelsData.levels.Length)
        {
            Debug.Log("TRIGGERING END PROCESS - Game Complete");
            yield return StartCoroutine(nameof(EndProcess));
        }
        else if (CanContinueTest())
        {
            Debug.Log("Continuing test - next trial");
            StartCoroutine(nameof(StartTest));
        }
        else
        {
            Debug.Log("Cannot continue test - stopping");
        }
    }

    private void SetupTestTimer()
    {
        bool isAudioStimulus = translatedStateData[stateCounter] == 2 || translatedStateData[stateCounter] == 3;
        thisTryTimer = isAudioStimulus ? level.otherTime : level.timeBetweenStimulus;
    }

    private bool CanContinueTest()
    {
        return !needsTutorial && 
               stateCounter < levelsData.levels[currentLevel].states.Length && 
               currentLevel < levelsData.levels.Length;
    }

    private IEnumerator EndProcess()
    {
        Debug.Log("EndProcess started!");
        UIAnimationController.Instance.ClearCombo();
        
        Debug.Log("Playing end audio...");
        endAudio.Play();
        
        // Wait for the actual audio clip length instead of hardcoded 6 seconds
        if (endAudio.clip != null)
        {
            Debug.Log($"Waiting for audio clip to finish - Duration: {endAudio.clip.length} seconds");
            yield return new WaitForSeconds(endAudio.clip.length);
        }
        else
        {
            Debug.LogWarning("No audio clip assigned to endAudio - using default 6 second wait");
            yield return new WaitForSeconds(6);
        }
        
        Debug.Log("Showing end panel!");
        endPanel.SetActive(true);
        Debug.Log("End panel should now be visible");
    }
    #endregion

    #region Data Processing
    private int[] TranslateStateDataToState(StateData[] stateDatas)
    {
        List<int> tempStateData = new List<int>();
        
        for (int i = 0; i < stateDatas.Length; i++)
        {
            if (stateDatas[i].FV)
                tempStateData.Add(0);
            else if (stateDatas[i].SV)
                tempStateData.Add(1);
            else if (stateDatas[i].FA)
                tempStateData.Add(2);
            else if (stateDatas[i].SA)
                tempStateData.Add(3);
            else
            {
                Debug.LogError($"Corrupted Level Data at State Data index {i} for this level");
                break;
            }
        }
        
        return tempStateData.ToArray();
    }

    private void SaveLevelDataVariables(StateData[] stateDatas)
    {
        var stimulusCategories = new Dictionary<string, List<int>>
        {
            {"useVisualFish", new List<int>()},
            {"useVisualShark", new List<int>()},
            {"useAudioFish", new List<int>()},
            {"useAudioShark", new List<int>()},
            {"useBothFish", new List<int>()},
            {"useBothShark", new List<int>()}
        };

        for (int i = 0; i < stateDatas.Length; i++)
        {
            if (stateDatas[i].FV)
                stimulusCategories["useVisualFish"].Add(0);
            else if (stateDatas[i].SV)
                stimulusCategories["useVisualShark"].Add(1);
            else if (stateDatas[i].FA)
                stimulusCategories["useAudioFish"].Add(2);
            else if (stateDatas[i].SA)
                stimulusCategories["useAudioShark"].Add(3);
            else
            {
                Debug.LogError($"Corrupted Level Data at State Data index {i} for this level");
                break;
            }
        }

        foreach (var category in stimulusCategories)
        {
            SaveThisStimulusData(category.Key, category.Value.ToArray());
        }
    }

    private void SaveThisStimulusData(string stimulusDescription, int[] stimulusData)
    {
        string data = $"This Level Has {stimulusData.Length} of type {stimulusDescription}";
        ReactionManager.Instance.RecevieStimulusGeneralData(data);
    }

    private void SaveLevelDescription(string levelDescription)
    {
        ReactionManager.Instance.RecevieLevelDescription(levelDescription);
    }
    #endregion
}

public enum GameStates
{
    WarmUp,
    CoreGamePlay,
    CoolDown
}