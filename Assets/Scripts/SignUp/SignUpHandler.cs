using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;
public class SignUpHandler : MonoBehaviour
{
    [SerializeField]
    Transform questionairesParent;
    [SerializeField]
    AnswerBase [] answerBases;
    [SerializeField]
    GameObject viewPort,wait;
    [SerializeField] UserCreator userCreator;
    [SerializeField] SimpleBlankSpaceAnswer nameBox;
    void Start() 
    {
        answerBases = questionairesParent.GetComponentsInChildren<AnswerBase>();
    }
    public void SignUp()
    {
        userCreator.CreateUser();
        viewPort.SetActive(false);
        wait.SetActive(true);
        WriteString();
        
    }
    [SerializeField] GameObject completed,failed;
    
    async void  WriteString()
    {
        var folder = "";
        var filePath = "";
        #if UNITY_EDITOR
            folder = Application.streamingAssetsPath;
            filePath = Path.Combine(folder, "SignUp.txt");
            if(! Directory.Exists(folder)) Directory.CreateDirectory(folder);
            if(Directory.Exists(filePath)) Directory.Delete(filePath);
        #else
            folder = Application.persistentDataPath;
            filePath = Path.Combine(folder, "SignUp.txt");
            if(Directory.Exists(filePath)) Directory.Delete(filePath);
        #endif
        
        StreamWriter writer = new StreamWriter(filePath, true);
        foreach (var item in answerBases)
        {
            await writer.WriteLineAsync(item.GetAnswer() + Environment.NewLine);
        }
        writer.Close();
        var file = System.IO.File.ReadAllText(filePath); 
        Action<bool> onComplete = new Action<bool>
        (
            (value)=> OnComplete(value)
        );
        FileUploader.Instance.UploadFile(file,"SignUp",onComplete,nameBox.answer);
        #if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
        #endif
    }
    void OnComplete(bool callback)
    {
        StartCoroutine(OnCompleteDelay(callback));
    }
    IEnumerator OnCompleteDelay(bool callback)
    {
        //completed.SetActive(callback);
        //failed.SetActive(!callback);
        yield return new WaitForSeconds(0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
