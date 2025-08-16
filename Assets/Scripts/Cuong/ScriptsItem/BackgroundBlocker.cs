using UnityEngine.EventSystems;
using UnityEngine;

public class BackgroundBlocker : MonoBehaviour, IPointerClickHandler
{
    public SplitMenuUI splitMenuUI;

    public void OnPointerClick(PointerEventData eventData)
    {
        splitMenuUI.Hide();
        gameObject.SetActive(false);
    }
}
