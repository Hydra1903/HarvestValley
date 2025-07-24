using UnityEngine;

public class WhiteGoat : AnimalBaseFac
{
    public override void Speak()
    {
        Debug.Log("White Goat Spawned!");
        //Ham dua long cuu khi lai gan va nhan "E" de thu hoach khi dat du dieu kien thoi gian 3 ngay de thu hoach
        //Pressing E!
        Debug.Log("Giving Bucket Of Milk");
        //Time Check if(3 day) -> giving Bucket Of Milk vi thoa man dieu kien 3 ngay + an uong day du
        //if(!3 day)qua 4 5...n day -> neu van cho an uong day du qua tung ngay thi van co the thu thap khi can
        //else -> khong the thu nhap vi khong cho an uong day du, Warning "cho an ngay!"
        //-> neu van khong cho an uong sau 1 ngay -> Cuu chet, Destroy gameobject WhiteGoat
    }
}
