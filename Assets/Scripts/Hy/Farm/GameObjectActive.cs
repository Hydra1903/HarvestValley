using UnityEditor.EditorTools;
using UnityEngine;

public class GameObjectActive : MonoBehaviour
{
    public static GameObjectActive Instance { get; private set; }

    [Header("VFX")]
    [SerializeField] private GameObject fxRain;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
    
        //if (Weather.Instance.currentWeather == WeatherState.Rainy || Weather.Instance.currentWeather == WeatherState.Stormy)
        //{
        //    fxRain.SetActive(true);
        //}
        //else
        //{
        //    fxRain.SetActive(false);
        //}
    }
    
    public void SetActiveByName(string objectName, bool state)
    {
        GameObject obj = GameObject.Find(objectName);
        if (obj != null)
        {
            obj.SetActive(state);
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy object có tên: {objectName}");
        }
    }
}
