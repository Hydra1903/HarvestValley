using UnityEngine;
using UnityEngine.EventSystems;

public class Teleporting : MonoBehaviour
{
    public enum TeleportType { In, Out }

    [Header("Teleport Setting")]
    public TeleportType teleportType = TeleportType.In;

    [Header("Target Teleport")]
    public Teleporting targetTeleport;

    [Header("Position To Teleport")]
    public Transform teleportSpawnPoint;

    private bool playerInZone = false;
    private bool isCoolingDown = false;

    private void Update()
    {
        if (!playerInZone || isCoolingDown)
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

            var cc = player.GetComponent<CharacterController>();
            if (cc) cc.enabled = false;

            if (targetTeleport != null && targetTeleport.teleportSpawnPoint != null)
            {
                player.transform.position = targetTeleport.teleportSpawnPoint.position;
            }
            else
            {
                Debug.LogWarning("TargetTeleport or its spawn point is missing!");
            }

            if (cc) cc.enabled = true;

            Debug.Log($"Success Teleport: {teleportType} ? {targetTeleport.teleportType}");

            StartCoroutine(SetCooldown());
            if (targetTeleport != null)
                targetTeleport.StartCoroutine(targetTeleport.SetCooldown());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            Debug.Log($"Player entered {teleportType}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            Debug.Log($"Player left {teleportType}");
        }
    }

    private System.Collections.IEnumerator SetCooldown()
    {
        isCoolingDown = true;
        playerInZone = false;
        yield return null;
        isCoolingDown = false;
    }
}
