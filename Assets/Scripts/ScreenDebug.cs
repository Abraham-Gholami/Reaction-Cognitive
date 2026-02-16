using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenDebug : GenericSingleton<ScreenDebug>
{
    [SerializeField] bool debugging;
    private void Start() {
        if(!debugging && debug ) debug.gameObject.SetActive(false);
    }
    [SerializeField] Text debug;
    public void Debug(string msg)
    {
        if(!debug) return;
        debug.text += System.Environment.NewLine + msg; 
        UnityEngine.Debug.Log(msg);
    }
}
