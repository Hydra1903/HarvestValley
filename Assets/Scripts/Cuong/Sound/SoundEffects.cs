using UnityEngine;

public class SoundEffects : MonoBehaviour
{

    public static SoundEffects Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public AudioSource audioSourceSoundEffects;
    public AudioClip houseDoor;
    public AudioClip penDoor;
    public AudioClip barnDoor;
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public void PlaySound_HouseDoor()
    {
        audioSourceSoundEffects.clip = houseDoor;
        audioSourceSoundEffects.Play();
    }
    public void PlaySound_PenDoor()
    {
        audioSourceSoundEffects.clip = penDoor;
        audioSourceSoundEffects.Play();
    }
    public void PlaySound_BarnDoor()
    {
        audioSourceSoundEffects.clip = barnDoor;
        audioSourceSoundEffects.Play();
    }

}
