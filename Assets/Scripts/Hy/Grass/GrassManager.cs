using System.Collections;
using UnityEngine;

public class GrassManager : MonoBehaviour
{
    [SerializeField] private int grassCount = 20;
    [SerializeField] private ItemData grassItem;
    [SerializeField] private float timeHarvest = 5f;

    private bool isHarvested = false;

    public bool HarvestGrass()
    {
        if (isHarvested) return false;

        CharacterStateMachine.Instance?.ChangeState(CharacterStateMachine.Instance.mowingState);

        // Add vào túi trước
        if (Inventory.Instance == null || !Inventory.Instance.AddItem(grassItem, grassCount))
        {
            Debug.LogWarning($"[Harvest] Túi đầy, không thể thu {grassCount} x {grassItem?.itemName}");
            return false;
        }
        isHarvested = true;

        StartCoroutine(Delay());
        Debug.Log($"Thu hoạch {grassCount} x {grassItem?.itemName}");
        return true;
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(timeHarvest);
        gameObject.SetActive(false);
    }
}
