using UnityEngine;

public class GameController : MonoBehaviour
{
    public FarmManager[] farms;
    public InventoryUI inventoryUI;

    [ContextMenu("Save Game")]
    public void SaveGame() => SaveManager.Save("slot1", farms);

    [ContextMenu("Load Game")]
    public void LoadGame() => SaveManager.Load("slot1", farms);

    private void Awake()
    {
        SaveManager.Load("slot1", farms);
        Debug.Log("Load game");
    }
}
