using UnityEngine;
using System.Collections.Generic;

public class AudioHandler : MonoBehaviour
{
    public static AudioHandler Instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private List<AudioSource> sfxSources = new List<AudioSource>();

    private int _currentSFXSourceIndex = 0;
    private float _musicVolume = 1f;
    private float _sfxVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Play music with optional fade-in
    public void PlayMusic(AudioClip clip, bool loop = true, float fadeDuration = 0f)
    {
        if (musicSource.isPlaying)
        {
            StartCoroutine(FadeOutMusic(fadeDuration, () =>
            {
                musicSource.clip = clip;
                musicSource.loop = loop;
                musicSource.Play();
                StartCoroutine(FadeInMusic(fadeDuration));
            }));
        }
        else
        {
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
            StartCoroutine(FadeInMusic(fadeDuration));
        }
    }

    // Stop music with optional fade-out
    public void StopMusic(float fadeDuration = 0f)
    {
        if (fadeDuration > 0)
        {
            StartCoroutine(FadeOutMusic(fadeDuration, () => musicSource.Stop()));
        }
        else
        {
            musicSource.Stop();
        }
    }

    // Play SFX with optional volume
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (sfxSources.Count == 0) return;

        AudioSource source = sfxSources[_currentSFXSourceIndex];
        source.PlayOneShot(clip, volume * _sfxVolume);

        // Cycle through SFX sources
        _currentSFXSourceIndex = (_currentSFXSourceIndex + 1) % sfxSources.Count;
    }

    // Set music volume
    public void SetMusicVolume(float volume)
    {
        _musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = _musicVolume;
    }

    // Set SFX volume
    public void SetSFXVolume(float volume)
    {
        _sfxVolume = Mathf.Clamp01(volume);
    }

    // Fade in music
    private System.Collections.IEnumerator FadeInMusic(float duration)
    {
        float startVolume = 0f;
        musicSource.volume = startVolume;

        while (musicSource.volume < _musicVolume)
        {
            musicSource.volume += Time.deltaTime / duration;
            yield return null;
        }

        musicSource.volume = _musicVolume;
    }

    // Fade out music
    private System.Collections.IEnumerator FadeOutMusic(float duration, System.Action onComplete = null)
    {
        float startVolume = musicSource.volume;

        while (musicSource.volume > 0)
        {
            musicSource.volume -= Time.deltaTime / duration;
            yield return null;
        }

        musicSource.Stop();
        onComplete?.Invoke();
    }
}
