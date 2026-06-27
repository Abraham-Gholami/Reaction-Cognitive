using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class InputDataController : MonoBehaviour,IPointerDownHandler
{
    [SerializeField]
    TouchType touchType;
    public void OnPointerDown(PointerEventData eventData)
    {
        ReactionManager.Instance.AddInputData(touchType);
    }
    public void SetName()
    {
        gameObject.name = touchType.ToString();
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
        
    }
}
#endif

