using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;
using System;
public class SettingsManager : GenericSingleton<SettingsManager>
{
    
    public Action settingPageClosed;
    // Start is called before the first frame update
    public bool useCombo = true ,useFeedBack = true ,useBubbleSFX = true ,useOxygenTank = true ,useStory = true 
    ,diamondSFX = true,useDiamondPrize ,useComboSFX = true , useBubbleParticle = true, useCustomTimer = false;
    public float diamondTime,visualTimer,audioTimer;
    public void ToggleDiamond()
    {
        useDiamondPrize = !useDiamondPrize;
    }
    public void ToggleBubbleSFX()
    {
        useBubbleSFX = !useBubbleSFX;
    }
    public void ToggleOxygenTank()
    {
        useOxygenTank = !useOxygenTank;
    }
    public void ToggleMultiplier()
    {
        useCombo = !useCombo;
    }
    public void ToggleFeedBack()
    {
        useFeedBack = !useFeedBack;
    }
    public void ToggleStory()
    {
        useStory = !useStory;
    }
    public void ToggleDiamondSFX()
    {
        diamondSFX = !diamondSFX;
    }
    public void ToggleMultiplierScoreSFX()
    {
        useComboSFX = !useComboSFX;
    }
    public void ToggleBubbleParticle()
    {
        useBubbleParticle = !useBubbleParticle;
    }
    public void ToggleCustomTimer()
    {
        useCustomTimer = !useCustomTimer;
        audioSlider.gameObject.SetActive(useCustomTimer);
        visualSlider.gameObject.SetActive(useCustomTimer);
    }
    private void Start() 
    {
        SettingPageClosed();
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
