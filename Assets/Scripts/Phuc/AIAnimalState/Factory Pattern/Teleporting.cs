using UnityEngine;
using UnityEngine.EventSystems;

public class Teleporting : MonoBehaviour
{
    public enum TeleportType { In, Out }

    [Header("Teleport Setting")]
    public TeleportType teleportType = TeleportType.In;

    [Header("Position To Teleport")]
    public Transform teleportSpawnPoint;

    private bool playerInZone = false;

    private void Update()
    {
        if (!playerInZone)
            return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player == null)
            {
                Debug.LogWarning("Cant Find Player");
                return;
            }
            float distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance > 3f)
            {
                Debug.Log("Player No Longer in The Collider Cant Teleport");
                playerInZone = false;
                return;
            }
            var cc = player.GetComponent<CharacterController>();
            if (cc) cc.enabled = false;

            player.transform.position = teleportSpawnPoint.position;

            if (cc) cc.enabled = true;

            Debug.Log("Success Teleport To: " + teleportType);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            Debug.Log($"Player In The Collider  {teleportType}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            Debug.Log($"Player Left The Collider {teleportType}");
        }
    }
}
