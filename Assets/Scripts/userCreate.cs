using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.UI;

public class userCreate : MonoBehaviour {
	
	private const string url = "http://gamesdata.cognitivetests.ir/Users";
	private const string AppId = "067a4533-fae4-41cf-b641-76d32f200558";
	string appId = "43921cf3-b5ca-4897-a2b9-4ac919e7af77";
	public string userid;
	public UnityWebRequest request;
	private const string UserID = "UserID";
	public bool isRegistered;
	ApiUser myobj;

	public void Awake()
	{
		
	}
	public void MakeUser()
    {
		myobj = new ApiUser ();
		var newGuid = System.Guid.NewGuid();
		userid = newGuid.ToString();
		myobj.id = userid.ToString();
		myobj.userName = userid.ToString();
		Debug.Log(userid);
		isRegistered = false;
		myobj.appId = appId;
		string json =  JsonConvert.SerializeObject(myobj);
		StartCoroutine(Post(url, json,isRegistered));
    }

	IEnumerator Post(string url, string bodyJsonString, bool registered)
	{
		if (!registered) {
			var request = new UnityWebRequest (url, "POST");
			byte[] bodyRaw = Encoding.UTF8.GetBytes (bodyJsonString);
			request.uploadHandler = (UploadHandler)new UploadHandlerRaw (bodyRaw);
			request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer ();
			request.SetRequestHeader ("Content-Type", "application/json");
			yield return request.Send ();
			Debug.Log ("Status Code: " + request.responseCode);
			
		} else {
			var request = new UnityWebRequest(url, "PUT");
			byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
			request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
			request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.Send();
			Debug.Log("Status Code: " + request.responseCode);
			
		}

	}

}

