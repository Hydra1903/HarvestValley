using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Collections;
using static UnityEngine.Rendering.HDROutputUtils;

public class Loading : MonoBehaviour
{
    public LoadingUI loadingUI;
    public AsyncOperation operation;

    public void LoadScene()
    {
        //StartCoroutine(LoadSceneCoroutine());
        SceneManager.LoadSceneAsync("Hy");
    }

    private IEnumerator LoadSceneCoroutine()
    {

        operation = SceneManager.LoadSceneAsync("Cuong");
        operation.allowSceneActivation = false; 


        while (operation.progress < 0.9f)
        {
            loadingUI.loadingBar.value = operation.progress;
            yield return null;
        }

        loadingUI.loadingBar.value = 1f;
        yield return new WaitForSeconds(0.5f);
        operation.allowSceneActivation = true;
    }
    private void Update()
    {
        Debug.Log(operation.progress);
    }
}
