using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class AudioManager : GenericSingleton<AudioManager>
{
    #region  Variables
    [SerializeField]
    AudioSource musicAudioSource,sfxAudioSource;
    public Clips clips;
    #endregion

    #region  MonoBehaviour
    void Start()
    {
        Setup();
        LoadSettings();
        PlayGameMusic();
    }
    void Reset() 
    {
        Setup();
        gameObject.name = "Audio Manager";
    }
    #endregion 

    #region GeneralMethods
    public void PlayClip(Clip clip)
    {
        if(clip == null)
            return;
        sfxAudioSource.PlayOneShot(clip.audioClip);
    }
    public void PlayMusic(Clip clip)
    {
        if(clip == null)
            return;
        musicAudioSource.loop = clip.loop;
        musicAudioSource.clip = clip.audioClip; 
        musicAudioSource.Play();
    }
    void Setup()
    {
        if(!sfxAudioSource)
        {
            sfxAudioSource = new GameObject("SFX AudioSource").AddComponent<AudioSource>();
            sfxAudioSource.loop = sfxAudioSource.playOnAwake = false;
            sfxAudioSource.transform.SetParent(transform);
        }
        if(!musicAudioSource)
        {
            musicAudioSource = new GameObject("Music AudioSource").AddComponent<AudioSource>();
            musicAudioSource.loop = musicAudioSource.playOnAwake = false;
            musicAudioSource.transform.SetParent(transform);
        }
    }
    public bool musicIsMuted,sfxIsMuted,vibrate;
    string sfxKey = "SFXKey",musicKey = "MusicKey",vibrationKey = "VibrationKey";
    void LoadSettings()
    {
        musicAudioSource.mute = musicIsMuted = PlayerPrefs.GetInt(musicKey, 1) == 0;
        sfxAudioSource.mute = sfxIsMuted = PlayerPrefs.GetInt(sfxKey, 1) == 0;
        vibrate = PlayerPrefs.GetInt(vibrationKey, 1) == 0;
    }
    // ui button
    public void MuteMusic()
    {
        musicAudioSource.mute = musicIsMuted = !musicIsMuted;
        PlayerPrefs.SetInt(musicKey, musicIsMuted ? 0 : 1) ;        
    }
    // ui button
    public void MuteSFX()
    {
       sfxAudioSource.mute = sfxIsMuted=!sfxIsMuted;
       PlayerPrefs.SetInt(sfxKey, sfxIsMuted ? 0 : 1) ;        

    }
    public void Vibration()
    {
       vibrate = !vibrate;
       PlayerPrefs.SetInt(vibrationKey, vibrate ? 0 : 1) ;        

    }
    #endregion

    #region ClipPlayer
    void PlayGameMusic()
    {
        if(clips && clips.gamePlayMusic != null)
        PlayMusic(clips.gamePlayMusic);

    }
    #endregion
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    #region AudioMixer
    #if useAudioMixer
    [SerializeField]
    AudioMixer audioMixer;
    [SerializeField]
    Slider volumeSlider;
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
        PlayerPrefs.SetFloat("volume", volume);
    }
    void SetUpSlider()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("volume");
    }
    #endif
    #endregion
}
