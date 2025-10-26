using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterInfo : MonoBehaviour
{
    public Sprite[] spriteCharacters;
    public Image ImageCharacter;
    public TextMeshProUGUI textName;
    public TextMeshProUGUI textMana;
    public TextMeshProUGUI textSkill;
    void Start()
    {
        SetSpriteCharacter();
    }
    public void SetSpriteCharacter()
    {
        switch (CharacterStateMachine.Instance.currentCharacter)
        {
            case ECharacter.Rin:
                ImageCharacter.sprite = spriteCharacters[0];
                textName.text = "Rin";
                textMana.text = "100/100";
                textSkill.text = "Tốc độ thao tác nhanh hơn <color=#34B4AF>20%</color>";
                break;
            case ECharacter.May:
                ImageCharacter.sprite = spriteCharacters[1];
                textName.text = "May";
                textMana.text = "85/85";
                textSkill.text = "Giảm <color=#34B4AF>10%</color> giá mua hạt giống";
                break;
            case ECharacter.Kai:
                ImageCharacter.sprite = spriteCharacters[2];
                textName.text = "Kai";
                textMana.text = "110/110";
                textSkill.text = "Nhận thêm <color=#34B4AF>8%</color> giá bán nông sản";
                break;
            case ECharacter.Max:
                ImageCharacter.sprite = spriteCharacters[3];
                textName.text = "Max";
                textMana.text = "100/100";
                textSkill.text = "Nhận thêm <color=#34B4AF>10%</color> kinh nghiệm";
                break;
            case ECharacter.Hana:
                ImageCharacter.sprite = spriteCharacters[4];
                textName.text = "Hana";
                textMana.text = "90/90";
                textSkill.text = "Tỉ lệ <color=#34B4AF>15%</color> nhận thêm khi thu hoạch";
                break;
            case ECharacter.Leon:
                ImageCharacter.sprite = spriteCharacters[5];
                textName.text = "Leon";
                textMana.text = "130/130";
                textSkill.text = "Tỉ lệ <color=#34B4AF>20%</color> thao tác không tốn thể lực";
                break;
        }
    }
}
