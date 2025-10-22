using UnityEngine;

public class FootstepSound : MonoBehaviour
{
    public CharacterSound characterSound;
    public float rayDistance = 1.5f;
    private string currentGroundTag;
    private void Start()
    {
        characterSound.audioSourceCharacter.Stop();
    }
    void Update()
    {
        DetectGround();
    }

    void DetectGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance))
        {
            if (!string.IsNullOrEmpty(currentGroundTag) && hit.collider.CompareTag(currentGroundTag))
                return;
            currentGroundTag = hit.collider.tag;
            //Debug.Log(hit.collider.tag);
            PlayFootstep();
        }
    }

    public void PlayFootstep()
    {
        AudioClip clip;
        switch (currentGroundTag)
        {
            case "Sound_Dirt":
                clip = characterSound.footstep_OnDirt;
                break;
            case "Sound_Wood":
                clip = characterSound.footstep_OnWood;
                break;
            default:
                clip = characterSound.footstep_OnGrass; 
                break;
        }
        characterSound.audioSourceCharacter.clip = clip;
        if (CharacterStateMachine.Instance.currentState == CharacterStateMachine.Instance.walkState ||
            CharacterStateMachine.Instance.currentState == CharacterStateMachine.Instance.runState)
        {
            characterSound.audioSourceCharacter.Play();
        }   
    }
}
