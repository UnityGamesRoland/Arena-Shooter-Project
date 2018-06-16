using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer mixer;
    public AudioClip[] musicClips;

    private AudioSource source;

    #region Singleton and References
    public static AudioManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            source = GetComponent<AudioSource>();
            source.ignoreListenerPause = true;
            DontDestroyOnLoad(gameObject);
        }

        else if (Instance != this) DestroyImmediate(gameObject);
    }
    #endregion

    private void Start()
    {
        source.clip = musicClips[0];
        source.Play();
    }

    public void TransitionToSnapshot(string snapshotName, float transitionTime)
    {
        AudioMixerSnapshot snapshot = mixer.FindSnapshot(snapshotName);
        if (snapshot != null) snapshot.TransitionTo(transitionTime);
    }
}
