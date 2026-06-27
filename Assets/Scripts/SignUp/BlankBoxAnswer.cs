using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTLTMPro; 
using TMPro;
public class BlankBoxAnswer : MonoBehaviour
{
    public TMP_InputField answerInputField;
    public string answer,question;
    public TMP_Text questionText,placeHolder;
    // Start is called before the first frame update
    void Start()
    {
        answerInputField.onEndEdit.AddListener
        (
            (value)=> OnRecieveAnswer(value)
        );
    }

    // Update is called once per frame
    public void Init(string question)
    {
        placeHolder.text = this.question = question;
        //placeHolder.text = "وارد کنید";
    }
    public string GetDetailedAnswer()
    {
        return answer + "  " + question;
    }
    public void OnRecieveAnswer(string answer)
    {
        this.answer = answer;
    }
    private void OnDestroy() 
    {
        answerInputField.onEndEdit.RemoveAllListeners();
    }
}
