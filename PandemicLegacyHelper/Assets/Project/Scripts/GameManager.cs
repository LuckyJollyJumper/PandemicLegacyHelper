using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// An interactable object that starts an activity when interacted with.
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject CardPrefabObject;
    [SerializeField] public List<GameObject> UnknownDeck; //The part of the drawpile we have not reached yet
    [SerializeField] public List<GameObject> PreviousKnownDeck; // The part of the drawpile we know
    [SerializeField] public List<GameObject> OpenDeck; // the cards that ar efaced up
    [Header("References")]
    [SerializeField] private GameObject UnknownDeckObject;
    [SerializeField] private GameObject PreviousKnownDeckObject;
    [SerializeField] private GameObject OpenDeckObject;
    [SerializeField] private GameObject PopUpObject;
    [Header("Debug")]
    [SerializeField] private bool DebugMode;
    private string DebugID = "[GameManager]";
    

    void Start(){
        CardData w = new() { CardName = "Washington", Colour = CardData.CardColour.Blue };
        AddCardToUnknownDeck(w, 3);
        w = new() { CardName = "New York",      Colour = CardData.CardColour.Blue };
        AddCardToUnknownDeck(w, 3);
        w = new() { CardName = "Jacksonville",  Colour = CardData.CardColour.Yellow };
        AddCardToUnknownDeck(w, 3);
        w = new() { CardName = "London",        Colour = CardData.CardColour.Blue };
        AddCardToUnknownDeck(w, 3);
        w = new() { CardName = "Lagos",         Colour = CardData.CardColour.Yellow };
        AddCardToUnknownDeck(w, 3);
        w = new() { CardName = "Sao Paolo",     Colour = CardData.CardColour.Yellow };
        AddCardToUnknownDeck(w, 3);
        w = new() { CardName = "Istanbul",      Colour = CardData.CardColour.Black };
        AddCardToUnknownDeck(w, 3);
        w = new() { CardName = "Tripoli",       Colour = CardData.CardColour.Black };
        AddCardToUnknownDeck(w, 3);
        w = new() { CardName = "Cairo",         Colour = CardData.CardColour.Black };
        AddCardToUnknownDeck(w, 3);

        PreviousKnownDeck = new();
        OpenDeck = new();
    }

    /*
     *  Functions used when adding user inputted cards
     */
    public void AddCardToUnknownDeck(CardData cardData, int amount){
        GameObject card = AddCardToUI(cardData, amount, UnknownDeckObject.transform);
        card.GetComponent<CardPrefab>().OnButtonClicked += () => { 
            AddCardToOpenDeck(card); 
            Debug.Log("--------------------------------");
        };

        UnknownDeck.Add(card);
        SetAllProbabilities(UnknownDeck);

        if(DebugMode){Debug.Log($"{DebugID} Added {card.GetComponent<CardPrefab>().Data.CardName} to UnknownDeck");}
    }
    public void AddCardToOpenDeck(GameObject card){
        CardPrefab cardPrefab = card.GetComponent<CardPrefab>();

        // Add card to OpenDeck
        (bool exists, GameObject foundCard) = CardExistsInList(OpenDeck, cardPrefab.Data);
        if (exists){
            foundCard.GetComponent<CardPrefab>().AddAmountByOne();
            if(DebugMode){Debug.Log($"{DebugID} Added 1 to {foundCard.GetComponent<CardPrefab>().Data.CardName}, new: {foundCard.GetComponent<CardPrefab>().Data.Amount}");}
        }
        else{
            GameObject newCard = AddCardToUI(cardPrefab.Data, 1, OpenDeckObject.transform);
            newCard.GetComponent<CardPrefab>().ChangeButton(true);
            newCard.GetComponent<CardPrefab>().OnButtonClicked += () =>{ RemoveCardFromOpenDeck(newCard); };
            OpenDeck.Add(newCard);
            if(DebugMode){Debug.Log($"{DebugID} Added new card {newCard.GetComponent<CardPrefab>().Data.CardName}");}
        }

        // Edit card from previous deck
        if (cardPrefab.Data.Amount <= 1){
            Debug.Log($"{cardPrefab.Data.Amount }");
            Destroy(card);
            UnknownDeck.Remove(card);
            if(DebugMode){Debug.Log($"{DebugID} Removed {card.GetComponent<CardPrefab>().Data.CardName}");}
        }
        else{
            cardPrefab.LowerAmountByOne();
            if(DebugMode){Debug.Log($"{DebugID} Removed 1 from {card.GetComponent<CardPrefab>().Data.CardName}, new: {card.GetComponent<CardPrefab>().Data.Amount}");}
        }

        // Update probabilities of all decks
        SetAllProbabilities(UnknownDeck);
        SortListByProbability(UnknownDeck);
        SetAllProbabilities(PreviousKnownDeck);
        SortListByProbability(PreviousKnownDeck);
    }
    public void AddCardToKnownDeck(CardData cardData, int amount){
        // PreviousKnownDeck.Add(cardData);
    }
    public (bool, GameObject) CardExistsInList(List<GameObject> list, CardData cardData){
        foreach (GameObject card in list){
            if (card.GetComponent<CardPrefab>().Data.CardName == cardData.CardName){ return (true, card); }
        }
        return (false, null);
    }
    public GameObject AddCardToUI(CardData cardData, int amount, Transform newParent){
        GameObject card = Instantiate(CardPrefabObject, newParent);
        card.GetComponent<CardPrefab>().SetCardPrefab(cardData.Clone(), amount);
        return card;
    }

    public void RemoveCardFromOpenDeck(GameObject card){
        
        CardPrefab cardPrefab = card.GetComponent<CardPrefab>();
        if (cardPrefab.GetAmount() == 1){ 
            Destroy(card); 
            OpenDeck.Remove(card);
        }
        else{ cardPrefab.LowerAmountByOne(); }
        SetAllProbabilities(OpenDeck);
    }



    public void SetAllProbabilities(List<GameObject> list){
        int size = GetActualListSize(list);
        for (int i = 0; i < list.Count; i++){
            CardPrefab cPrefab = list[i].GetComponent<CardPrefab>();
            cPrefab.SetProbability(GetProbability(size, cPrefab.Data.Amount));
        }
    }
    public float GetProbability(int listSize, int cardAmounts){
        return MathF.Round((float)cardAmounts / (float)listSize * 100f,   1);
    }
    public int GetActualListSize(List<GameObject> list){
        int size = 0;
        foreach (GameObject card in list){
            size += card.GetComponent<CardPrefab>().Data.Amount;
            //Debug.Log($"{card.GetComponent<CardPrefab>().Data.CardName}: {size}");
        }
        return size;
    }
    public void SortListByProbability(List<GameObject> list){
        list.Sort((a, b) => b.GetComponent<CardPrefab>().Data.Probability.CompareTo(a.GetComponent<CardPrefab>().Data.Probability));
        for (int i = 0; i < list.Count; i++){
            list[i].transform.SetSiblingIndex(i);
        }
    }


    /// <summary>
    /// Called by the button
    /// </summary>
    public void AddCard(){
        if(DebugMode){Debug.Log($"{DebugID} Adding card");}
        PopUpObject.SetActive(true);
    }
    /// <summary>
    /// Called by the Pandemic button. Puts all cards from the OpenDeck into the PreviousKnowndeck
    /// </summary>
    public void ReshuffleCards(){
        for (int i = 0; i < OpenDeck.Count; i++){
            GameObject card = OpenDeck[i];
            CardPrefab cardPrefab = card.GetComponent<CardPrefab>();

            // Add card to PreviousKnownDeck
            (bool exists, GameObject foundCard) = CardExistsInList(PreviousKnownDeck, cardPrefab.Data);
            if (exists){
                foundCard.GetComponent<CardPrefab>().AddAmountByOne();
                if(DebugMode){Debug.Log($"{DebugID} Added 1 to {foundCard.GetComponent<CardPrefab>().Data.CardName} in PreviousKnownDeck, new: {foundCard.GetComponent<CardPrefab>().Data.Amount}");}
            }
            else{
                GameObject newCard = AddCardToUI(cardPrefab.Data, cardPrefab.GetAmount(), PreviousKnownDeckObject.transform);
                newCard.GetComponent<CardPrefab>().ChangeButton(false);
                PreviousKnownDeck.Add(newCard);
                if(DebugMode){Debug.Log($"{DebugID} Added new card {newCard.GetComponent<CardPrefab>().Data.CardName} to PreviousKnownDeck");}
            }

            Destroy(card);
        }
    }
}
