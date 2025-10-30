using System;

[Serializable]
public class HayCellData
{
    public int penId;      // ID của chuồng chứa ô này
    public int[] quantities = new int[2]; // số lượng hay trong ô (hoặc 0 nếu trống)
}
