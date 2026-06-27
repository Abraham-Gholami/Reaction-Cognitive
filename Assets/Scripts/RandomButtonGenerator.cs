using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    void Start()
    {
        gameplay.SetActive(false);
        bubble.gameObject.SetActive(false);
        image = bubble.GetComponent<UnityEngine.UI.Image>();
        defualtBubbleSprite = image.sprite;

    }
    UnityEngine.UI.Image image;

    public void StartGame()
    {
        ReadLevel(false);
        gameIsPlaying = true;
        startGameButton.SetActive(false);
        gameplay.SetActive(true);
        if(SettingsManager.Instance.useCustomTimer)
        {
            visualTimer = SettingsManager.Instance.visualTimer;
            audioTimer = SettingsManager.Instance.audioTimer;
        }
        Invoke("RandomizeButtons",2f);
    }
    bool gameIsPlaying , useTank;
    void RandomizeButtons()
    {
        GetTutorial();
    }
    public void OnStartMission()
    {
        if(tutorialAudioSource.isPlaying) return;
        if(levelsData.levels[currentLevel].hasSecondTutorial)
        {
            SetUpSecondTutorial();
            levelsData.levels[currentLevel].hasSecondTutorial = false;
        }
        else 
        {
            
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
        Invoke("SetUpTutorial",3f);
    }
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
        Debug.Log(1);
        Debug.Log(waitForTutorial);
        if(levelsData.levels[currentLevel].useTutorial && waitForTutorial)
        {
            needsTutorial = true;
            GetTutorial(true);

            return;
        } 
        if(currentLevel >= levelsData.levels.Length) return;
        Debug.Log(2);
        level = levelsData.levels[currentLevel];
        levelDescriptionText.text = level.levelDescription;
        SaveLevelDescription(level.levelDescription);
        visualTimer = level.visualTimer;
        audioTimer = level.audioTimer;
        timeBetweenEachRespawn = level.timeBetweenStimulus;
        translatedStateData = TranslateStateDataToState(level.states);
        SaveLevelDataVariables(level.states);
        if(level.useTank)
        {
            useTank = true;
            ReactionManager.Instance.progressBar.Activate();
        }
        
        
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
        if(stateCounter >= levelsData.levels[currentLevel].states.Length)
        {
            currentLevel ++;
            ReactionManager.Instance.SaveThisLevelData();
            ReadLevel();
            stateCounter = 0;
            //state = translatedStateData[stateCounter];
            // goes to next level

        }
        
        

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
    IEnumerator StartTest()
    {
        // starts the test 
        bubble.gameObject.SetActive(true);
        // activates stimulus
        BehaviourState(stateCounter);
        if(needsTutorial) yield return null;
        if(translatedStateData[stateCounter] == 2 || translatedStateData[stateCounter] == 3)
        {
            thisTryTimer = level.otherTime;
            yield return new WaitForSecondsRealtime(level.otherTime/2);
        }
        else 
        {
            yield return new WaitForSecondsRealtime(level.timeBetweenStimulus/2);
            thisTryTimer = level.timeBetweenStimulus;
        }
        // deactivates stimulus

        DisableButtons();
        // waits to deactive bubble

        if(translatedStateData[stateCounter] == 2 || translatedStateData[stateCounter] == 3)
        {
            thisTryTimer = level.otherTime;
            yield return new WaitForSecondsRealtime(level.otherTime/2);
        }
        else 
        {
            yield return new WaitForSecondsRealtime(level.timeBetweenStimulus/2);
            thisTryTimer = level.timeBetweenStimulus;
        }
        // deactives bubble

        bubble.gameObject.SetActive(false);
        // waits to start process bubble
        yield return new WaitForSeconds(.1f);
        // if can proceed starts if not it stops
        if(!needsTutorial && stateCounter < levelsData.levels[currentLevel].states.Length && currentLevel < levelsData.levels.Length)
        {
            StartCoroutine("StartTest");
        }
        else if(currentLevel >= levelsData.levels.Length)
        {
            Debug.Log("End");
            endPanel.SetActive(true);
            yield return null; 
        }
        
        

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
                Debug.LogError("Corrupted Level Data Check Out State Data at " + i + "Of this Level");
                break;
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