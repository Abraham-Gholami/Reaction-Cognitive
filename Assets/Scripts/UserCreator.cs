using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json;
public class UserCreator : GenericSingleton<UserCreator> {
	
	private const string url = "https://gamesdata.cognitivetests.ir/Users";
	///private const string AppId = "0058aada-1bd1-4171-b993-d328a33a3bfd";
	string AppId = "43921cf3-b5ca-4897-a2b9-4ac919e7af77";
	public string userid;
	public bool isRegistered;
	ApiUser myobj;
	public void CreateUser()
    {
		ScreenDebug.Instance.Debug("requested");
		myobj = new ApiUser ();
		var newGuid = System.Guid.NewGuid();
		userid = newGuid.ToString();
		myobj.id = userid;
		myobj.userName = userid;
		myobj.password =  System.Guid.NewGuid().ToString();
		isRegistered = false;
		myobj.appId = AppId;
		string json =  JsonConvert.SerializeObject(myobj);

		Debug.Log(json);
		StartCoroutine(PostWithJSON(url, json));
    }

	IEnumerator PostWithJSON(string url, string bodyJsonString)
	{
		ScreenDebug.Instance.Debug("ApiUser Created");

		if (string.IsNullOrEmpty(bodyJsonString))
		{
			ScreenDebug.Instance?.Debug("Error: JSON body is empty.");
			yield break;
		}

		// Log JSON body
		ScreenDebug.Instance.Debug("JSON Body: " + bodyJsonString);

		// Create a new UnityWebRequest
		var request = new UnityWebRequest(url, "POST");
    
		// Convert the JSON string to bytes and assign it to the upload handler
		byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
		request.uploadHandler = new UploadHandlerRaw(bodyRaw);
		request.downloadHandler = new DownloadHandlerBuffer();
    
		// Set the content type header to application/json
		request.SetRequestHeader("Content-Type", "application/json");

		// Log raw request data for debugging
		ScreenDebug.Instance.Debug("Raw Request Data: " + System.Text.Encoding.UTF8.GetString(bodyRaw));

		// Send the request
		yield return request.SendWebRequest();

		// Log the status code and response
		ScreenDebug.Instance?.Debug("Status Code: " + request.responseCode);
		ScreenDebug.Instance?.Debug("Response: " + request.downloadHandler.text);

		if (request.result == UnityWebRequest.Result.Success)
		{
			isRegistered = true;
		}
		else 
		{
			ScreenDebug.Instance?.Debug("Error: " + request.error);
			Debug.Log(request.downloadHandler.text);
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
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.DataProcessingError)
            ScreenDebug.Instance?.Debug(www.error);
        else
            ScreenDebug.Instance?.Debug("Form upload complete!");

    }

}

