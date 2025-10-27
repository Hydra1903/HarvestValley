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
    [SerializeField] Vector2 fullScreenDimensions;

    [Header("Zoom Map")]
    //[SerializeField] float zoomSpeed = 0.1f;
    //[SerializeField] float maxZoom = 10f;
    //[SerializeField] float minZoom = 1f;
    //[SerializeField] bool scaleFullmapIconsWithZoom = false;
    private Vector3 defaultPosition; 
    private Vector3 defaultScale;

    [Header("Transform and Panel")]
    [SerializeField] RectTransform scrollViewRectTransform;
    [SerializeField] RectTransform miniMapContent;
    [SerializeField] RectTransform fullMapContent;
    [SerializeField] GameObject fullMapPanel;

    [SerializeField] MinimapIcon1 minimapIconPrefab;

    //Matrix tinh map
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

    private IEnumerator Start()
    {
        yield return null;
        CalculateTransformationMatrix();
        defaultPosition = fullMapContent.anchoredPosition;
        defaultScale = fullMapContent.localScale;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            fullMapPanel.SetActive(!fullMapPanel.activeSelf);

            if (fullMapPanel.activeSelf)
            {
                SetMinimapMode(MinimapMode.Fullscreen);
            }
            else
            {
                SetMinimapMode(MinimapMode.Mini);
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
        minimapIcon.RectTransform.sizeDelta = new Vector2(70, 70); // nho hõn

        // Icon cho fullmap
        var fullmapIcon = Instantiate(minimapIconPrefab, fullMapContent);
        fullmapIcon.Image.sprite = miniMapWorldObject.MinimapIcon;
        fullmapIcon.RectTransform.sizeDelta = new Vector2(60, 60); // to hõn

        miniMapWorldObjectsLookup[miniMapWorldObject] = new MinimapIconPair(minimapIcon, fullmapIcon);

        if (followObject)
        {
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
        //const float defaultScaleWhenFullScreen = 1.3f;

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
                break;
        }
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

        Vector2 miniSize = miniMapContent.rect.size / 2.223f;
        foreach (var kvp in miniMapWorldObjectsLookup)
        {
            var miniMapWorldObject = kvp.Key;
            var pair = kvp.Value;

            var mapPosition = WorldToMiniMapPosition(miniMapWorldObject.transform.position);

            mapPosition.x = Mathf.Clamp(mapPosition.x, -miniSize.x, miniSize.x);
            mapPosition.y = Mathf.Clamp(mapPosition.y, -miniSize.y, miniSize.y);

            pair.miniIcon.RectTransform.anchoredPosition = mapPosition;
            pair.miniIcon.RectTransform.localScale = Vector3.one * scaleFactor;
            if (miniMapWorldObject.isPlayer && pair.miniIcon.ViewCone != null)
            {
                float angle = miniMapWorldObject.transform.eulerAngles.y + 180f;
                pair.miniIcon.ViewCone.localRotation = Quaternion.Euler(0, 0, -angle);
            }
            if (pair.miniIcon.ViewCone != null)
            {
                pair.miniIcon.ViewCone.localScale = Vector3.one * scaleFactor;
                pair.miniIcon.ViewCone.anchoredPosition = Vector2.zero;
            }
        }
    }
    private void UpdateFullMapIcons()
    {
        Vector2 fullSize = fullMapContent.rect.size / 2.223f;
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
            if (miniMapWorldObject.isPlayer && pair.fullIcon.ViewCone != null)
            {
                float angle = miniMapWorldObject.transform.eulerAngles.y + 180f;
                pair.fullIcon.ViewCone.localRotation = Quaternion.Euler(0, 0, -angle);
            }
            if (pair.fullIcon.ViewCone != null)
            {
                pair.fullIcon.ViewCone.localScale = Vector3.one;
                pair.fullIcon.ViewCone.anchoredPosition = Vector2.zero;
            }
        }
    }
    private Vector2 WorldToMiniMapPosition(Vector3 worldPos)
    {
        float worldMinX = -100f;
        float worldMaxX = 80f;
        float worldMinZ = -25f;
        float worldMaxZ = 135f;

        Vector2 miniMapSize = miniMapContent.rect.size;

        float normalizedX = Mathf.InverseLerp(worldMinX, worldMaxX, worldPos.x);
        float normalizedZ = Mathf.InverseLerp(worldMinZ, worldMaxZ, worldPos.z);

        float mapX = Mathf.Lerp(miniMapSize.x / 2.2f, -miniMapSize.x / 2.2f, normalizedX);
        float mapY = Mathf.Lerp(miniMapSize.y / 2.287f, -miniMapSize.y / 2.4f, normalizedZ);

        return new Vector2(mapX, mapY);
    }
    
    private Vector2 WorldToFullMapPosition(Vector3 worldPos)
    {
         float worldMinX = -100f;
        float worldMaxX = 80f;
        float worldMinZ = -30f;
        float worldMaxZ = 135f;

        Vector2 fullMapSize = fullMapContent.rect.size;

        float normalizedX = Mathf.InverseLerp(worldMinX, worldMaxX, worldPos.x);
        float normalizedZ = Mathf.InverseLerp(worldMinZ, worldMaxZ, worldPos.z);

        float mapX = Mathf.Lerp(fullMapSize.x / 2.2f, -fullMapSize.x / 2.2f, normalizedX);
        float mapY = Mathf.Lerp(fullMapSize.y / 2.287f, -fullMapSize.y / 2.4f, normalizedZ);

        return new Vector2(mapX, mapY);
    }
    private void CalculateTransformationMatrix()
    {
        float worldMinX = -100;
        float worldMaxX = 100;
        float worldMinZ = -100;
        float worldMaxZ = 100;

        float worldWidth = worldMaxX - worldMinX;
        float worldHeight = worldMaxZ - worldMinZ;

        worldSize = new Vector2(worldWidth, worldHeight);
        Vector2 worldCenter = new Vector2((worldMaxX + worldMinX) / 2f, (worldMaxZ + worldMinZ) / 2f);

        // Mini map
        var miniSize = miniMapContent.rect.size;
        var miniScale = new Vector3(miniSize.x / worldWidth, miniSize.y / worldHeight, 1);
        var miniTranslation = -worldCenter * miniScale + (Vector2)miniSize / 2f;
        miniMapMatrix = Matrix4x4.TRS(miniTranslation, Quaternion.identity, miniScale);

        // Full map
        var fullSize = fullMapContent.rect.size;
        var fullScale = new Vector3(fullSize.x / worldWidth, fullSize.y / worldHeight, 1);
        var fullTranslation = -worldCenter * fullScale + (Vector2)fullSize / 2f;
        fullMapMatrix = Matrix4x4.TRS(fullTranslation, Quaternion.identity, fullScale);
    }

}
