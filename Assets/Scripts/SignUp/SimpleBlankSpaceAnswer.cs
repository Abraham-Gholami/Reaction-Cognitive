using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class SimpleBlankSpaceAnswer : AnswerBase
{
    // Start is called before the first frame update
    [SerializeField]
    BlankBoxAnswer blankBoxAnswer;
    void Start()
    {
        Init();
    }
    public override void Init()
    {
        base.Init();
        blankBoxAnswer = GetComponentInChildren<BlankBoxAnswer>();
        blankBoxAnswer.answerInputField.onEndEdit.AddListener
        (
            (value)=> Answer(value)
        );
        blankBoxAnswer.Init("");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void Answer(string _answer)
    {
        answer = _answer;
    
    }
    private void OnDestroy() 
    {
        blankBoxAnswer.answerInputField.onEndEdit.RemoveAllListeners();
    }
}
#if UNITY_EDITOR

[CustomEditor(typeof(SimpleBlankSpaceAnswer))]
public class SimpleBlankSpaceAnswerEditor : Editor 
{
    SimpleBlankSpaceAnswer simpleBlankSpaceAnswer;
    public override void OnInspectorGUI() 
    {
        if(GUILayout.Button("Init"))
        {
            simpleBlankSpaceAnswer.Init();
        }
        base.OnInspectorGUI();
        
    }
    private  void OnEnable() 
    {
        simpleBlankSpaceAnswer = (SimpleBlankSpaceAnswer) target;
    }
}
#endif