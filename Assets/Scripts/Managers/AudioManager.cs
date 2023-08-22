using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public List<AudioClip> LevelsMusic;
    public AudioClip MenuMusic;
    public AudioClip SelectLevelMusic;
    private string currentScene;
    private static AudioManager instance = null;
    public AudioSource musicSource;
    public AudioSource whiteNoiseSource;
    public AudioSource uiSource;
    private float baseFade = 1.5f;

    void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        currentScene = SceneManager.GetActiveScene().name;
        AudioEventManager.ChangeMusic += HandleChangeMusic;
        AudioEventManager.ChangeAudioVolume += HandleChangeAudioVolume;
        AudioEventManager.PlayUISound += HandlePlayUISound;
        SceneManager.activeSceneChanged += HandleActiveSceneChange;
    }

    void Start()
    {
        uiSource.volume = PlayerPrefs.GetFloat("SfxVolume");
        musicSource.volume = 0;
        musicSource.clip = MenuMusic;
        musicSource.Play();
        StartCoroutine(StartFade(this.musicSource, null, baseFade, PlayerPrefs.GetFloat("MusicVolume")));
    }

    private void HandlePlayUISound(AudioClip audioClip)
    {
        uiSource.clip = audioClip;
        uiSource.Play();
    }

    private void HandleChangeAudioVolume()
    {
        musicSource.volume = PlayerPrefs.GetFloat("MusicVolume");
        whiteNoiseSource.volume = PlayerPrefs.GetFloat("SfxVolume") / 5;
        uiSource.volume = PlayerPrefs.GetFloat("SfxVolume");
    }

    private void HandleChangeMusic(AudioClip audioClip, string sceneName)
    {
        if (audioClip == null)
        {
            audioClip = ReturnMusicForScene(sceneName);
        }

        if (audioClip.name == musicSource.clip.name) return;
        StartCoroutine(StartFade(this.musicSource, audioClip, baseFade, 0));
    }

    private void HandleActiveSceneChange(Scene prev, Scene loaded)
    {
        if (currentScene != loaded.name)
        {
            currentScene = loaded.name;
            HandleChangeMusic(null, currentScene);
        }

        var player = FindAnyObjectByType<Frog>();
        if (player != null)
        {
            foreach (var audioSources in player.GetComponents<AudioSource>())
            {
                audioSources.volume = PlayerPrefs.GetFloat("SfxVolume");
            }
        }
    }

    private AudioClip ReturnMusicForScene(string sceneName)
    {
        int level = 0;

        try
        {
            level = int.Parse(sceneName);
        }
        catch
        {
            switch (sceneName)
            {
                case "Menu":
                    return MenuMusic;
                case "SelectLevel":
                    return SelectLevelMusic;
                default:
                    return LevelsMusic[0];
            }
        }

        try
        {
            return LevelsMusic[level];
        }
        catch
        {
            return LevelsMusic[0];
        }
    }

    public IEnumerator StartFade(AudioSource audioSource, AudioClip toClip, float duration, float targetVolume)
    {
        if (audioSource == null)
        {
            yield break;
        }

        float currentTime = 0;
        float start = audioSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }

        if (toClip != null)
        {
            audioSource.clip = toClip;
            audioSource.Play();
            StartCoroutine(StartFade(audioSource, null, 1, PlayerPrefs.GetFloat("MusicVolume")));
        }

        yield break;
    }
}
