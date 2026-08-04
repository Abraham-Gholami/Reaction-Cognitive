using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class InputDataController : MonoBehaviour,IPointerDownHandler
{
    [SerializeField]
    TouchType touchType;
    [SerializeField]
    Image image;
    [SerializeField]
    Text text;
    int inputCount;
    public void OnPointerDown(PointerEventData eventData)
    {
        ReactionManager.Instance.AddInputData(touchType);
        inputCount ++;
        if(text && text.gameObject.activeInHierarchy)
            text.text = inputCount.ToString();
    }
    private void Start() 
    {
        GetRefs();
    }
    public void SetName()
    {
        gameObject.name = touchType.ToString();
    }
    bool full;
    public void SetImageColor()
    {
        full = !full;
        var color = image.color;
        color.a = full ? 1:0;
        image.color = color;
        image.enabled = false;
        image.enabled = true;
        if(text)
        {
            text.gameObject.SetActive(full);
        }

    }
    public void GetRefs()
    {
        image = GetComponent<Image>();
        text = GetComponentInChildren<Text>(true);
        if(text)
        {
            text.text = inputCount.ToString();
            text.gameObject.SetActive(false);
        }
        
    }
}
[System.Serializable]
public enum TouchType
{
    upperLeft,middleLeft,lowerLeft,upperMiddle,middleMiddle,lowerMiddle,upperRight,middleRight,lowerRight
}
#if UNITY_EDITOR
[CustomEditor(typeof(InputDataController))]
public class InputDataControllerEditor : Editor {
    InputDataController InputDataController;
     private void OnEnable() {
        InputDataController = (InputDataController) target;
    }
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if(GUILayout.Button("Set Name"))
        {
            InputDataController.SetName();
        }
        if(GUILayout.Button("Set Image Full"))
        {
            InputDataController.GetRefs();
            InputDataController.SetImageColor();
        }
        
    }
}
#endif

