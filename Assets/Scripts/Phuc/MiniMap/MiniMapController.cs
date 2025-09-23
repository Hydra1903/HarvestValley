using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class MinimapIconPair
{
    public MinimapIcon1 miniIcon;
    public MinimapIcon1 fullIcon;

    public MinimapIconPair(MinimapIcon1 mini, MinimapIcon1 full)
    {
        miniIcon = mini;
        fullIcon = full;
    }
}

public enum MinimapMode
{
    Mini, Fullscreen
}
public class MiniMapController : MonoBehaviour
{
    public static MiniMapController Instance;

    [Header("Map Size and Dimension Size")]
    [SerializeField]
    Vector2 worldSize;

    [SerializeField]
    Vector2 fullScreenDimensions = new Vector2(1000, 1000);
    [Header("Zoom Map")]
    [SerializeField]
    float zoomSpeed = 0.1f;
    [SerializeField]
    float maxZoom = 10f;
    [SerializeField]
    float minZoom = 1f;

    [Header("Transform and Panel")]
    [SerializeField]RectTransform scrollViewRectTransform;
    [SerializeField] RectTransform miniMapContent;
    [SerializeField] RectTransform fullMapContent;
    [SerializeField] GameObject fullMapPanel;

    [SerializeField]
    MinimapIcon1 minimapIconPrefab;

    Matrix4x4 transformationMatrix;

    private MinimapMode currentMiniMapMode = MinimapMode.Mini;
    private MinimapIcon1 followIcon;
    private Vector2 scrollViewDefaultSize;
    private Vector2 scrollViewDefaultPosition;
    Dictionary<MinimapWorld, MinimapIconPair> miniMapWorldObjectsLookup = new Dictionary<MinimapWorld, MinimapIconPair>();
    private void Awake()
    {
        Instance = this;
        scrollViewDefaultSize = scrollViewRectTransform.sizeDelta;
        scrollViewDefaultPosition = scrollViewRectTransform.anchoredPosition;
    }

    private void Start()
    {
        CalculateTransformationMatrix();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            fullMapPanel.SetActive(!fullMapPanel.activeSelf);
        }

