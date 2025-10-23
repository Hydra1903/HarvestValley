using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public Slider loadingBar;
    public void ShowSceneLoading()
    {
        UIManager.Instance.ShowUI("Loading");
    }
    public void LoadScene()
    {
        CharacterSelection.Instance.SelectCharacter();
        SceneManager.LoadSceneAsync("Main");
    }
}
