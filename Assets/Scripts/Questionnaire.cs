using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTLTMPro;
using TMPro;
public class Questionnaire : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    RTLTextMeshPro question,inputFieldPlaceHolder;
    public SignUpQuestionaire thisQuestionaire;
    public void SetUp(SignUpQuestionaire questionaire)
    {
        thisQuestionaire = questionaire;
        question.text = questionaire.question;
        inputFieldPlaceHolder.text = questionaire.sampleAnswer;
    }
    public void OnRecieveAnswer(string answer)
    {
        thisQuestionaire.answer = answer;
    }
    public string GetData()
    {
        return thisQuestionaire.answer + " : " +  thisQuestionaire.question;
    }
}
