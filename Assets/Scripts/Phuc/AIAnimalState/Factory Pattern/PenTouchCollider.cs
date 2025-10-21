using UnityEngine;

public class PenTouchCollider : MonoBehaviour
{
    [SerializeField] private AnimalPenUIManager uiManager;

    private void Awake()
    {
        if (uiManager == null)
            uiManager = GetComponentInParent<AnimalPenUIManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && uiManager != null)
        {
            uiManager.ShowPenInfo(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && uiManager != null)
        {
            uiManager.ShowPenInfo(false);
        }
    }
}
