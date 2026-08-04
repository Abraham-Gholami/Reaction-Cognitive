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
    string appId = "43921cf3-b5ca-4897-a2b9-4ac919e7af77";
    string Url = "http://gamesdata.cognitivetests.ir/Data/apps/43921cf3-b5ca-4897-a2b9-4ac919e7af77/users/";
    string location;
    bool hasLocation;
    [SerializeField] float registrationTimeout = 20f;
    [SerializeField] int requestTimeout = 60;

    private IEnumerator PostRequest(string file,string fileName,Action<bool> onComplete,string _location = null)
    {
        if(!hasLocation && _location != null)
        {
            location = _location;
            hasLocation = true;
        } 
		ScreenDebug.Instance.Debug("PostRequest");
        // This used to be an unbounded WaitUntil. If user registration had failed — no
        // network at the school, or the GamePlay-only build where UserCreator does not
        // exist at all — it blocked forever behind the "please wait" panel with no error
        // and the whole session was lost. Give up after a bounded wait and report failure.
        var registrationDeadline = Time.realtimeSinceStartup + registrationTimeout;
        while(!UserCreator.Instance.isRegistered && Time.realtimeSinceStartup < registrationDeadline)
            yield return null;

        if(!UserCreator.Instance.isRegistered)
        {
            ScreenDebug.Instance?.Debug("User not registered - upload aborted");
            onComplete?.Invoke(false);
            yield break;
        }
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
    public  IEnumerator Upload(string url, byte[] file,string fileName,Action<bool> onComplete,string filetext,Data data)
    {
		    ScreenDebug.Instance.Debug("Upload Process");
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            MultipartFormFileSection myFormFile = new MultipartFormFileSection("files", file, 
            fileName, "multipart/form-data");
            WWWForm form = new WWWForm();
            form.AddField("userid", data.userid);
            form.AddField("appid", data.appId);
            form.AddBinaryData("file",data.file);
            form.AddField("fileName",data.fileName);
            form.AddField("rawdata",filetext);
            form.AddField("location",data.location + " " + data.fileName);
            //formData.Add(myFormFile);
            UnityWebRequest www = UnityWebRequest.Post(url, form);
            www.timeout = requestTimeout;   // a hung TCP connection must not stall forever
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                onComplete?.Invoke(false);
                ScreenDebug.Instance.Debug(www.error);

            }
            else
            {
                ScreenDebug.Instance.Debug("Done!!!!!");
                onComplete?.Invoke(true);
            }

            //Debug.Log(www.downloadHandler.text);

            
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
