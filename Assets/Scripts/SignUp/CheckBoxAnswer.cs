using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class CheckBoxAnswer : AnswerBase
{
    public ToggleAnswer [] toggleAnswers;
    public string  [] toggleAnswersStrings;
    void Start()
    {
        Init();
        
    }
    public override void Init()
    {
        base.Init();
        toggleAnswers = GetComponentsInChildren<ToggleAnswer>(true);
        for (int i = 0; i < toggleAnswers.Length; i++)
        {
            toggleAnswers[i].checkBoxAnswer = this;
            toggleAnswers[i].index = i;
            Debug.Log(toggleAnswers[i].index);
            toggleAnswers[i].Init(toggleAnswersStrings[i]);
            toggleAnswers[i].toggle.isOn = false;
        }
    }
    public override string GetAnswer()
    {
        return  question + "    " + answer;
    }
    public void Answered(bool active ,int toggleIndex)
    {
        for (int i = 0; i < toggleAnswers.Length; i++)
        {
            if(i == toggleIndex)
            {
                toggleAnswers[i].toggle.isOn = active;
            }
            else 
            {
                toggleAnswers[i].toggle.isOn = false;

            }
        }
        answer = toggleAnswers[toggleIndex].answer;
    }

}
#if UNITY_EDITOR

[CustomEditor(typeof(CheckBoxAnswer))]
public class CheckBoxAnswerEditor : Editor 
{
    CheckBoxAnswer CheckBoxAnswer;
    public override void OnInspectorGUI() 
    {
        if(GUILayout.Button("Init"))
        {
            CheckBoxAnswer.Init();
        }
        base.OnInspectorGUI();
        
    }
    private  void OnEnable() 
    {
        CheckBoxAnswer = (CheckBoxAnswer) target;
    }
}
#endif