        float zoom = Input.GetAxis("Mouse ScrollWheel");
        ZoomMap(zoom);
        UpdateMiniMapIcons();
        UpdateFullMapIcons();
        CenterMapOnIcon();
    }

    public void RegisterMinimapWorldObject(MinimapWorld miniMapWorldObject, bool followObject = false)
    {
        // Icon cho minimap
        var minimapIcon = Instantiate(minimapIconPrefab, miniMapContent);
        minimapIcon.Image.sprite = miniMapWorldObject.MinimapIcon;

        // ép size c? ð?nh 55x55
        minimapIcon.RectTransform.sizeDelta = new Vector2(61, 61);

        // Icon cho fullmap
        var fullmapIcon = Instantiate(minimapIconPrefab, fullMapContent);
        fullmapIcon.Image.sprite = miniMapWorldObject.MinimapIcon;
        fullmapIcon.RectTransform.sizeDelta = new Vector2(0.5f, 0.5f); // gi? nh? cho b?n ð? l?n

        // G?p vào dictionary
        miniMapWorldObjectsLookup[miniMapWorldObject] = new MinimapIconPair(minimapIcon, fullmapIcon);

        if (followObject)
            followIcon = minimapIcon;
    }

    public void RemoveMinimapWorldObject(MinimapWorld minimapWorldObject)
    {
        if (miniMapWorldObjectsLookup.TryGetValue(minimapWorldObject, out MinimapIconPair pair))
        {
            miniMapWorldObjectsLookup.Remove(minimapWorldObject);
            Destroy(pair.miniIcon.gameObject);
            Destroy(pair.fullIcon.gameObject);
        }
    }



    private Vector2 halfVector2 = new Vector2(0.5f, 0.5f);
    public void SetMinimapMode(MinimapMode mode)
    {
        const float defaultScaleWhenFullScreen = 1.3f;

        if (mode == currentMiniMapMode)
            return;

        switch (mode)
        {
            case MinimapMode.Mini:
                scrollViewRectTransform.sizeDelta = scrollViewDefaultSize;
                scrollViewRectTransform.anchorMin = Vector2.one;
                scrollViewRectTransform.anchorMax = Vector2.one;
                scrollViewRectTransform.pivot = Vector2.one;
                scrollViewRectTransform.anchoredPosition = scrollViewDefaultPosition;
                currentMiniMapMode = MinimapMode.Mini;
                break;
            case MinimapMode.Fullscreen:
                scrollViewRectTransform.sizeDelta = fullScreenDimensions;
                scrollViewRectTransform.anchorMin = halfVector2;
                scrollViewRectTransform.anchorMax = halfVector2;
                scrollViewRectTransform.pivot = halfVector2;
                scrollViewRectTransform.anchoredPosition = Vector2.zero;
                currentMiniMapMode = MinimapMode.Fullscreen;
                miniMapContent.transform.localScale = Vector3.one * defaultScaleWhenFullScreen;
                break;
        }
    }

    private void ZoomMap(float zoom)
    {
        if (zoom == 0)
            return;
        //lãn chu?t ð? zoom map ? v? trí c? ð?nh c?a nhân v?t
        float currentMapScale = miniMapContent.localScale.x;
        float zoomAmount = (zoom > 0 ? zoomSpeed : -zoomSpeed) * currentMapScale;
        float newScale = currentMapScale + zoomAmount;
        float clampedScale = Mathf.Clamp(newScale, minZoom, maxZoom);
        miniMapContent.localScale = Vector3.one * clampedScale;
    }

    private void CenterMapOnIcon()
    {
        if (followIcon != null)
        {
            float mapScale = miniMapContent.transform.localScale.x;
            miniMapContent.anchoredPosition = (-followIcon.RectTransform.anchoredPosition * mapScale);
        }
    }

    private void UpdateMiniMapIcons()
    {
        float iconScale = 1 / miniMapContent.transform.localScale.x;

        foreach (var kvp in miniMapWorldObjectsLookup)
        {
            var miniMapWorldObject = kvp.Key;
            var pair = kvp.Value;

            var mapPosition = WorldPositionToMapPosition(miniMapWorldObject.transform.position);

            pair.miniIcon.RectTransform.anchoredPosition = mapPosition;
            pair.miniIcon.IconRectTransform.localScale = Vector3.one * iconScale;

            pair.fullIcon.RectTransform.anchoredPosition = mapPosition;
            pair.fullIcon.IconRectTransform.localRotation = Quaternion.identity;
        }
    }

    private void UpdateFullMapIcons()
    {
        foreach (var kvp in miniMapWorldObjectsLookup)
        {
            var miniMapWorldObject = kvp.Key;
            var fullmapIcon = fullMapContent.GetComponentInChildren<MinimapIcon1>(); 
            if (fullmapIcon == null) continue;

            var mapPosition = WorldPositionToMapPosition(miniMapWorldObject.transform.position);
            fullmapIcon.RectTransform.anchoredPosition = mapPosition;
            fullmapIcon.IconRectTransform.localRotation = Quaternion.identity;
        }
    }
    private Vector2 WorldPositionToMapPosition(Vector3 worldPos)
    {
        var pos = new Vector2(worldPos.x, worldPos.z);
        return transformationMatrix.MultiplyPoint3x4(pos);
    }


    private void CalculateTransformationMatrix()
    {
        var minimapSize = miniMapContent.rect.size;
        var halfWorld = worldSize / 2f;

        var translation = -minimapSize / 2;
        var scaleRatio = minimapSize / worldSize;

        transformationMatrix = Matrix4x4.TRS(translation + minimapSize / 2, Quaternion.identity, scaleRatio);
    }
}