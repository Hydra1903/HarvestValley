using UnityEngine;

public class Teleporting : MonoBehaviour
{
    [Header("Get In To Pen")]
    public Transform teleportSpawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false; 
            }

            other.transform.position = teleportSpawnPoint.position;

            if (cc != null)
            {
                cc.enabled = true;
            }
        }
    }
}
