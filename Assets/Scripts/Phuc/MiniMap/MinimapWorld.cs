using UnityEngine;

public class MinimapWorld : MonoBehaviour
{
    [SerializeField]
    private bool followObject = false;
    [SerializeField]
    private Sprite[] minimapIcon;
    public Sprite MinimapIcon;
    public bool isPlayer;


    private void Start()
    {
        SetIconMap();
        MiniMapController.Instance.RegisterMinimapWorldObject(this, followObject);
    }

    private void OnDestroy()
    {
        if (MiniMapController.Instance != null)
            MiniMapController.Instance.RemoveMinimapWorldObject(this);
    }
    public void SetIconMap()
    {
        switch (CharacterStateMachine.Instance.currentCharacter)
        {
            case ECharacter.Rin:
                MinimapIcon = minimapIcon[0];
                break;
            case ECharacter.May:
                MinimapIcon = minimapIcon[1];
                break;
            case ECharacter.Kai:
                MinimapIcon = minimapIcon[2];
                break;
            case ECharacter.Max:
                MinimapIcon = minimapIcon[3];
                break;
            case ECharacter.Hana:
                MinimapIcon = minimapIcon[4];
                break;
            case ECharacter.Leon:
                MinimapIcon = minimapIcon[5];
                break;
        }
    }
}
