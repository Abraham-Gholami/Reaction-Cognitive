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
    public void ResetCounter()
    {
        upperLeft = 0;
        middleLeft = 0;
        lowerLeft = 0;
        upperMiddle = 0;
        middleMiddle = 0;
        lowerMiddle = 0;
        upperRight = 0;
        middleRight = 0;
        lowerRight = 0;
    }

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
    public int upperLeft,middleLeft,lowerLeft,upperMiddle,middleMiddle,lowerMiddle,upperRight,middleRight,lowerRight,starTaps;
    // A phone call took the audio during this trial, so the child may not have heard
    // the stimulus. Exclude these rows when scoring.
    public bool interrupted;
}
[System.Serializable]
public class GyroData
{
    public int second;
    public List<Vector3> acceleration,gyroscope;
    public void ResetData()
    {
        gyroscope = new List<Vector3>();
        acceleration = new List<Vector3>();
    }
}
public enum StimulusType
{
    Visual,Auditory
}
