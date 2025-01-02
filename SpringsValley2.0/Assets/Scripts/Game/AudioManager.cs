using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

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

    public void PlaySound(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("PlaySound called with a null AudioClip!");
            return;
        }

        AudioSource.PlayClipAtPoint(clip, position, volume);
    }

    [ContextMenu("Play Test Sound")]
    private void PlayTestSound()
    {
        if (Instance != null)
        {
            Instance.PlaySound(testClip, Vector3.zero);
        }
    }

    public AudioClip testClip; // Assign a test clip in the Inspector
}

