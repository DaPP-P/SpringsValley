using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioClip backgroundMusic;
    public AudioClip acceptSound;
    public AudioClip declineSound;

    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.5f;
    [SerializeField, Range(0f, 1f)] private float soundVolume = 1f;

    private AudioSource musicSource;
    private AudioSource soundSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("A duplicate AudioManager component was removed.", this);
            Destroy(this);
            return;
        }

        Instance = this;

        AudioSource[] existingSources = GetComponents<AudioSource>();
        musicSource = existingSources.Length > 0 ? existingSources[0] : gameObject.AddComponent<AudioSource>();
        soundSource = existingSources.Length > 1 ? existingSources[1] : gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume;
        musicSource.spatialBlend = 0f;

        soundSource.loop = false;
        soundSource.playOnAwake = false;
        soundSource.volume = soundVolume;
        soundSource.spatialBlend = 0f;

        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void PlaySound(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null || soundSource == null)
        {
            return;
        }

        soundSource.PlayOneShot(clip, Mathf.Clamp01(volume));
    }
}
