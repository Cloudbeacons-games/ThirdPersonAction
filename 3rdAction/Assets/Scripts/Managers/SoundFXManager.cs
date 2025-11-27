using NUnit.Framework;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance;
    [SerializeField] private AudioSource soundFXObject;
    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    public void PlaySoundFXClip(AudioClip clip, Transform spawnTransform , float volume)
    {
        AudioSource audiosource = Instantiate(soundFXObject, spawnTransform.position,Quaternion.identity);

        audiosource.clip = clip;

        audiosource.volume = volume;

        audiosource.Play();

        float clipLength = audiosource.clip.length;

        Destroy(audiosource.gameObject, clipLength);

    }
}
