using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentHandler : MonoBehaviour
{
    [SerializeField]
    float spacing;
    public Questionnaire [] GetQuestionnaires()
    {
        
        var questionaire =  GetComponentsInChildren<Questionnaire>();
        var rect = questionaire[0].GetComponent<RectTransform>();
        var startingPos = rect.anchoredPosition;
        for (int i = 1; i < questionaire.Length; i++)
        {
            startingPos.y -= spacing;
            questionaire[i].GetComponent<RectTransform>().anchoredPosition = startingPos;
        }
        return questionaire;
    }
}
