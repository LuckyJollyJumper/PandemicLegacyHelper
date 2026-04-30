using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Represents a single deck of CardDataPrefabs. It handles all adding, removing and sorting.
/// </summary>
public class DeckObject : MonoBehaviour
{
    public enum DeckType{UnknownDeck, PreviousKnownDeck, OpenDeck } // Used to change the button on the cards

    [SerializeField] public DeckType DType;
    [SerializeField] private GameObject CardPrefabObject;
    [SerializeField] public List<GameObject> Deck; // Holds all the children Cards of this deck
     [Header("Debug")]
    [SerializeField] protected bool DebugMode;
    protected string DebugID;

    void Start(){
        this.DebugID = $"[DeckObject/{DType}]";
    }


    //----------------------------------------------------------------------//
    //----------Functions for adding and removing cards from deck----------//
    //----------------------------------------------------------------------//

    /// <summary>
    /// Used to load in new cards using only CardData by instantiating a new card if not already in the deck.
    /// </summary>
    public virtual void AddCardData(CardData cardData, int amount){
        (bool exists, GameObject foundCard) = CardExistsInDeck(cardData);
        if (exists){
            foundCard.GetComponent<CardPrefab>().AddAmount(amount);
        }else{
            GameObject newCard = AddCardDataToUI(cardData, amount);
            Deck.Add(newCard);
        }
        UpdateDeck();
    }
    public virtual void AddCardDataList(List<CardData> cards){
        for (int i = 0; i < cards.Count; i++){
            AddCardData(cards[i], cards[i].Amount);
        }
    }
    /// <summary>
    /// Helper function to instantiate a new visual cardObject as a child of this deck
    /// </summary>
    public virtual GameObject AddCardDataToUI(CardData cardData, int amount){
        GameObject card = Instantiate(CardPrefabObject, this.transform);
        card.GetComponent<CardPrefab>().SetCardPrefab(cardData.Clone(), amount, DType, this);
        return card;
    }

    /// <summary>
    /// Removes a single entry from a visual CardObject. If there is only 1 amount left it will remove the whole instance.
    /// It will also automatically update the percentages of the whole deck.
    /// </summary>
    public virtual void RemoveCardData(CardData card, int amount){
        (bool exists, GameObject foundCard) = CardExistsInDeck(card);
        if (exists){
            if(foundCard.GetComponent<CardPrefab>().GetAmount() <= amount){ 
                Deck.Remove(foundCard);
                Destroy(foundCard);
            }
            else{ foundCard.GetComponent<CardPrefab>().SubtractAmount(amount); }
            UpdateDeck();
        }
        else{ if (DebugMode){ Debug.Log($"{DebugID} Could not remove card from list, does not exist"); } }
    }

    /// <summary>
    /// Empties the wholde deck by removing all the instances of CardObjects and returns this.
    /// Used to reload decks from memory and move all cards from OpenDeck in one call.
    /// </summary>
    public virtual List<CardData> EmptyDeck(){
        List<CardData> returnList = new();
        for (int i = Deck.Count-1; i >= 0; i--){
            returnList.Add(Deck[i].GetComponent<CardPrefab>().GetData());
            Destroy(Deck[i]);
            Deck.Remove(Deck[i]);
        }
        return returnList;
    }

    /// <summary>
    /// Helper function to determine if a card is already present in this deck. Needed to determine if a new instance of the card
    /// is needed to be created.
    /// </summary>
    /// <param name="cardData"></param>
    /// <returns></returns>
    public virtual (bool, GameObject) CardExistsInDeck(CardData cardData){
        foreach (GameObject card in Deck){
            if (card.GetComponent<CardPrefab>().GetCardName() == cardData.CardName){ return (true, card); }
        }
        return (false, null);
    }



    //----------------------------------------------------------------------//
    //-------Functions related to calculating probabilities of cards--------//
    //----------------------------------------------------------------------//

    private void UpdateDeck(){
        SetAllProbabilities();
        SortDeckByProbability();
    }

    /// <summary>
    /// Sets probabilities of all the CardObjects in Deck given their amount and the amount in the whole list.
    /// </summary>
    private void SetAllProbabilities(){
        int size = GetActualDeckSize();
        for (int i = 0; i < Deck.Count; i++){
            CardPrefab cPrefab = Deck[i].GetComponent<CardPrefab>();
            cPrefab.SetProbability(GetProbability(size, cPrefab.GetAmount()));
        }
    }
    private float GetProbability(int listSize, int cardAmounts){
        return MathF.Round((float)cardAmounts / (float)listSize * 100f,   1);
    }
    /// <summary>
    /// Returns the size of all the amounts of all the CardObjects added together
    /// </summary>
    public int GetActualDeckSize(){
        int size = 0;
        foreach (GameObject card in Deck){
            size += card.GetComponent<CardPrefab>().GetAmount();
        }
        return size;
    }
    private void SortDeckByProbability(){
        Deck.Sort((a, b) => b.GetComponent<CardPrefab>().GetProbability().CompareTo(a.GetComponent<CardPrefab>().GetProbability()));
        for (int i = 0; i < Deck.Count; i++){
            Deck[i].transform.SetSiblingIndex(i);
        }
    }

}
