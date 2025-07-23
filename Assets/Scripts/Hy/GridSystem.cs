using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public GameObject objectToPlace;
    public float gridSize = 1f;

    private GameObject ghostObject;
    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();

    private int groundLayerMask;

    private void Start()
    {
        // Chỉ raycast vào layer "Ground"
        groundLayerMask = LayerMask.GetMask("Ground");

        CreateGhostObject();
    }

    private void Update()
    {
        UpdateGhostPosition();

        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceObject();
        }
    }

    void CreateGhostObject()
    {
        ghostObject = Instantiate(objectToPlace);
        ghostObject.name = "GhostObject";

        // Đặt vào layer "Ghost"
        ghostObject.layer = LayerMask.NameToLayer("Ghost");

        // Vô hiệu hóa tất cả collider để ghost không bị raycast
        foreach (Collider col in ghostObject.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }

        // Làm trong suốt ghost object
        foreach (Renderer renderer in ghostObject.GetComponentsInChildren<Renderer>())
        {
            Material mat = renderer.material;
            Color color = mat.color;
            color.a = 0.5f;
            mat.color = color;

            mat.SetFloat("_Mode", 2);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.renderQueue = 3000;
        }
    }

    void UpdateGhostPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayerMask))
        {
            Vector3 point = hit.point;

            Vector3 snappedPosition = new Vector3(
                Mathf.Round(point.x / gridSize) * gridSize,
                Mathf.Round(point.y / gridSize) * gridSize,
                Mathf.Round(point.z / gridSize) * gridSize
            );

            ghostObject.transform.position = snappedPosition;

            if (occupiedPositions.Contains(snappedPosition))
                SetGhostColor(Color.red);   // Không thể đặt
            else
                SetGhostColor(Color.green); // Có thể đặt

            ghostObject.SetActive(true);
        }
        else
        {
            ghostObject.SetActive(false); // Không có chỗ đặt
        }
    }

    void SetGhostColor(Color color)
    {
        foreach (Renderer renderer in ghostObject.GetComponentsInChildren<Renderer>())
        {
            Material mat = renderer.material;
            mat.color = new Color(color.r, color.g, color.b, 0.5f);
        }
    }

    void TryPlaceObject()
    {
        Vector3 placementPosition = ghostObject.transform.position;

        if (!occupiedPositions.Contains(placementPosition))
        {
            Instantiate(objectToPlace, placementPosition, Quaternion.identity);
            occupiedPositions.Add(placementPosition);
        }
        else
        {
            Debug.Log("Đã có object ở vị trí này!");
        }
    }
}
