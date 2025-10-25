using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public static Loading Instance;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    public void ShowScene1to2Loading()
    {
        UIManager.Instance.ShowUI("Loading");
    }
    public void LoadScene1to2()
    {
        SceneManager.LoadSceneAsync("Main");
    }

    public void ShowScene2to1Loading()
    {
        UIManager.Instance.ShowUI("Loading");
    }
    public void LoadScene2to1()
    {
        SceneManager.LoadSceneAsync("Main2");
    }
}
