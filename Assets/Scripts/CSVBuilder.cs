using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using System.Globalization;
using System.IO;
using UnityEngine.Networking;

public class CSVBuilder : GenericSingleton<CSVBuilder>
{
    StringBuilder sb = new StringBuilder("Type,Mode,StartingTime,ReactionTime,Response,Trial,upperLeft,middleLeft,lowerLeft,upperMiddle,middleMiddle,lowerMiddle,upperRight,middleRight,lowerRight,starTaps,Interrupted");
    [TextArea(40,100)]
    [SerializeField]
    string allData;
    string gyroDataString;
    string space = "  ,  ";
    // Every number goes through this. StringBuilder.Append(float) formats with the OS
    // culture, so on a Persian (fa-IR) tablet a reaction time of 0.53 was written as
    // "0٫53" (U+066B) and no analysis tool could parse the export.
    static string N(float v) { return v.ToString(CultureInfo.InvariantCulture); }
    static string N(int v) { return v.ToString(CultureInfo.InvariantCulture); }

    public void ToCSV(StimulusData data)
    {

        sb.Append('\n').Append(data.levelDescription).Append(space).Append(GetType(data.stateData)).Append(space)
        .Append(N(timerType)).Append(space).Append(N(data.reactionTimer)).Append(space).Append(GetResponse(data)).Append(space)
        .Append(N(data.tryNumber)).Append(space).Append(N(data.upperLeft)).Append(space).Append(N(data.middleLeft))
        .Append(space).Append(N(data.lowerLeft)).Append(space).Append(N(data.upperMiddle)).Append(space).Append(N(data.middleMiddle)).Append(space)
        .Append(N(data.lowerMiddle)).Append(space).Append(N(data.upperRight)).Append(space).Append(N(data.middleRight)).Append(space).Append(N(data.lowerRight)).Append(space).Append(N(data.starTaps))
        .Append(space).Append(data.interrupted ? "1" : "0");
        allData = sb.ToString();
        AutoSave();
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
    [SerializeField] GameObject panel,completed,failed;
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

        

        string file;
        try
        {
            using(var writer = new StreamWriter(filePath, false))
            {
                await writer.WriteAsync(content);
            }
            file = System.IO.File.ReadAllText(filePath);
        }
        catch (Exception e)
        {
            // async void swallows exceptions, so a disk error used to leave the button
            // looking simply dead. Surface it and let the operator out.
            Debug.LogError("Export write failed: " + e.Message);
            ScreenDebug.Instance?.Debug("Export write failed: " + e.Message);
            SessionGuard.AllowQuit = true;
            panel.SetActive(true);
            failed.SetActive(true);
            return;
        }
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

        string file;
        try
        {
            using(var writer = new StreamWriter(filePath, false))
            {
                await writer.WriteAsync(content);
            }
            file = System.IO.File.ReadAllText(filePath);
        }
        catch (Exception e)
        {
            Debug.LogError("Gyro write failed: " + e.Message);
            ScreenDebug.Instance?.Debug("Gyro write failed: " + e.Message);
            SessionGuard.AllowQuit = true;
            failed.SetActive(true);
            return;
        }
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
        // The test is over either way, so the operator is allowed to leave the app now.
        SessionGuard.AllowQuit = true;
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
        {
            // Upload failed: the run is still on disk (export_autosave.txt /
            // gyro_autosave.txt in persistentDataPath) and can be recovered by hand.
            SessionGuard.AllowQuit = true;
            failed.SetActive(true);
        }

    }
    public void GatherGyroscopeData(GyroData data)
    {
        if(data == null || data.gyroscope == null || data.acceleration == null) return;
        // This used to loop a hard-coded 25 over lists that gain one entry per frame.
        // A single dropped frame made it throw from ReactionManager.Update BEFORE the
        // timer reset, so it then threw every frame for the rest of the session and the
        // gyro export froze. Write whatever we actually captured instead.
        var count = Mathf.Min(data.gyroscope.Count, data.acceleration.Count);
        for (int i = 0; i < count; i++)
        {
            stringBuilder.Append('\n').Append(N(data.second)).Append(space).Append(N(i + 1)).Append(space).Append(N(data.gyroscope[i].x)).Append(space).Append(N(data.gyroscope[i].y)).
            Append(space).Append(N(data.gyroscope[i].z)).Append(space).Append(N(data.acceleration[i].x)).Append(space).Append(N(data.acceleration[i].y)).Append(space).Append(N(data.acceleration[i].z));
        }
        gyroDataString = stringBuilder.ToString();
    }

    // Written after every trial so a crash, a flat battery or the Android back button
    // costs at most the current trial instead of the entire session. The end-of-test
    // upload still runs; this is purely a safety net on disk.
    string autoSavePath;
    void AutoSave()
    {
        try
        {
            if(string.IsNullOrEmpty(autoSavePath))
                autoSavePath = Path.Combine(Application.persistentDataPath, "export_autosave.txt");
            File.WriteAllText(autoSavePath, allData);
        }
        catch (Exception e)
        {
            Debug.LogWarning("CSV autosave failed: " + e.Message);
        }
    }

    // Companion autosave for the gyro stream, flushed once per second.
    public void AutoSaveGyro()
    {
        try
        {
            File.WriteAllText(Path.Combine(Application.persistentDataPath, "gyro_autosave.txt"), gyroDataString);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Gyro autosave failed: " + e.Message);
        }
    }
    StringBuilder stringBuilder = new StringBuilder("Second,DataCounter,GSD_X,GSD_Y,GSD_Z,Acc_X,Acc_Y,Acc_Z");

}
public class User 
{
    public string userid,appId,userName,firstName,lastName,password;
}
public class Data 
{
    public string userid,appId,location,rawdata,fileName;
    public byte [] file;
}