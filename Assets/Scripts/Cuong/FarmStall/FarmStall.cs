using UnityEngine;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class FarmStall : MonoBehaviour
{
    public int[] sellPriceSpring;
    public int[] sellPriceSummer;
    public int[] sellPriceFall;
    public int[] sellPriceWinter;

    public bool[] canGrowInSpring;
    public bool[] canGrowInSummer;
    public bool[] canGrowInFall; 
    public bool[] canGrowInWinter;

    public int[] quantity;

    public int totalAmount;
    void Awake()
    {
        quantity = new int[sellPriceSpring.Length];
    }
    public void TotalAmount()
    {
        totalAmount = 0;
        for (int i = 0; i < sellPriceSpring.Length; i++)
        {
            switch (Season.Instance.currentSeason)
            {
                case SeasonState.Spring:
                    totalAmount += sellPriceSpring[i] * quantity[i];
                    break;
                case SeasonState.Summer:
                    totalAmount += sellPriceSummer[i] * quantity[i];
                    break;
                case SeasonState.Fall:
                    totalAmount += sellPriceFall[i] * quantity[i];
                    break;
                case SeasonState.Winter:
                    totalAmount += sellPriceWinter[i] * quantity[i];
                    break;
            }
        }
        if (CharacterSelection.currentCharacter == ECharacter.Kai)
        {
            totalAmount = (int)(totalAmount * 1.08f);
        }
        FarmStallUI.Instance.UpdateUI();
    }
}
