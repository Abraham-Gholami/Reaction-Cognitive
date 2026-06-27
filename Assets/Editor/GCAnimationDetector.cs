using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using gamingCloud;
using gamingCloud.Network;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

[CustomEditor(typeof(GCNetworkAnimation))]

public class GCAnimationDetector : Editor
{
    public Animator animator;
    SerializedProperty animatorField;

    void OnEnable()
    {

        animatorField = serializedObject.FindProperty("animator");
    }
    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(animatorField, new GUIContent("Animator"), GUILayout.Height(20));

        List<NormalizeInfo> statesName = new List<NormalizeInfo>();

        var animationScript = target as GCNetworkAnimation;
        if (animationScript.GetComponent<StreambleObjectTag>() == null)
            throw new Exception("There is not any StreambleObjectTag On GameObject.");

        animator = animationScript.animator;
        if (GUILayout.Button("Generate States"))
        {
            var runtimeController = animator.runtimeAnimatorController;
            if (runtimeController == null)
                throw new Exception("There is not Animator On GameObject.");

            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GetAssetPath(runtimeController));
            AnimatorControllerLayer[] layers = controller.layers;
            foreach (var VARIABLE in layers) // generate states
            {
                foreach (var state in VARIABLE.stateMachine.states)
                {
                    statesName.Add(new NormalizeInfo()
                    {
                        li = animator.GetLayerIndex(VARIABLE.name),
                        nt = 0,
                        st = state.state.name
                    });

                }
            }
            foreach (var VARIABLE in layers) // generate sub - states
            {
                foreach (var state in VARIABLE.stateMachine.stateMachines)
                {
                    statesName.Add(new NormalizeInfo()
                    {
                        li = animator.GetLayerIndex(VARIABLE.name),
                        nt = 0,
                        st = state.stateMachine.name
                    });

                }
            }
            GenerateFile(statesName, animationScript.GetComponent<StreambleObjectTag>().id);
        }

    }
    string EncodeTo64(string toEncode)
    {

        byte[] toEncodeAsBytes = ASCIIEncoding.ASCII.GetBytes(toEncode);

        return System.Convert.ToBase64String(toEncodeAsBytes);

    }
    public class NormalizeInfo
    {
        public string st;
        public float nt;
        public int li;
    }
    void GenerateFile(List<NormalizeInfo> states, string name)
    {
        string filePath = Application.dataPath + "/Resources/" + name + "_AnimationStates.txt";
        if (File.Exists(filePath))
            File.WriteAllText(filePath, "");
        StreamWriter outStream = File.CreateText(filePath);
        outStream.Flush();
        Debug.Log(JsonConvert.SerializeObject(states));
        outStream.WriteLine(EncodeTo64(JsonConvert.SerializeObject(states)));
        outStream.Close();
        Debug.Log("Your States has been set!");

    }
}
