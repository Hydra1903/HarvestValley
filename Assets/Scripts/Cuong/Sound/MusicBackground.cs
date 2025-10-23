using UnityEngine;

public class MusicBackground : MonoBehaviour
{
    public static MusicBackground Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public AudioSource audioSourceMusic;
    public AudioClip backgroundMusicDay1;
    public AudioClip backgroundMusicDay2;
    public AudioClip backgroundMusicNight1;
    public AudioClip backgroundMusicNight2;
    void Start()
    {
        ChangeBackgroundMusic();
    }

    void Update()
    {
        PLayNextTrack();
    }
    public void ChangeBackgroundMusic()
    {
        if (GameTime.Instance.currentTimeOfDay == TimeOfDay.Day)
        {
            audioSourceMusic.clip = backgroundMusicDay1;
        }
        else if (GameTime.Instance.currentTimeOfDay == TimeOfDay.Night)
        {
            audioSourceMusic.clip = backgroundMusicNight1;
        }
        audioSourceMusic.Play();
    }
    public void PLayNextTrack()
    {
        if (!audioSourceMusic.isPlaying && GameTime.Instance.currentTimeOfDay == TimeOfDay.Day)
        {
            ChangeMusicDay(audioSourceMusic.clip);
            audioSourceMusic.Play();
        }
        if (!audioSourceMusic.isPlaying && GameTime.Instance.currentTimeOfDay == TimeOfDay.Night)
        {
            ChangeMusicNight(audioSourceMusic.clip);
            audioSourceMusic.Play();
        }
    }
    public void ChangeMusicDay(AudioClip currentMusic)
    {
        if(currentMusic == backgroundMusicDay1)
        {
            audioSourceMusic.clip = backgroundMusicDay2;
        }
        else if(currentMusic == backgroundMusicDay2)
        {
            audioSourceMusic.clip = backgroundMusicDay1;
        }
    }
    public void ChangeMusicNight(AudioClip currentMusic)
    {
        if (currentMusic == backgroundMusicNight1)
        {
            audioSourceMusic.clip = backgroundMusicNight2;
        }
        else if (currentMusic == backgroundMusicNight2)
        {
            audioSourceMusic.clip = backgroundMusicNight1;
        }
    }
}
