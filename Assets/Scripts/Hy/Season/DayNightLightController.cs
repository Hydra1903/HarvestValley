using UnityEngine;

public class DayNightLightController : MonoBehaviour
{
    public Light mainLight;

    [Header("Cấu hình thời gian")]
    [Range(0f, 24f)] public float editorPreviewHour;  // để thử trong Editor khi GameTime chưa chạy
    public bool useGameTime = true;                   // lấy từ GameTime.Instance
    public float smoothRotTime = 0.25f;               // lớn hơn → mượt hơn (0.15–0.4)
    public float smoothIntTime = 0.35f;               // mượt cường độ
    public float smoothColTime = 0.35f;               // mượt màu

    [Header("Quỹ đạo mặt trời")]
    public float azimuth = 170f;                      // hướng đông-tây (trục Y)
    public float dawnOffsetDeg = -90f;                // 0h = -90° (dưới chân trời)
    public AnimationCurve elevationCurve = AnimationCurve.Linear(0, -10, 1, 370); // tùy chọn

    [Header("Cường độ theo thời gian (0..1 là 0h..24h)")]
    public AnimationCurve intensityCurve = new AnimationCurve(
        new Keyframe(0.00f, 0.10f),
        new Keyframe(0.22f, 1.00f), // ~5h30 sáng
        new Keyframe(0.70f, 1.00f), // ~16h50
        new Keyframe(0.83f, 0.10f), // ~20h
        new Keyframe(1.00f, 0.10f)
    );
    public float maxIntensity = 1.6f;

    [Header("Màu theo thời gian")]
    public Gradient lightColor = new Gradient
    {
        colorKeys = new[] {
            new GradientColorKey(new Color(0.30f,0.40f,0.60f), 0.00f), // đêm
            new GradientColorKey(new Color(1.00f,0.75f,0.55f), 0.23f), // bình minh
            new GradientColorKey(new Color(1.00f,0.98f,0.92f), 0.50f), // trưa
            new GradientColorKey(new Color(1.00f,0.70f,0.50f), 0.78f), // hoàng hôn
            new GradientColorKey(new Color(0.30f,0.40f,0.60f), 1.00f)  // đêm
        },
        alphaKeys = new[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 1) }
    };

    // trạng thái mượt
    private float _velPitch, _velIntensity;
    private Color _currentColor;

    void Reset() { mainLight = GetComponent<Light>(); }

    void Update()
    {
        if (!mainLight) mainLight = GetComponent<Light>();
        if (!mainLight) return;

        // 0..1 trong ngày
        float tDay = GetDayTime01();
        // Góc cao mặt trời (pitch) từ 0..1 → 0..360 (hoặc dùng curve của bạn)
        float targetPitch = (tDay * 360f) + dawnOffsetDeg;   // quay 1 vòng / ngày
        // Bạn có thể dùng elevationCurve để nắn đường cong nếu thích:
        // float targetPitch = elevationCurve.Evaluate(tDay);

        // --- Smooth rotation (mượt)
        // Lấy góc hiện tại theo trục X:
        float currX = mainLight.transform.eulerAngles.x;
        float smoothedX = Mathf.SmoothDampAngle(currX, targetPitch, ref _velPitch, smoothRotTime);

        // Giữ hướng phương vị (azimuth) cố định để màu trời ổn định
        mainLight.transform.rotation = Quaternion.Euler(smoothedX, azimuth, 0f);

        // --- Smooth intensity
        float targetIntensity = intensityCurve.Evaluate(tDay) * maxIntensity;
        float smoothedIntensity = Mathf.SmoothDamp(mainLight.intensity, targetIntensity, ref _velIntensity, smoothIntTime);
        mainLight.intensity = smoothedIntensity;

        // --- Smooth color
        Color targetColor = lightColor.Evaluate(tDay);
        // Lerp theo thời gian thực để mượt:
        _currentColor = Color.Lerp(_currentColor.a == 0 ? targetColor : _currentColor, targetColor, 1f - Mathf.Exp(-Time.deltaTime / Mathf.Max(0.0001f, smoothColTime)));
        mainLight.color = _currentColor;

        // (tuỳ chọn) Ambient theo cường độ
        RenderSettings.ambientLight = Color.Lerp(new Color(0.06f, 0.07f, 0.10f), targetColor, smoothedIntensity / Mathf.Max(0.001f, maxIntensity));
    }

    float GetDayTime01()
    {
        if (Application.isPlaying && useGameTime && GameTime.Instance)
        {
            float h = GameTime.Instance.hour;
            float m = GameTime.Instance.minute;
            float s = GameTime.Instance.timeSpeed;
            return Mathf.Repeat((h + m / 60f + s / 3600f) / 24f, 1f);
        }
        else
        {
            // Preview trong Editor
            return Mathf.Repeat(editorPreviewHour / 24f, 1f);
        }
    }
}
