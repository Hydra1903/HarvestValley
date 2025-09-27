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
    [SerializeField] Vector2 worldSize;
    [SerializeField] Vector2 fullScreenDimensions = new Vector2(1000, 1000);

    [Header("Zoom Map")]
    [SerializeField] float zoomSpeed = 0.1f;
    [SerializeField] float maxZoom = 10f;
    [SerializeField] float minZoom = 1f;
    [SerializeField] bool scaleFullmapIconsWithZoom = false;
    private Vector3 defaultPosition; 
    private Vector3 defaultScale;

    [Header("Transform and Panel")]
    [SerializeField] RectTransform scrollViewRectTransform;
    [SerializeField] RectTransform miniMapContent;
    [SerializeField] RectTransform fullMapContent;
    [SerializeField] GameObject fullMapPanel;

    [SerializeField] MinimapIcon1 minimapIconPrefab;

    // thay v? 1 matrix, tách ra 2 matrix riêng
    private Matrix4x4 miniMapMatrix;
    private Matrix4x4 fullMapMatrix;

    private MinimapMode currentMiniMapMode = MinimapMode.Mini;
    private MinimapIcon1 followIcon;
    private MinimapIcon1 followMiniIcon;
    private MinimapIcon1 followFullIcon;
    private Vector2 scrollViewDefaultSize;
    private Vector2 scrollViewDefaultPosition;

    Dictionary<MinimapWorld, MinimapIconPair> miniMapWorldObjectsLookup = new Dictionary<MinimapWorld, MinimapIconPair>();

    private void Awake()
    {
        Instance = this;
        scrollViewDefaultSize = scrollViewRectTransform.sizeDelta;
        scrollViewDefaultPosition = scrollViewRectTransform.anchoredPosition;
        if (fullMapPanel != null)
        {
            fullMapPanel.SetActive(false);
        }
    }

    private void Start()
    {
        CalculateTransformationMatrix();
        defaultPosition = fullMapContent.anchoredPosition;
        defaultScale = fullMapContent.localScale;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            fullMapPanel.SetActive(!fullMapPanel.activeSelf);

            // chuy?n mode ð? phân bi?t zoom
            if (fullMapPanel.activeSelf)
                SetMinimapMode(MinimapMode.Fullscreen);
            else
                SetMinimapMode(MinimapMode.Mini);
        }

        // ch? cho zoom khi ðang m? fullmap
        if (currentMiniMapMode == MinimapMode.Fullscreen && fullMapPanel.activeSelf)
        {
            float zoom = Input.GetAxis("Mouse ScrollWheel");
            if (zoom != 0)
            {
                ZoomMap(zoom);
            }
        }

        UpdateMiniMapIcons();
        UpdateFullMapIcons();
        CenterMapOnIcon();
    }
    public void RegisterMinimapWorldObject(MinimapWorld miniMapWorldObject, bool followObject = false)
    {
        // Icon cho minimap
        var minimapIcon = Instantiate(minimapIconPrefab, miniMapContent);
        minimapIcon.Image.sprite = miniMapWorldObject.MinimapIcon;
        minimapIcon.RectTransform.sizeDelta = new Vector2(60, 60); // nh? hõn

        // Icon cho fullmap
        var fullmapIcon = Instantiate(minimapIconPrefab, fullMapContent);
        fullmapIcon.Image.sprite = miniMapWorldObject.MinimapIcon;
        fullmapIcon.RectTransform.sizeDelta = new Vector2(42, 42); // to hõn

        miniMapWorldObjectsLookup[miniMapWorldObject] = new MinimapIconPair(minimapIcon, fullmapIcon);

        if (followObject)
        {
            // follow luôn c? 2 icon
            followMiniIcon = minimapIcon;
            followFullIcon = fullmapIcon;
        }
    }


    public void RemoveMinimapWorldObject(MinimapWorld minimapWorldObject)
    {
        if (miniMapWorldObjectsLookup.TryGetValue(minimapWorldObject, out MinimapIconPair pair))
        {
            miniMapWorldObjectsLookup.Remove(minimapWorldObject);

            if (pair.miniIcon != null && pair.miniIcon.gameObject != null)
                Destroy(pair.miniIcon.gameObject);

            if (pair.fullIcon != null && pair.fullIcon.gameObject != null)
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
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;

            case MinimapMode.Fullscreen:
                scrollViewRectTransform.sizeDelta = fullScreenDimensions;
                scrollViewRectTransform.anchorMin = halfVector2;
                scrollViewRectTransform.anchorMax = halfVector2;
                scrollViewRectTransform.pivot = halfVector2;
                scrollViewRectTransform.anchoredPosition = Vector2.zero;
                currentMiniMapMode = MinimapMode.Fullscreen;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
        }
    }

    private void ZoomMap(float zoom)
    {
        if (zoom == 0) return;

        float currentScale = fullMapContent.localScale.x;
        float zoomAmount = (zoom > 0 ? zoomSpeed : -zoomSpeed) * currentScale;
        float newScale = currentScale + zoomAmount;
        float clampedScale = Mathf.Clamp(newScale, minZoom, maxZoom);
        if (Mathf.Approximately(clampedScale, minZoom))
        {
            fullMapContent.localScale = defaultScale;
            fullMapContent.anchoredPosition = defaultPosition;
            fullMapContent.pivot = new Vector2(0.5f, 0.5f);
            return;
        }
        Vector2 localMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            fullMapContent, Input.mousePosition, null, out localMousePos
        );
        Vector2 pivot = new Vector2(
            (localMousePos.x / fullMapContent.rect.width) + 0.5f,
            (localMousePos.y / fullMapContent.rect.height) + 0.5f
        );

        fullMapContent.pivot = pivot;
        Vector2 offset = -localMousePos * (clampedScale - currentScale);
        fullMapContent.anchoredPosition += offset;
        fullMapContent.localScale = Vector3.one * clampedScale;
    }
    private void CenterMapOnIcon()
    {
        if (followMiniIcon != null)
        {
            float mapScale = miniMapContent.transform.localScale.x;
            miniMapContent.anchoredPosition = (-followMiniIcon.RectTransform.anchoredPosition * mapScale);
        }
    }

    private void UpdateMiniMapIcons()
    {
        float mapScale = miniMapContent.localScale.x;
        float scaleFactor = 1f / mapScale; 

        Vector2 miniSize = miniMapContent.rect.size / 2f;
        foreach (var kvp in miniMapWorldObjectsLookup)
        {
            var miniMapWorldObject = kvp.Key;
            var pair = kvp.Value;

            var mapPosition = WorldToMiniMapPosition(miniMapWorldObject.transform.position);

            mapPosition.x = Mathf.Clamp(mapPosition.x, -miniSize.x, miniSize.x);
            mapPosition.y = Mathf.Clamp(mapPosition.y, -miniSize.y, miniSize.y);

            pair.miniIcon.RectTransform.anchoredPosition = mapPosition;
            pair.miniIcon.RectTransform.localScale = Vector3.one * scaleFactor; 
        }
    }

    private void UpdateFullMapIcons()
    {
        Vector2 fullSize = fullMapContent.rect.size / 2f;

        foreach (var kvp in miniMapWorldObjectsLookup)
        {
            var miniMapWorldObject = kvp.Key;
            var pair = kvp.Value;

            var mapPosition = WorldToFullMapPosition(miniMapWorldObject.transform.position);

            mapPosition.x = Mathf.Clamp(mapPosition.x, -fullSize.x, fullSize.x);
            mapPosition.y = Mathf.Clamp(mapPosition.y, -fullSize.y, fullSize.y);

            pair.fullIcon.RectTransform.anchoredPosition = mapPosition;
            pair.fullIcon.RectTransform.localScale = Vector3.one;
            pair.fullIcon.IconRectTransform.localRotation = Quaternion.identity;
        }
    }


    private Vector2 WorldToMiniMapPosition(Vector3 worldPos)
    {
        var pos = new Vector2(worldPos.x, worldPos.z);
        return miniMapMatrix.MultiplyPoint3x4(pos);
    }

    private Vector2 WorldToFullMapPosition(Vector3 worldPos)
    {
        var pos = new Vector2(worldPos.x, worldPos.z);
        return fullMapMatrix.MultiplyPoint3x4(pos);
    }

    private void CalculateTransformationMatrix()
    {
        // matrix cho minimap
        var miniSize = miniMapContent.rect.size;
        var miniTranslation = -miniSize / 2;
        var miniScale = miniSize / worldSize;
        miniMapMatrix = Matrix4x4.TRS(miniTranslation + miniSize / 2, Quaternion.identity, miniScale);

        // matrix cho fullmap
        var fullSize = fullMapContent.rect.size;
        var fullTranslation = -fullSize / 2;
        var fullScale = fullSize / worldSize;
        fullMapMatrix = Matrix4x4.TRS(fullTranslation + fullSize / 2, Quaternion.identity, fullScale);
    }
}
