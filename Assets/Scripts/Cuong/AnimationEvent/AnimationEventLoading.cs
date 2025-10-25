using UnityEngine;

public class AnimationEventLoading : MonoBehaviour
{
    public void OnLoadScene1To2Event()
    {
        Loading.Instance.LoadScene1to2();
    }
    public void OnLoadScene2To1Event()
    {
        Loading.Instance.LoadScene2to1();
    }
}

