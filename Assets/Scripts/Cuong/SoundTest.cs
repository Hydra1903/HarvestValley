using UnityEngine;

public class SoundTest : MonoBehaviour
{
    public static SoundTest Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public AudioSource AudioSource;
    public void Play()
    {
        AudioSource.Play();
    }
}
