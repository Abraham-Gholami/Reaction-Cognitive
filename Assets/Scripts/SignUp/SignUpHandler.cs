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
    [SerializeField] userCreate userCreator;
    
    void Start() 
    {
        answerBases = questionairesParent.GetComponentsInChildren<AnswerBase>();
    }
    public void SignUp()
    {
        userCreator.MakeUser();
        viewPort.SetActive(false);
        wait.SetActive(true);
        WriteString();
    }
    async void  WriteString()
    {
        #if UNITY_EDITOR
            var folder = Application.streamingAssetsPath;
            if(! Directory.Exists(folder)) Directory.CreateDirectory(folder);
        #else
            var folder = Application.persistentDataPath;
        #endif
        var filePath = Path.Combine(folder, "SignUp.text");
        StreamWriter writer = new StreamWriter(filePath, true);
        foreach (var item in answerBases)
        {
            await writer.WriteLineAsync(item.GetAnswer() + Environment.NewLine);
        }
        writer.Close();
        wait.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    
        #if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
        #endif
    }

}
