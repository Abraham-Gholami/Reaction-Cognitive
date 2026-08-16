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
    // The oxygen tank is drawn as a full (yellow) capsule laid exactly over an empty
    // (olive) one, revealed by Image.fillAmount. Driving fillAmount rather than letting
    // the Slider resize the fill rect keeps the artwork undistorted - a resized rect
    // squashes the capsule instead of emptying it.
    [SerializeField] Image fillImage;
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
    [SerializeField] GameObject bubble;
    void AnimateProgressBarBackward()
    {
        if(slider.value > 0)
        {
            // (was Debug.Log(1) here - it fired every frame for the whole test)
            slider.value -= Time.deltaTime;
        }
        else if(slider.value <= 0)
        {
            bubble.SetActive(false);
            animateProgressBarNow = false;

        }
        
    }
    bool playAnimation;
    public void SliderAnimationState(bool stop)
    {
        playAnimation = !stop;
    }
    void SyncFill()
    {
        if(fillImage == null || slider == null) return;
        var range = slider.maxValue - slider.minValue;
        fillImage.fillAmount = range > 0f ? Mathf.Clamp01((slider.value - slider.minValue) / range) : 0f;
    }

    void Update()
    {
        SyncFill();
        if(animateProgressBar && animateProgressBarNow)
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
        get => RandomButtonGenerator.Instance.gameIsRunning && playAnimation;
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
        // Never resurrect the settings singleton during teardown just to unsubscribe.
        var settings = SettingsManager.Instance;
        if (settings != null) settings.settingPageClosed -= OnSettingMenuClosed;
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