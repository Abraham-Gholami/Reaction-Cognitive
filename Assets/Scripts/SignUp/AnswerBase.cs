using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTLTMPro;
public class AnswerBase : MonoBehaviour
{
    public RTLTextMeshPro questionText;
    public virtual void Init()
    {
        questionText.text = question;
    }
    public string question;
    public string answer;
    public virtual string GetAnswer()
    {
        return  answer + "    " + question;
    }


}
