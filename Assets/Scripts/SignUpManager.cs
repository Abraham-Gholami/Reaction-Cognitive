using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;
public class SignUpManager : MonoBehaviour
{   [SerializeField]
    SignUpQuestionaire firstLastName,sex,dateOfBirth,isLeftHanded,beenToShrink,hasADHD,hasOtherProblems,
    isUsingMediceneForADHD,uedMedicene,examinerName,examinerRelation,date,time,place,distraction,
    nThChild,areRelativesNuts,isSick,isDistracted;
    public Questionnaire [] questionnaires;
    public ContentHandler ch;
    [SerializeField]
    GameObject viewPort,wait;

    void Start()
    {
        questionnaires = ch.GetQuestionnaires();
        int counter = 0;
        
        firstLastName.SetUp(questionnaires[counter]);
        counter ++;
        sex.SetUp(questionnaires[counter]);
        counter ++;
        dateOfBirth.SetUp(questionnaires[counter]);
        counter ++;
        isLeftHanded.SetUp(questionnaires[counter]);
        counter ++;
        beenToShrink.SetUp(questionnaires[counter]);
        counter ++;
        hasADHD.SetUp(questionnaires[counter]);
        counter ++;
        hasOtherProblems.SetUp(questionnaires[counter]);
        counter ++;
        isUsingMediceneForADHD.SetUp(questionnaires[counter]);
        counter ++;
        uedMedicene.SetUp(questionnaires[counter]);
        counter ++;
        examinerName.SetUp(questionnaires[counter]);
        counter ++;
        examinerRelation.SetUp(questionnaires[counter]);
        counter ++;
        date.SetUp(questionnaires[counter]);
        counter ++;
        time.SetUp(questionnaires[counter]);
        counter ++;
        place.SetUp(questionnaires[counter]);
        counter ++;
        distraction.SetUp(questionnaires[counter]);
        counter ++;
        nThChild.SetUp(questionnaires[counter]);
        counter ++;
        areRelativesNuts.SetUp(questionnaires[counter]);
        counter ++;
        isSick.SetUp(questionnaires[counter]);
        counter ++;
        Debug.Log(counter);

        isDistracted.SetUp(questionnaires[counter]);
    }
    public void SignUp()
    {
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
        foreach (var item in questionnaires)
        {
            await writer.WriteLineAsync(item.GetData() + Environment.NewLine);
        }
        writer.Close();
        wait.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    
        #if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
        #endif
    }
    void CreateText()
    {

    }
}
[System.Serializable]
public class SignUpQuestionaire
{
    public string question,sampleAnswer,answer;
    public Questionnaire questionnaire;
    public void SetUp(Questionnaire _questionnaire)
    {
        questionnaire = _questionnaire;
        questionnaire.SetUp(this);
    }
    public SignUpQuestionaire thisQuestionnaire 
    {
        get => questionnaire.thisQuestionaire;
    }
}

