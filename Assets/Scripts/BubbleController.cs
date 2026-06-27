using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BubbleController : MonoBehaviour
{
    [SerializeField]
    new ParticleSystem  particleSystem;
    [SerializeField]
    GameObject bubble,twoBubbleParticles;
    private void Awake() 
    {
        SettingsManager.Instance.settingPageClosed += OnSettingMenuClosed;
    }
    public void BurstBubble()
    {
        bubble.SetActive(false);
        particleSystem.Play();
    }
    void OnSettingMenuClosed()
    {
        var isActive = SettingsManager.Instance.useBubbleParticle;
        twoBubbleParticles.gameObject.SetActive(isActive);
    }
    private void OnDestroy() 
    {
        SettingsManager.Instance.settingPageClosed -= OnSettingMenuClosed;
    }

}
