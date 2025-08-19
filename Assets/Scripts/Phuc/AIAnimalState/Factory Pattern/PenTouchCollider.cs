using UnityEngine;

public class PenTouchCollider : MonoBehaviour
{
    public AnimalPen pen;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pen.ShowPenInfo(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pen.ShowPenInfo(false);
        }
    }
}
