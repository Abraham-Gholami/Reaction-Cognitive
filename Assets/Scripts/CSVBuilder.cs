using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using System.IO;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class CSVBuilder : GenericSingleton<CSVBuilder>
{
    string userPost = "http://gamesdata.cognitivetests.ir/Users";
    string appId = "43921cf3-b5ca-4897-a2b9-4ac919e7af77";
    User user = new User();
    public void OnMakeUser()
    {
        StartCoroutine(MakeUser());
    }
    bool signedUp;
    private IEnumerator  MakeUser()
    {
       if(signedUp) yield return null;
        var guid = Guid.NewGuid();
        user.userid = guid.ToString();
        user.appId = appId;
        int random = UnityEngine.Random.Range(1000,110000);
        user.userName = "TestUser123456" + random;
        user.firstName = "Test";
        user.lastName = "User";
        user.password = "12345" + random;
        var jsonObject = JsonConvert.SerializeObject(user);
        DownloadHandlerBuffer downloadHandlerBuffer = new DownloadHandlerBuffer();
        var jsonBinary = System.Text.Encoding.UTF8.GetBytes(jsonObject);
        UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(jsonBinary);
        UnityWebRequest www =
        new UnityWebRequest(userPost, "POST", downloadHandlerBuffer, uploadHandlerRaw);
		www.SetRequestHeader ("Content-Type", "application/json");
        yield return www.Send();
        if (www.isNetworkError || www.downloadHandler.text.IndexOf("Success") == -1)
        {
            Debug.LogError(string.Format("{0}: {1} json is: {2}", www.url, www.error, jsonObject));
        }
        else
        {
            Debug.Log(string.Format("Response: {0}", www.downloadHandler.text));

        }
        signedUp = true;

    }
    StringBuilder sb = new StringBuilder("Type,Mode,StartingTime,ReactionTime,Response,Trial,GSD_X,GSD_Y,GSD_Z,upperLeft,middleLeft,lowerLeft,upperMiddle,middleMiddle,lowerMiddle,upperRight,middleRight,lowerRight");
    [TextArea(40,100)]
    [SerializeField]
    string allData;
    string space = "  ,  ";
    [SerializeField]
    bool save;
    public void ToCSV(StimulusData data)
    {
        
        sb.Append('\n').Append(data.levelDescription).Append(space).Append(GetType(data.stateData)).Append(space)
        .Append(data.startingTimer).Append(space).Append(data.reactionTimer).Append(space).Append(GetResponse(data)).Append(space)
        .Append(data.tryNumber).Append(space).Append(data.gyroScopeData.x).Append(space)
        .Append(data.gyroScopeData.y).Append(space)
        .Append(data.gyroScopeData.z).Append(space).Append(data.upperLeft).Append(space).Append(data.middleLeft)
        .Append(space).Append(data.lowerLeft).Append(space).Append(data.upperMiddle).Append(space).Append(data.middleMiddle)
        .Append(data.lowerMiddle).Append(space).Append(data.upperRight).Append(space).Append(data.middleRight).Append(space).Append(data.lowerRight);
        allData = sb.ToString();
        if(save) SaveToFile();
    }
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
    string Url = "http://gamesdata.cognitivetests.ir/Data/apps/43921cf3-b5ca-4897-a2b9-4ac919e7af77/users/";
    public async void SaveToFile ()
    {
        var content = allData;
    #if UNITY_EDITOR
        var folder = Application.streamingAssetsPath;

        if(! Directory.Exists(folder)) Directory.CreateDirectory(folder);
    #else
        var folder = Application.persistentDataPath;
    #endif

        var filePath = Path.Combine(folder, "export.txt");

        using(var writer = new StreamWriter(filePath, false))
        {
            await writer.WriteAsync(content);
        }
        //File.WriteAllText(content);
        var file = System.IO.File.ReadAllText(filePath); 
        StartCoroutine(PostRequest(file));
    #if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
    #endif
    }
    private IEnumerator PostRequestCoroutine(string json)
    {
    
        var jsonData = System.Text.Encoding.UTF8.GetBytes(json);
        DownloadHandlerBuffer downloadHandlerBuffer = new DownloadHandlerBuffer();
        UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(jsonData);
        UnityWebRequest www =
        new UnityWebRequest(Url + user.userid, "POST", downloadHandlerBuffer, uploadHandlerRaw);
		www.SetRequestHeader ("Content-Type", "text/plain");
        yield return www.SendWebRequest();
        Debug.Log(www.responseCode);
        
    }
    private IEnumerator PostRequest(string json)
    {
        yield return MakeUser();

        var jsonBinary = System.Text.Encoding.UTF8.GetBytes(json);
        Data data = new Data();
        data.userid = user.userid;
        data.appId = user.appId;
        data.file = jsonBinary;
        var jsonObject  = JsonConvert.SerializeObject(data);
        yield return PostRequestCoroutine(jsonObject);
        
    }
}
public class User 
{
    public string userid,appId,userName,firstName,lastName,password;
}
public class Data 
{
    public string userid,appId,location,rawdata;
    public byte [] file;
}