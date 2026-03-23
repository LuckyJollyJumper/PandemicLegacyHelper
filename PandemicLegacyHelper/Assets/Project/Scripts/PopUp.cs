using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class used to control the inputfields and dropdowns of the PopUp GameObject and send filled in values as new card
/// to the GameManager
/// </summary>
public class PopUp : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMPro.TMP_InputField CardNameInput;
    [SerializeField] private TMPro.TMP_Dropdown CardColourInput;
    [SerializeField] private TMPro.TMP_Dropdown CardAmountInput;
    
     /// <summary>
     /// Opens the pop up and allows the user to input a card, which is then added to the unknown deck.
     /// The pop up is closed after the card is added.
     /// </summary>
    public void OnSubmit(){
        string name = this.CardNameInput.text;
        int amount = this.CardAmountInput.value + 1;
        string colour = this.CardColourInput.options[this.CardColourInput.value].text;
        CardData.CardColour cardColour = (CardData.CardColour) System.Enum.Parse(typeof(CardData.CardColour), colour);
        
        GameManager.Instance.AddCardDataToUnknownDeck(new CardData { CardName = name, Colour = cardColour }, amount);

        ClosePopUp();
    }
    public void ClosePopUp(){
        this.gameObject.SetActive(false);
    }
}
