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
        (bool exists, GameObject foundCard) = CardExistsInList(cardData);
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
    public virtual GameObject AddCardDataToUI(CardData cardData, int amount){
        GameObject card = Instantiate(CardPrefabObject, this.transform);
        card.GetComponent<CardPrefab>().SetCardPrefab(cardData.Clone(), amount, DType, this);
        return card;
    }

    public virtual void RemoveCardData(CardData card, int amount){
        (bool exists, GameObject foundCard) = CardExistsInList(card);
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

    public virtual List<CardData> EmptyDeck(){
        List<CardData> returnList = new();
        for (int i = Deck.Count-1; i >= 0; i--){
            returnList.Add(Deck[i].GetComponent<CardPrefab>().GetData());
            Destroy(Deck[i]);
            Deck.Remove(Deck[i]);
        }
        return returnList;
    }

    public virtual (bool, GameObject) CardExistsInList(CardData cardData){
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
        SortListByProbability();
    }

    private void SetAllProbabilities(){
        int size = GetActualListSize();
        for (int i = 0; i < Deck.Count; i++){
            CardPrefab cPrefab = Deck[i].GetComponent<CardPrefab>();
            cPrefab.SetProbability(GetProbability(size, cPrefab.GetAmount()));
        }
    }
    private float GetProbability(int listSize, int cardAmounts){
        return MathF.Round((float)cardAmounts / (float)listSize * 100f,   1);
    }
    public int GetActualListSize(){
        int size = 0;
        foreach (GameObject card in Deck){
            size += card.GetComponent<CardPrefab>().GetAmount();
        }
        return size;
    }
    private void SortListByProbability(){
        Deck.Sort((a, b) => b.GetComponent<CardPrefab>().GetProbability().CompareTo(a.GetComponent<CardPrefab>().GetProbability()));
        for (int i = 0; i < Deck.Count; i++){
            Deck[i].transform.SetSiblingIndex(i);
        }
    }

}
