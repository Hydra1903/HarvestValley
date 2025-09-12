using UnityEngine;

public class Sprinkler : MonoBehaviour
{
    public int gridX;
    public int gridY;
    public int halfRange = 7; 

    public void Init(int gx, int gy, int half)
    {
        gridX = gx;
        gridY = gy;
        halfRange = half;
    }
}
