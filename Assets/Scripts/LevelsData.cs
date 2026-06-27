using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "LevelsData", menuName = "Reaction/LevelsData", order = 0)]
public class LevelsData : ScriptableObject 
{
    public Level [] levels;
}
[System.Serializable]
public struct Level
{
    public float visualTimer,audioTimer,timeBetweenStimulus,otherTime;
    public StateData [] states;
    public string levelDescription;
    public bool useDifferentSpawnTime,useTutorial,isTraining,hasSecondTutorial;
    public AudioClip tutorialClip,miniTutorialClip,tutorialClip2;
    public Sprite tutorialImage,tutorialImage2,miniTutorialImage,bubbleImage;
    public bool useTank;
}
[System.Serializable]
public struct StateData
{
    public bool FV,FA,SV,SA;
}
[System.Serializable]
public struct ServerSideData
{
    public List <ServerLevelData> serverLevelDatas;
    public string firstName,lastName;
}
[System.Serializable]
public struct ServerLevelData
{
    public string levelDescription;
    public int totalStimulusTapped,rightStimulus,wrongStimulus;
    public List<StimulusData> stimulusData;
    public List<string> stimiulusGeneralData;
    public int upperLeft,middleLeft,lowerLeft,upperMiddle,middleMiddle,lowerMiddle,upperRight,middleRight,lowerRight;

}
[System.Serializable]
public struct StimulusData
{
    public StateData stateData;
    public Answer answer;
    public float reactionTimer,startingTimer;
    public bool wasClickedOn;
    public int tryNumber;
    public string levelDescription;
    public int upperLeft,middleLeft,lowerLeft,upperMiddle,middleMiddle,lowerMiddle,upperRight,middleRight,lowerRight;
    public Vector3 gyroScopeData;

}
[SerializeField]
public enum StimulusType
{
    Visual,Auditory
}
