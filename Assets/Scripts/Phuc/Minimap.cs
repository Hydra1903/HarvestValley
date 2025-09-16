using UnityEngine;

public class Minimap : MonoBehaviour
{
    public Transform player;
    public Camera ThirdCamera;
    public Camera FirstCamera;

    public bool ReadyToRotate = true;
    public void Start()
    {
        Position();
    }
    private void LateUpdate()
    {
        if (player != null)
        {
            Position();
        }
    }
    void Position()
    {
        var newPos = player.position;
        newPos.y = transform.position.y;

        transform.position = newPos;
    }
}

