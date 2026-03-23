using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// Class used by the Card Prefab. Represents the data and amount of a card and holds the CardData
/// </summary>
public class CardPrefab : MonoBehaviour
{
    private GameObject ParentDeck; // The deck that the CardData is in 
    public CardData Data;
    [SerializeField] private TMPro.TextMeshProUGUI CardNameText;
    [SerializeField] private TMPro.TextMeshProUGUI CardPercentageText;
    [SerializeField] private TMPro.TextMeshProUGUI CardAmountText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TMPro.TextMeshProUGUI ButtonText;

    [HideInInspector] public event Action OnButtonClicked;
    
    public void SetCardPrefab(CardData card, int amount){
        this.OnButtonClicked += () => { GameManager.Instance.AddCardToOpenDeck(this.gameObject);};
        this.Data = card;
        this.CardNameText.text = Data.CardName;
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

    public void ChangeButton(bool setToRemove){
        OnButtonClicked = null;
        if (setToRemove){ 
            this.OnButtonClicked += () => { GameManager.Instance.RemoveCardFromOpenDeck(this.gameObject); };
            this.ButtonText.text = "X";
        }
        else{
            this.OnButtonClicked += () => { GameManager.Instance.AddCardToOpenDeck(this.gameObject);};
            this.ButtonText.text = ">"; 
        }
    }

    public void ButtonClicked(){ OnButtonClicked?.Invoke(); }
}
