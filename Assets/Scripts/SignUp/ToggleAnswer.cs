using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RTLTMPro;

public class ToggleAnswer : MonoBehaviour
{
    // Start is called before the first frame update
    public Toggle toggle ;
    public string answer ;
    public int index;
    public RTLTextMeshPro answerText;
    public void Init(string _answer)
    {
        answerText.text = answer = _answer;
        if(checkBoxAnswer)
            toggle.onValueChanged.AddListener
            (
                (value)=> Answered(value)
            );
    }
    public void Answered(bool active)
    {
        checkBoxAnswer.Answered(active,index);
    }
    public CheckBoxAnswer checkBoxAnswer;
   private void OnDestroy() 
   {
        toggle.onValueChanged.RemoveAllListeners();
   }
}
