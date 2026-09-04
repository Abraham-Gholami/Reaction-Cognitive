using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using UnityEngine.UI;
using Newtonsoft.Json;
public class UserCreator : GenericSingleton<UserCreator> {
	
	// https, not http: the server answers plain HTTP with a 301 to the https URL, and
	// UnityWebRequest downgrades a redirected POST to a GET and drops the body - so every
	// registration arrived as a GET and came back 405, leaving isRegistered false and
	// aborting every upload that waits on it.
	// Address lives in Backend so scheme/host is a single edit for both endpoints.
	static string url { get { return Backend.UsersUrl; } }
	string appId = Backend.AppId;
	[SerializeField] int requestTimeout = 15;
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

		byte[] bodyRaw = Encoding.UTF8.GetBytes (bodyJsonString);
		var target = url;

		for (var hop = 0; hop <= Backend.MaxRedirects; hop++)
		{
			var request = new UnityWebRequest (target, "POST");
			request.uploadHandler = (UploadHandler)new UploadHandlerRaw (bodyRaw);
			request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer ();
			request.SetRequestHeader ("Content-Type", "text/json");
			// Do not let Unity follow the redirect itself: it re-sends a POST as a
			// bodyless GET, which this route answers with 405.
			request.redirectLimit = 0;
			request.timeout = requestTimeout;
			yield return request.SendWebRequest();

			var redirect = Backend.FollowRedirect(request, target);
			if (redirect != null)
			{
				ScreenDebug.Instance?.Debug ("Register redirected -> " + redirect);
				target = redirect;
				request.Dispose();
				continue;
			}

			ScreenDebug.Instance?.Debug ("Status Code: " + request.responseCode);
			if (request.result == UnityWebRequest.Result.Success)
			{
				isRegistered = true;
			}
			else
			{
				var detail = "Register failed " + request.responseCode + " " + request.result +
				             ": " + request.error + " -> " + target;
				ScreenDebug.Instance?.Debug (detail);
				Debug.LogError(detail);
			}
			request.Dispose();
			yield break;
		}

		ScreenDebug.Instance?.Debug ("Register gave up after too many redirects");
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

