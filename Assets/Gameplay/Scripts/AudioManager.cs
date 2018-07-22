using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public AudioMixer mixer;
    public AudioMixerGroup musicMixerGroup;
    public Soundtrack[] soundtracks;

    #region Singleton
    public static AudioManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else if (Instance != this) DestroyImmediate(gameObject);
    }
    #endregion

    private void Start()
    {
        PlayMusic(soundtracks[0]);
    }

    public void TransitionToSnapshot(string snapshotName, float transitionTime)
    {
        AudioMixerSnapshot snapshot = mixer.FindSnapshot(snapshotName);
        if (snapshot != null) snapshot.TransitionTo(transitionTime);
    }

    public void PlayUISound(AudioClip soundEffect, float volume)
    {
        AudioSource source = transform.GetChild(0).GetComponent<AudioSource>();

        source.ignoreListenerPause = true;
        source.PlayOneShot(soundEffect, volume);
    }

    public void PlayMusic(Soundtrack newMusic)
    {
        AudioSource currentSource = gameObject.GetComponent<AudioSource>();
        if (currentSource == null) currentSource = gameObject.AddComponent<AudioSource>();

        currentSource.clip = newMusic.clip;
        currentSource.volume = newMusic.volume;
        currentSource.outputAudioMixerGroup = musicMixerGroup;
        currentSource.ignoreListenerPause = true;
        currentSource.playOnAwake = false;
        currentSource.loop = true;

        //Delay a bit to bypass audio initalization pop sound.
        StartCoroutine(BeginMusic(currentSource));
    }

    public void CrossFadeMusic(Soundtrack newMusic, float fadeTime)
    {
        StopAllCoroutines();
        StartCoroutine(BeginMusicFade(newMusic, fadeTime));
    }

    private IEnumerator BeginMusicFade(Soundtrack newMusic, float fadeTime)
    {
        AudioSource currentSource = gameObject.GetComponent<AudioSource>();
        AudioSource newSource = gameObject.AddComponent<AudioSource>();

        newSource.clip = newMusic.clip;
        newSource.volume = 0;
        newSource.outputAudioMixerGroup = musicMixerGroup;
        newSource.ignoreListenerPause = true;
        newSource.playOnAwake = false;
        newSource.loop = true;

        newSource.Play();

        float currentSourceVolume = currentSource.volume;
        float progress = 0f;

        while (progress < 1)
        {
            progress += Time.unscaledDeltaTime / fadeTime;

            newSource.volume = Mathf.Lerp(0, newMusic.volume, progress);
            currentSource.volume = Mathf.Lerp(currentSourceVolume, 0f, progress);

            yield return null;
        }

        Destroy(currentSource);
    }

    private IEnumerator BeginMusic(AudioSource source)
    {
        yield return new WaitForSeconds(0.1f);
        source.Play();
    }
}

[System.Serializable]
public class Soundtrack
{
    public string name = "New Soundtrack";
    public AudioClip clip;
    public float volume = 1;
}
