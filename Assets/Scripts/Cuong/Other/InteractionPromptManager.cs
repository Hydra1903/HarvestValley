using UnityEngine;

public class InteractionPromptManager : MonoBehaviour
{
    public TriggerCanvas[] triggerCanvas;
    public static InteractionPromptManager Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    private void Update()
    {
        if (UIStateMachine.Instance.inputCooldown > 0)
        {
            UIStateMachine.Instance.inputCooldown -= Time.deltaTime;
            return;
        }
        if (Input.GetKeyDown(KeyCode.E) && UIStateMachine.Instance.currentState == UIStateMachine.Instance.mainScreenState)
        {
            for (int i = 0; i < triggerCanvas.Length; i++)
            {
                if (triggerCanvas[i].isPlayerNearby)
                {
                    CheckStateUI(triggerCanvas[i].state);
                    break;
                }
            }
        }
    }
    public void CheckStateUI(ETriggerCanvas state)
    {
        switch (state)
        {
            case ETriggerCanvas.None:
                break;
            case ETriggerCanvas.Barn:
                UIStateMachine.Instance.ChangeState(UIStateMachine.Instance.barnState);
                break;
            case ETriggerCanvas.FarmStall:
                UIStateMachine.Instance.ChangeState(UIStateMachine.Instance.farmStallState);
                break;
            case ETriggerCanvas.SeedShop:
                UIStateMachine.Instance.ChangeState(UIStateMachine.Instance.seedShopState);
                break;
            case ETriggerCanvas.Merchant:
                UIStateMachine.Instance.ChangeState(UIStateMachine.Instance.merchantState);
                break;
            case ETriggerCanvas.Builder:
                UIStateMachine.Instance.ChangeState(UIStateMachine.Instance.builderState);
                break;
            case ETriggerCanvas.AnimalBarn1:
                UIStateMachine.Instance.ChangeState(UIStateMachine.Instance.animalBarn1State);
                break;
            case ETriggerCanvas.AnimalBarn2:
                UIStateMachine.Instance.ChangeState(UIStateMachine.Instance.animalBarn2State);
                break;
            case ETriggerCanvas.SellAnimal:
                UIStateMachine.Instance.ChangeState(UIStateMachine.Instance.sellAnimalState);
                break;
            case ETriggerCanvas.PenDoor:
                SoundEffects.Instance.PlaySound_PenDoor();
                break;
            case ETriggerCanvas.HouseDoor:
                SoundEffects.Instance.PlaySound_HouseDoor();
;               break;
            case ETriggerCanvas.BarnDoor:
                SoundEffects.Instance.PlaySound_BarnDoor();
                break;
            case ETriggerCanvas.GreenhouseDoor:
                SoundEffects.Instance.PlaySound_GreenhouseDoor();
                break;
            case ETriggerCanvas.WaterWell:
                WaterCan.Instance.FillTheWaterCan();
                SoundEffects.Instance.PlaySound_FillWater();
                break;
            case ETriggerCanvas.Bed:
                if (GameTime.Instance.hour >= 6)
                {
                    UIStateMachine.Instance.ChangeState(UIStateMachine.Instance.sleepState);
                    Mp.Instance.ResetMp(false);
                }
                else
                {
                    Notification.Instance.ShowNotification("Chưa đến giờ ngủ!");
                }
                break;
        }
    }
}
