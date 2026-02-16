using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] AnimateType type;
    [SerializeField] Slider slider;
    [SerializeField] int sliderMaxValue;
    [SerializeField] GameObject bubble;
    
    int fill;
    bool animateProgressBarNow;
    bool playAnimation;
    
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
        {
            slider.value -= Time.deltaTime;
        }
        else if(slider.value <= 0)
        {
            Debug.Log("Timer reached 0 - calling HandleTimerEnd");
            bubble.SetActive(false);
            animateProgressBarNow = false;
            ReactionManager.Instance.HandleTimerEnd(); // FIXED: Added this line
        }
    }
    
    public void SliderAnimationState(bool stop)
    {
        playAnimation = !stop;
    }
    
    void Update() 
    {
        if(animateProgressBar && animateProgressBarNow)
            switch (type)
            {
                case AnimateType.Backward:
                    AnimateProgressBarBackward();
                    break;
                case AnimateType.Forward:
                    AnimateProgressBarForward();
                    break;
            }
    }
    
    bool animateProgressBar 
    {
        get => GameFlowController.Instance.gameIsRunning && playAnimation;
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
    Forward,
    Backward
}