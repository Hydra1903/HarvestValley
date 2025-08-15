using UnityEngine;

public class GameController : MonoBehaviour
{
    public FarmGrid[] farmGrids;

    [ContextMenu("Save Game")]
    public void SaveGame() => SaveManager.Save("slot1", farmGrids);

    [ContextMenu("Load Game")]
    public void LoadGame() => SaveManager.Load("slot1", farmGrids);
}
