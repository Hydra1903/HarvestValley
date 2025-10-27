using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public class GameStartManager : MonoBehaviour
{
    public StartScreenUI startScreenUI;
    public void ContinueGame()
    {
        if (SaveManager.IsHasFarm())
        {
            Loading.Instance.ShowScene1to2Loading();
        }
        else
        {
            Notification.Instance.ShowNotification("Bạn chưa có nông trại nào!");
        }
    }
    public void CreateNewFarm()
    {
        if (startScreenUI.nameFarm.text != "")
        {
            CharacterSelection.Instance.SelectCharacter();
            SaveManager.CreateFarm("Slot1", startScreenUI.nameFarm.text);
            SaveManager.SaveCharacter("Slot1");
            Loading.Instance.ShowScene1to2Loading();
        }
        else
        {
            Notification.Instance.ShowNotification("Chưa nhập tên nông trại!");
        }
        
    }
}
