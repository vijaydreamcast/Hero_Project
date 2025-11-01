using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header(" AudioSources")]
    public AudioSource backGroundAS;
    public AudioSource trafficAS;

    public static AudioManager instance;


    private void Awake()
    {
        instance = this;
    }


    public void PlayBG()
    {
        backGroundAS.Play();
    }

    public void StopBG()
    {
        backGroundAS.Stop();
    }


}
