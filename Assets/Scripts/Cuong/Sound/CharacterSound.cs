using UnityEngine;

public class CharacterSound : MonoBehaviour
{
    public static CharacterSound Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public AudioSource audioSourceCharacter;
    public AudioClip footstep_OnDirt;
    public AudioClip footstep_OnWood;
    public AudioClip footstep_OnGrass;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
