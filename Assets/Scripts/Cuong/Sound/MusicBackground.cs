using UnityEngine;

public class MusicBackground : MonoBehaviour
{
    public static MusicBackground Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public AudioSource audioSourceMusicDay;
    public AudioSource audioSourceMusicNight;
    void Start()
    {
    }
    public void ChangeBackgroundMusic()
    {
        if (GameTime.Instance.currentTimeOfDay == TimeOfDay.Day)
        {
            audioSourceMusicDay.Play();
            audioSourceMusicNight.Stop();
        }
        else if (GameTime.Instance.currentTimeOfDay == TimeOfDay.Night)
        {
            audioSourceMusicNight.Play();
            audioSourceMusicDay.Stop();
        }
    }
}
