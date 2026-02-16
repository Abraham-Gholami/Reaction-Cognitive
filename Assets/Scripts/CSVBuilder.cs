using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using System.IO;
using UnityEngine.Networking;

public class CSVBuilder : GenericSingleton<CSVBuilder>
{
    StringBuilder sb = new StringBuilder("Type,Mode,StartingTime,ReactionTime,Response,Trial,upperLeft,middleLeft,lowerLeft,upperMiddle,middleMiddle,lowerMiddle,upperRight,middleRight,lowerRight,starTaps");
    [TextArea(40,100)]
    [SerializeField]
    string allData;
    string gyroDataString;
    string space = "  ,  ";
    
    public void ToCSV(StimulusData data)
    {
        sb.Append('\n').Append(data.levelDescription).Append(space).Append(GetType(data.stateData)).Append(space)
        .Append(timerType).Append(space).Append(data.reactionTimer).Append(space).Append(GetResponse(data)).Append(space)
        .Append(data.tryNumber).Append(space).Append(data.upperLeft).Append(space).Append(data.middleLeft)
        .Append(space).Append(data.lowerLeft).Append(space).Append(data.upperMiddle).Append(space).Append(data.middleMiddle).Append(space)
        .Append(data.lowerMiddle).Append(space).Append(data.upperRight).Append(space).Append(data.middleRight).Append(space).Append(data.lowerRight).Append(space).Append(data.starTaps);
        allData = sb.ToString();
    }
    
    float timerType;
    
    string GetType(StateData data)
    {
        string type = "";
        if(data.FV)
            type = "Fish Visual";
        else if(data.SV)
           type = "Shark Visual";
        else if(data.FA)
            type = "Fish Auditory";
        else if(data.SA)
            type = "Shark Auditory";
        if(data.SA || data.FA) timerType = 2;
        else if(data.SV || data.FV) timerType = 1.6f;
        return type;
    }
    
    string GetResponse(StimulusData stimulus)
    {
        string response = "";
        if(stimulus.answer == Answer.Right && stimulus.wasClickedOn)
            response = "Hit";
        else if(stimulus.answer == Answer.Wrong && !stimulus.wasClickedOn)
            response = "correct reject";
        else if(stimulus.answer == Answer.Wrong && stimulus.wasClickedOn)
            response = "comission error";
        else if(stimulus.answer == Answer.Right && !stimulus.wasClickedOn)
            response = "omission error";
        return response;
    }
    
    [SerializeField] GameObject panel;
    [SerializeField] GameObject completed;
    [SerializeField] GameObject failed;
    
    public async void SaveToFile ()
    {
        var content = allData;
        var folder = "";
        var filePath = "";
    #if UNITY_EDITOR
        folder = Application.streamingAssetsPath;
        filePath = Path.Combine(folder, "export.txt");
        if(! Directory.Exists(folder)) Directory.CreateDirectory(folder);
        if(Directory.Exists(filePath)) Directory.Delete(filePath);
    #else
        folder = Application.persistentDataPath;
        filePath = Path.Combine(folder, "export.txt");
        if(Directory.Exists(filePath)) Directory.Delete(filePath);
    #endif

        using(var writer = new StreamWriter(filePath, false))
        {
            await writer.WriteAsync(content);
        }
        var file = System.IO.File.ReadAllText(filePath);
        panel.SetActive(true);
        Action<bool> onComplete = new Action<bool>
        (
            (value) => MainDataResult(value)
        ); 
        FileUploader.Instance.UploadFile(file,"export",onComplete);
    #if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
    #endif
    }
    
    public async void SaveGyroToFile ()
    {
        var content = gyroDataString;
        var folder = "";
        var filePath = "";
    #if UNITY_EDITOR
        folder = Application.streamingAssetsPath;
        filePath = Path.Combine(folder, "gyro.txt");
        if(! Directory.Exists(folder)) Directory.CreateDirectory(folder);
        if(Directory.Exists(filePath)) Directory.Delete(filePath);
    #else
        folder = Application.persistentDataPath;
        filePath = Path.Combine(folder, "gyro.txt");
        if(Directory.Exists(filePath)) Directory.Delete(filePath);
    #endif

        using(var writer = new StreamWriter(filePath, false))
        {
            await writer.WriteAsync(content);
        }
        var file = System.IO.File.ReadAllText(filePath);
        panel.SetActive(true);
        Action<bool> onComplete = new Action<bool>
        (
            (value) => ShowResult(value)
        ); 
        ScreenDebug.Instance.Debug("gyro");
        FileUploader.Instance.UploadFile(file,"gyro",onComplete);
    #if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
    #endif
    }
    
    void ShowResult(bool result)
    {
        if(result)
            completed.SetActive(true);
        else
            failed.SetActive(true);
    }
    
    void MainDataResult(bool result)
    {
        if(result)
            SaveGyroToFile ();
        else
            failed.SetActive(true);
    }
    
    public void GatherGyroscopeData(GyroData data)
    {
        // FIXED: Check actual list sizes and use the minimum to prevent index out of range
        int gyroCount = data.gyroscope?.Count ?? 0;
        int accelCount = data.acceleration?.Count ?? 0;
        int maxSamples = Mathf.Min(gyroCount, accelCount, 25); // Use minimum of all three
        
        Debug.Log($"Gathering gyro data - Gyro count: {gyroCount}, Accel count: {accelCount}, Using: {maxSamples} samples");
        
        for (int i = 0; i < maxSamples; i++)
        {
            stringBuilder.Append('\n').Append(data.second).Append(space).Append(i + 1).Append(space)
                .Append(data.gyroscope[i].x).Append(space).Append(data.gyroscope[i].y).Append(space).Append(data.gyroscope[i].z).Append(space)
                .Append(data.acceleration[i].x).Append(space).Append(data.acceleration[i].y).Append(space).Append(data.acceleration[i].z);
        }
        gyroDataString = stringBuilder.ToString();
    }
    
    StringBuilder stringBuilder = new StringBuilder("Second,DataCounter,GSD_X,GSD_Y,GSD_Z,Acc_X,Acc_Y,Acc_Z");
}

public class User 
{
    public string userid;
    public string appId;
    public string userName;
    public string firstName;
    public string lastName;
    public string password;
}

public class Data 
{
    public string userid;
    public string appId;
    public string location;
    public string rawdata;
    public string fileName;
    public byte[] file;
}