using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using UnityEngine.UI;
using Newtonsoft.Json;
public class UserCreator : GenericSingleton<UserCreator> {
	
	private const string url = "http://gamesdata.cognitivetests.ir/Users";
	string appId = "43921cf3-b5ca-4897-a2b9-4ac919e7af77";
	public string userid;
	public bool isRegistered;
	ApiUser myobj;
	public void CreateUser()
    {
		ScreenDebug.Instance.Debug("requested");
		myobj = new ApiUser ();
		var newGuid = System.Guid.NewGuid();
		userid = newGuid.ToString();
		myobj.id = userid.ToString();
		myobj.userName = userid.ToString();
		myobj.password =  System.Guid.NewGuid().ToString();
		isRegistered = false;
		myobj.appId = appId;
		string json =  JsonConvert.SerializeObject(myobj);
		Debug.Log(json);
		StartCoroutine(PostWithJSON(url, json));
    }

	IEnumerator PostWithJSON(string url, string bodyJsonString)
	{
		ScreenDebug.Instance.Debug("ApiUser Created");

		var request = new UnityWebRequest (url, "POST");
		byte[] bodyRaw = Encoding.UTF8.GetBytes (bodyJsonString);
		request.uploadHandler = (UploadHandler)new UploadHandlerRaw (bodyRaw);
		request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer ();
		request.SetRequestHeader ("Content-Type", "text/json");
		yield return request.SendWebRequest();
		ScreenDebug.Instance?.Debug ("Status Code: " + request.responseCode);
		if (request.result == UnityWebRequest.Result.Success)
        {
            isRegistered = true;

        }
		else 
		{
			ScreenDebug.Instance?.Debug ("Error : " + request.error);
		}
	}
	IEnumerator  PostWithForm(ApiUser user ,string url, string bodyJsonString)
    {
		ScreenDebug.Instance?.Debug("PostWithForm Created");

        UnityEngine.WWWForm form = new UnityEngine.WWWForm();
        form.AddField("id",user.id);
        form.AddField("userName",user.userName);
        form.AddField("password",user.password);
        form.AddField("firstName",user.firstName);
        form.AddField("lastName",user.lastName);
        form.AddField("appId",user.appId);
        var jsonObject = JsonUtility.ToJson(form);
        var data = System.Text.Encoding.UTF8.GetBytes(jsonObject); 
        UnityWebRequest www= UnityWebRequest.Put(url, data);
        UploadHandler uploadHandlerRaw = new UploadHandlerRaw(data);
        www.method = "POST";
		www.SetRequestHeader("Content-Type", "application/json");
        www.uploadHandler = uploadHandlerRaw;
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
            ScreenDebug.Instance?.Debug(www.error);
        else
            ScreenDebug.Instance?.Debug("Form upload complete!");

    }

}

