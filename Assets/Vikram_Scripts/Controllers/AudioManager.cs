using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header(" AudioSources")]
    public AudioSource welcomeAudio;

    public static AudioManager instance;


    private void Awake()
    {
        instance = this;
    }


    public void PlayWelcomeSound()
    {
        welcomeAudio.Play();
    }
}
