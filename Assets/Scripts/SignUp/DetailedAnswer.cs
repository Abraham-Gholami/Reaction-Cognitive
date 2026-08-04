using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTLTMPro; 
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class DetailedAnswer : AnswerBase
{
    public BlankBoxAnswer [] blankBoxes;
    public string[] blankBoxesQuestion;
    public TMP_InputField answerInputField;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void Init()
    {
        base.Init();
        blankBoxes = GetComponentsInChildren<BlankBoxAnswer>(true);
        for (int i = 0; i < blankBoxes.Length; i++)
        {
            blankBoxes[i].Init(blankBoxesQuestion[i]);
        }
    }
    public override string GetAnswer()
    {
        string answers = "";
        foreach (var blankBox in blankBoxes)
        {
            answers += blankBox.GetDetailedAnswer() + "    ";
        }
        return question += "   " + answers;
    }
}
#if UNITY_EDITOR

[CustomEditor(typeof(DetailedAnswer))]
public class DetailedAnswerEditor : Editor 
{
    DetailedAnswer DetailedAnswer;
    public override void OnInspectorGUI() 
    {
        if(GUILayout.Button("Init"))
        {
            DetailedAnswer.Init();
        }
        base.OnInspectorGUI();
        
    }
    private  void OnEnable() 
    {
        DetailedAnswer = (DetailedAnswer) target;
    }
}
#endif