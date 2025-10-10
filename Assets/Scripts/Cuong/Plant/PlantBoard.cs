using TMPro;
using UnityEngine;

public class PlantBoard : MonoBehaviour
{
    public TextMeshProUGUI numberPage;
    public GameObject[] page;
    public int currentNumberPage = 1;
    public void Start()
    {
        numberPage.text = "1/" + page.Length.ToString();    
    }
    public void Next()
    {
        if (currentNumberPage < page.Length)
        {
            currentNumberPage++;
            page[currentNumberPage - 1].SetActive(true);
            page[currentNumberPage - 2].SetActive(false);
            numberPage.text = currentNumberPage.ToString() + "/" + page.Length.ToString();
        }
    }
    public void Back()
    {
        if (currentNumberPage > 1)
        {
            currentNumberPage--;
            page[currentNumberPage - 1].SetActive(true);
            page[currentNumberPage].SetActive(false);
            numberPage.text = currentNumberPage.ToString() + "/" + page.Length.ToString();
        }

    }
}
