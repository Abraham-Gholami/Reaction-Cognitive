using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTLTMPro; 
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class DetailedWithCheckBoxAnswer : CheckBoxAnswer
{
    public BlankBoxAnswer [] blankBoxes;
    public string[] blankBoxesQuestion;
    void Start()
    {
        Init();
    }
    
    public override void Init()
    {
        base.Init();
        toggleAnswers = GetComponentsInChildren<ToggleAnswer>();
        blankBoxes = GetComponentsInChildren<BlankBoxAnswer>();
        for (int i = 0; i < blankBoxes.Length; i++)
        {
            blankBoxes[i].Init(blankBoxesQuestion[i]);
        }
        for (int i = 0; i < toggleAnswers.Length; i++)
        {
            toggleAnswers[i].checkBoxAnswer = this;
            toggleAnswers[i].index = i;
            Debug.Log(toggleAnswers[i].index);
            toggleAnswers[i].Init(toggleAnswersStrings[i]);
            toggleAnswers[i].toggle.isOn = false;
        }
        for (int i = 0; i < toggleAnswers.Length; i++)
        {
            toggleAnswers[i].index = i;
            toggleAnswers[i].toggle.onValueChanged.AddListener
            (
                (value)=> Answered(true, toggleAnswers[i].index)
            );
        }
    }
    public override string GetAnswer()
    {
        string answers = "";
        foreach (var blankBox in blankBoxes)
        {
            answers += blankBox.GetDetailedAnswer() + "    ";
        }
        foreach (var toggle in toggleAnswers)
        {
            answers += toggle.answer + "    ";
        }
        Debug.Log(answers);
        return question += "   " + answers;


    }
}
#if UNITY_EDITOR

[CustomEditor(typeof(DetailedWithCheckBoxAnswer))]
public class DetailedWithCheckBoxAnswerEditor : Editor 
{
    DetailedWithCheckBoxAnswer detailedWithCheckBoxAnswer;
    public override void OnInspectorGUI() 
    {
        if(GUILayout.Button("Init"))
        {
            detailedWithCheckBoxAnswer.Init();
        }
        base.OnInspectorGUI();
        
    }
    private  void OnEnable() 
    {
        detailedWithCheckBoxAnswer = (DetailedWithCheckBoxAnswer) target;
    }
}
#endif