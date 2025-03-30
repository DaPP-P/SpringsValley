using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioClip backgroundMusic;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.volume = 0.5f; // Adjust as needed

            if (backgroundMusic != null)
            {
                audioSource.clip = backgroundMusic;
                audioSource.Play(); // Play the background music
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("PlaySound called with a null AudioClip!");
            return;
        }

        AudioSource.PlayClipAtPoint(clip, position, volume);
    }

}

