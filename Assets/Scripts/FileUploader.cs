using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using System.Net.Http;
using System.IO;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
public class FileUploader : GenericSingleton<FileUploader>
{
    ///private string appId = "0058aada-1bd1-4171-b993-d328a33a3bfd";
    string appId = "43921cf3-b5ca-4897-a2b9-4ac919e7af77";
    string Url = "https://gamesdata.cognitivetests.ir/Data/apps/43921cf3-b5ca-4897-a2b9-4ac919e7af77/users/";
    string location;
    bool hasLocation;

    private IEnumerator PostRequest(string file,string fileName,Action<bool> onComplete,string _location = null)
    {
        if(!hasLocation && _location != null)
        {
            location = _location;
            hasLocation = true;
        } 
		ScreenDebug.Instance.Debug("PostRequest");
        yield return new WaitUntil(() => UserCreator.Instance.isRegistered); 
        var jsonBinary = System.Text.Encoding.UTF8.GetBytes(file);
        string result = System.Text.Encoding.UTF8.GetString(jsonBinary);
        Data data = new Data();
        data.userid = UserCreator.Instance.userid;
        data.appId = appId;
        data.file = jsonBinary;
        data.fileName = fileName;
        data.location = location;
        var jsonObject  = JsonUtility.ToJson(data);
        var jsonData = System.Text.Encoding.UTF8.GetBytes(file);
        ScreenDebug.Instance.Debug("Upload TRY");
        yield return Upload(Url + UserCreator.Instance.userid,jsonData,fileName,onComplete,file,data);
    }
    
    public IEnumerator Upload(string url, byte[] file, string fileName, Action<bool> onComplete, string filetext, Data data)
    {
        ScreenDebug.Instance.Debug("Starting Upload Process");

        // Construct form
        WWWForm form = new WWWForm();
        form.AddField("userid", data.userid ?? throw new Exception("userid is null"));
        form.AddField("appid", data.appId ?? throw new Exception("appid is null"));
        form.AddBinaryData("file", file, fileName, "application/octet-stream");
        form.AddField("fileName", data.fileName ?? throw new Exception("fileName is null"));
        form.AddField("rawdata", filetext);
        form.AddField("location", $"{data.location} {data.fileName}" ?? throw new Exception("location is null"));

        // Create request
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            // Do not manually set Content-Type; Unity handles this.
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                ScreenDebug.Instance.Debug($"Error: {www.error}");
                ScreenDebug.Instance.Debug($"Response: {www.downloadHandler.text}");
                onComplete?.Invoke(false);
            }
            else
            {
                ScreenDebug.Instance.Debug("Upload Successful!");
                onComplete?.Invoke(true);
            }
        }
    }

    
    public void UploadFile(string file,string fileName,Action<bool> onComplete = null,string _location = null)
    {
        StartCoroutine(PostRequest(file,fileName,onComplete,_location));
    }
    public string ToBinary(string data, bool formatBits = false)
    {
        char[] buffer = new char[(((data.Length * 8) + (formatBits ? (data.Length - 1) : 0)))];
        int index = 0;
        for (int i = 0; i < data.Length; i++)
        {
            string binary = Convert.ToString(data[i], 2).PadLeft(8, '0');
            for (int j = 0; j < 8; j++)
            {
                buffer[index] = binary[j];
                index++;
            }
            if (formatBits && i < (data.Length - 1))
            {
                buffer[index] = ' ';
                index++;
            }
        }
        return new string(buffer);
    }
}
