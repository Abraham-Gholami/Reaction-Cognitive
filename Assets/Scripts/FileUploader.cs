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
    string appId = Backend.AppId;
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
        yield return Upload(Backend.DataUrl(UserCreator.Instance.userid),jsonData,fileName,onComplete,file,data);
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
            var target = url;
            for (var hop = 0; hop <= Backend.MaxRedirects; hop++)
            {
                UnityWebRequest www = UnityWebRequest.Post(target, form);
                // Unity's own redirect handling re-sends a POST as a bodyless GET, which
                // this route answers 405 - the file never leaves the device. Handle the
                // hop ourselves so the body survives, whichever scheme the server wants.
                www.redirectLimit = 0;
                www.timeout = requestTimeout;   // a hung TCP connection must not stall forever
                yield return www.SendWebRequest();

                var redirect = Backend.FollowRedirect(www, target);
                if (redirect != null)
                {
                    ScreenDebug.Instance.Debug("Upload redirected -> " + redirect);
                    target = redirect;
                    www.Dispose();
                    continue;
                }

                if (www.result != UnityWebRequest.Result.Success)
                {
                    // The bare www.error hid what was happening (a 405 on a redirected
                    // POST). Carry the status code and the final URL.
                    var detail = $"Upload failed {(int)www.responseCode} {www.result}: {www.error} -> {target}";
                    ScreenDebug.Instance.Debug(detail);
                    Debug.LogError(detail);
                    www.Dispose();
                    onComplete?.Invoke(false);
                }
                else
                {
                    ScreenDebug.Instance.Debug("Done!!!!!");
                    www.Dispose();
                    onComplete?.Invoke(true);
                }
                yield break;
            }

            ScreenDebug.Instance.Debug("Upload gave up after too many redirects");
            onComplete?.Invoke(false);
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
