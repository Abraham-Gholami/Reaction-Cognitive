using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gamingCloud;
using gamingCloud.Network;
using gamingCloud.templates;
using System;
using System.IO;
using System.Text;
public class DataSender : MonoBehaviour
{
    public TextAsset text;
    [SerializeField]
    ServerSideData serverSideData;
    string tableId = "617bcb508c071b3b48511291";
    // Start is called before the first frame update
    void Start()
    {
        SendTheData();
    }
    string id;
    // Update is called once per frame
    void Update()
    {

    }
    public void RecieveSessionData(ServerSideData serverData)
    {
        serverSideData = serverData;
    }
    public void MakeID()
    {
        id = serverSideData.firstName + serverSideData.lastName;
    }
    public async void SendTheData()
    {
        ApiResponse response = await Players.CreateGuestPlayer();
        SendData();
    }
    public async void SendData()
    {
        DBaaS_PlayerSession data = new DBaaS_PlayerSession();
        data.playerName = "Test Player";
        data.file = ConvertToBase64("Hi I'm a Test 2040");
        DBaaSResponse resp = await GameUtilities.AddDocumentToDBaaS<DBaaS_PlayerSession>(tableId, data);
        //ApiResponseArray  respt = await GameUtilities.GetTableDataById(tableId);
        //Debug.Log(respt.status);
    }
    string ConvertToBase64(string value)
    {
        //Byte[] bytes = Encoding.ASCII.GetBytes(text);
        string path = "Assets/Resources/text.txt";
        Byte[] bytes = File.ReadAllBytes(path);
        string file = Convert.ToBase64String(bytes);
        return file;
    }
    void ConvertFromBase64()
    {
        
        Byte[] bytes = Convert.FromBase64String("test");
        File.WriteAllBytes("path", bytes);
    }

}
public class DBaaS_PlayerSession
{
    public string playerName,file;
    
}
public class Test
{
    public string name;
}