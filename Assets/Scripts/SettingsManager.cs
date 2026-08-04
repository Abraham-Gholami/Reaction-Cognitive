using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class SettingsManager : GenericSingleton<SettingsManager>
{
    public GameObject inputGettersParent;
    public InputDataController [] getControllers;
    public void SetDataGettters()
    {
        foreach (var item in getControllers)
        {
            item.SetImageColor();
        }
    }
    public Action settingPageClosed;
    // Start is called before the first frame update
    public bool useCombo = true ,useFeedBack = true ,useBubbleSFX = true ,useOxygenTank = true ,useStory = true 
    ,diamondSFX = true,useDiamondPrize ,useComboSFX = true , useBubbleParticle = true, useCustomTimer = false;
    public float diamondTime,visualTimer,audioTimer;
    public void ToggleDiamond()
    {
        useDiamondPrize = !useDiamondPrize;
        SFXPlayer.Instance.PlaySFX();
    }
    public void ToggleBubbleSFX()
    {
        SFXPlayer.Instance.PlaySFX();
        useBubbleSFX = !useBubbleSFX;
    }
    public void ToggleOxygenTank()
    {
        SFXPlayer.Instance.PlaySFX();

        useOxygenTank = !useOxygenTank;
    }
    public void ToggleMultiplier()
    {
        SFXPlayer.Instance.PlaySFX();
        useCombo = !useCombo;
    }
    public void ToggleFeedBack()
    {
        SFXPlayer.Instance.PlaySFX();
        useFeedBack = !useFeedBack;
    }
    public void ToggleStory()
    {
        SFXPlayer.Instance.PlaySFX();
        useStory = !useStory;
    }
    public void ToggleDiamondSFX()
    {
        SFXPlayer.Instance.PlaySFX();
        diamondSFX = !diamondSFX;
    }
    public void ToggleMultiplierScoreSFX()
    {
        SFXPlayer.Instance.PlaySFX();
        useComboSFX = !useComboSFX;
    }
    public void ToggleBubbleParticle()
    {
        SFXPlayer.Instance.PlaySFX();
        useBubbleParticle = !useBubbleParticle;
    }
    public void ToggleCustomTimer()
    {
        SFXPlayer.Instance.PlaySFX();
        useCustomTimer = !useCustomTimer;
        audioSlider.gameObject.SetActive(useCustomTimer);
        visualSlider.gameObject.SetActive(useCustomTimer);
    }
    private void Start() 
    {
        SettingPageClosed();
        getControllers = inputGettersParent.GetComponentsInChildren<InputDataController>();

    }
    public bool closed;
    private void Update() {
        if(closed)
        {
            SettingPageClosed();
            closed = false;
        }
    }
    public void SettingPageClosed()
    {
        settingPageClosed?.Invoke();
    }
    [SerializeField]
    GameObject settingsMenu;
    [SerializeField]
    Text audioTimerText,visualTimerText;
    bool pageIsOpen;
    public void HandleSettingPage()
    {
        pageIsOpen = !pageIsOpen;
        settingsMenu.SetActive(pageIsOpen);

    }
    string second =  " ﻪﯿﻧﺎﺛ ";
    public void SetAudioTimer(float timer)
    {
        audioTimer = timer;
        audioTimerText.text = timer +second;
    }
    public void SetVisualTimer(float timer)
    {

        visualTimer = timer;
        visualTimerText.text = timer +second;

    }
    [SerializeField]
    Slider audioSlider,visualSlider;

}
#if UNITY_EDITOR
[CustomEditor(typeof(SettingsManager))]
public class SettingsManagerEditor : Editor {
    SettingsManager settingsManager;
     private void OnEnable() {
        settingsManager = (SettingsManager) target;
    }
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if(GUILayout.Button("SetData Getters"))
        {
            settingsManager.getControllers = settingsManager.inputGettersParent.GetComponentsInChildren<InputDataController>();
            settingsManager.SetDataGettters();
        }
        
    }
}
#endif