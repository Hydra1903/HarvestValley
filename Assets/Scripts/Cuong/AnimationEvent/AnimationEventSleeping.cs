using UnityEngine;

public class AnimationEventSleeping : MonoBehaviour
{
    public void WakeUp()
    {
        UIStateMachine.Instance.ChangeState(UIStateMachine.Instance.mainScreenState);
        GameTime.Instance.UnpauseGame();
    }
    public void NextDayAndSave()
    {
        GameTime.Instance.NextDay();       
        GameController.Instance.SaveGame();
    }
}
