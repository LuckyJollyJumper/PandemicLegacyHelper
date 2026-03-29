using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// Class used by the Card Prefab. Represents the data and amount of a card and holds the CardData
/// </summary>
public class CardPrefab : MonoBehaviour
{
    private DeckObject ParentDeck; // The deck that the CardData is in 
    public CardData Data;
    [SerializeField] private TMPro.TextMeshProUGUI CardNameText;
    [SerializeField] private TMPro.TextMeshProUGUI CardPercentageText;
    [SerializeField] private TMPro.TextMeshProUGUI CardAmountText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TMPro.TextMeshProUGUI ButtonText;

    [HideInInspector] public event Action OnButtonClicked;
    
    public void SetCardPrefab(CardData card, int amount, DeckObject.DeckType cardType, DeckObject parentDeck){
        SetButton(cardType);
        this.Data = card;
        this.CardNameText.text = Data.CardName;
        this.ParentDeck = parentDeck;
        SetAmount(amount);

        Color c = new Color(0f,0f,0f);
        if (Data.Colour == CardData.CardColour.Yellow){
            this.CardAmountText.color = c;
            this.CardPercentageText.color = c;
            this.CardNameText.color = c;
        }
        this.backgroundImage.color = Data.GetCardColour();
    }

    public void SetProbability(float prob){ 
        this.Data.Probability = prob;
        this.CardPercentageText.text = $"{Data.Probability}%";
    }
    public void AddAmount(int amount){
        this.Data.Amount += amount;
        this.CardAmountText.text = $"{Data.Amount}";
    }
    public void SubtractAmount(int amount){
        this.Data.Amount -= amount;
        this.CardAmountText.text = $"{Data.Amount}";
    }
    public void SetAmount(int amount){
        this.Data.Amount = amount;
        this.CardAmountText.text = $"{Data.Amount}";
    }
    public int GetAmount(){ return Data.Amount; }

    public void SetButton(DeckObject.DeckType cardType){
        OnButtonClicked = null; // Reset the list
        switch (cardType){
            case DeckObject.DeckType.UnknownDeck:
                this.OnButtonClicked += () => { GameManager.Instance.AddCardDataToOpenDeck(this.Data, ParentDeck);};
                this.ButtonText.text = ">";
                break;
            case DeckObject.DeckType.PreviousKnownDeck:
                this.OnButtonClicked += () => { GameManager.Instance.AddCardDataToOpenDeck(this.Data, ParentDeck);};
                this.ButtonText.text = ">";
                break;
            case DeckObject.DeckType.OpenDeck:
                this.OnButtonClicked += () => { GameManager.Instance.RemoveCardFromOpenDeck(this.Data); };
                this.ButtonText.text = "X";
                break;
        }
    }

    public void ButtonClicked(){ OnButtonClicked?.Invoke(); }
}
