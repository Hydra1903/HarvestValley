using UnityEngine;

public class Pause : MonoBehaviour
{
    public void BackToMainMenu()
    {
        Loading.Instance.ShowScene2to1Loading();
    }
}
