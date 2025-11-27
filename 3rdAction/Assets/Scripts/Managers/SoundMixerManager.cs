using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public void SetMasterVolume(float lvl)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(lvl)*20);
    }

    public void SetMusicVolume(float lvl)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(lvl) * 20);
    }

    public void SetSFXVolume(float lvl)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(lvl) * 20);
    }
}
