using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
        SaveManager.Load("slot1", farms);
        Debug.Log("Load game");
    }
    public FarmManager[] farms;

    [ContextMenu("Save Game")]
   public void SaveGame() => SaveManager.Save("slot1", farms);

    [ContextMenu("Load Game")]
    public void LoadGame() => SaveManager.Load("slot1", farms);

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            foreach (var f in farms)
                f.plantManager.CheckNextDay();
            GameTime.Instance.NextDay();
        }
    }
}
