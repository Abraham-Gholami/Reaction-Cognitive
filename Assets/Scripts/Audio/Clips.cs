using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Clips", menuName = "Clip", order = 0)]
public class Clips : ScriptableObject 
{

    public Clip buttonClick,levelCompleted,bubblePopped,levelFailed,negEnforcer,posEnforcer;
    public Clip ReturnRandonmClip(Clip [] clipArray)
    {
        int randomNum = Random.Range(0,clipArray.Length);
        return clipArray[randomNum];
    }
    public Clip gamePlayMusic;
}
[System.Serializable]
public class Clip 
{
    public AudioClip audioClip;
    public bool loop;
}

