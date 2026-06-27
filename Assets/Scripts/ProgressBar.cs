using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField]
    AnimateType type;
    [SerializeField]
    Slider slider;
    [SerializeField] int sliderMaxValue;
    // Start is called before the first frame update
    int fill;
    private void Awake() 
    {
        SettingsManager.Instance.settingPageClosed += OnSettingMenuClosed;
        if(type == AnimateType.Forward)
            slider.maxValue = sliderMaxValue;
    }
    public void Animate(int value = 0)
    {
        fill = value;
        animateProgressBarNow = true;
    }
    bool animateProgressBarNow;
    void AnimateProgressBarForward()
    {
        if(slider.value < fill)
            slider.value += 0.1f;
        else if(slider.value >= fill)
        {
            slider.value = fill;
            animateProgressBarNow = false;
        }
    }
    void AnimateProgressBarBackward()
    {
        if(slider.value > 0)
            slider.value -= Time.deltaTime;
        if(slider.value <= 0)
            animateProgressBarNow = false;
    }

    void Update() 
    {
        if(animateProgressBar && animateProgressBarNow )
            switch (type)
            {
                case AnimateType.Forward:
                    AnimateProgressBarForward();
                break;
                case AnimateType.Backward:
                    AnimateProgressBarBackward();
                break;
            }
    }
    bool animateProgressBar 
    {
        get => RandomButtonGenerator.Instance.gameIsRunning;
    }
    public void SetMaxFill(float seconds)
    {
        if(!SettingsManager.Instance.useOxygenTank) return;
        slider.maxValue = seconds;
        slider.value = seconds;
        animateProgressBarNow = true;

    } 
    void OnSettingMenuClosed()
    {
        var isActive = SettingsManager.Instance.useOxygenTank;
        slider.gameObject.SetActive(isActive && animateProgressBar);
    }
    public void Activate()
    {
        slider.gameObject.SetActive(true);
    }
    private void OnDestroy() 
    {
        SettingsManager.Instance.settingPageClosed -= OnSettingMenuClosed;
    }
}
public enum AnimateType
{
    Forward,Backward
}
/*
    void OnSettingMenuClosed()
    {
        SettingsManager.Instance.settingPageClosed += OnSettingMenuClosed;

        var isActive = SettingsManager.Instance.useOxygenTank;
        slider.gameObject.SetActive(isActive);
    }
    private void OnDestroy() 
    {
        SettingsManager.settingPageClosed -= OnSettingMenuClosed;
    }
*/