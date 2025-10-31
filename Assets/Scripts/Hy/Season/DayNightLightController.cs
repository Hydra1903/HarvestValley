using UnityEngine;

[ExecuteAlways] // cho phép xem thay đổi trong Editor
public class DayNightLightController : MonoBehaviour
{
    [Header("Tham chiếu")]
    public Light mainLight; // Directional Light trong scene

    [Header("Màu sắc ánh sáng trong ngày")]
    public Color dawnColor = new Color(1f, 0.8f, 0.6f);      // Bình minh
    public Color dayColor = new Color(1f, 1f, 0.95f);         // Ban ngày
    public Color eveningColor = new Color(1f, 0.7f, 0.5f);    // Hoàng hôn
    public Color nightColor = new Color(0.3f, 0.4f, 0.6f);    // Ban đêm

    [Header("Cường độ ánh sáng")]
    public float dayIntensity = 1.5f;
    public float nightIntensity = 0.2f;

    [Header("Góc quay mặt trời (0h = 0°, 24h = 360°)")]
    public float rotationSpeed = 15f; 

    private void Start()
    {
        if (!mainLight) mainLight = GetComponent<Light>();
    }

    void Update()
    {
        if (!mainLight || GameTime.Instance == null) return;

        int hour = GameTime.Instance.hour;
        float t = (hour + GameTime.Instance.minute / 60f) / 24f; // tỷ lệ 0–1 theo thời gian trong ngày

        // Quay mặt trời theo thời gian trong ngày
        mainLight.transform.rotation = Quaternion.Euler(new Vector3((t * 360f) - 60f, 170f, 0f));

        // Tính màu và độ sáng theo giờ
        if (hour >= 5 && hour < 8)
            SetLight(dawnColor, Mathf.Lerp(nightIntensity, dayIntensity, (hour - 5f) / 3f));
        else if (hour >= 8 && hour < 17)
            SetLight(dayColor, dayIntensity);
        else if (hour >= 17 && hour < 19)
            SetLight(eveningColor, Mathf.Lerp(dayIntensity, nightIntensity, (hour - 17f) / 2f));
        else
            SetLight(nightColor, nightIntensity);
    }

    void SetLight(Color color, float intensity)
    {
        mainLight.color = color;
        mainLight.intensity = intensity;
    }
}
