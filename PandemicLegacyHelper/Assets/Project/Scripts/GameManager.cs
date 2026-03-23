using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// An interactable object that starts an activity when interacted with.
/// </summary>
public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance{ // Make sure only one instance of GameManager exists
        get{
            if (instance == null){
                instance = FindFirstObjectByType<GameManager>();

                if (instance == null){
                    GameObject singleton = new GameObject(typeof(GameManager).ToString());
                    instance = singleton.AddComponent<GameManager>();
                    DontDestroyOnLoad(singleton);
                }
            }
            return instance;
        }
    }
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
        PopUpObject.SetActive(false);
        PreviousKnownDeck = new();
        UnknownDeck = new();
        OpenDeck = new();

        LoadFromFile();
    }

    //----------------------------------------------------------------------//
    //----------Functions for adding and removing cards from decks----------//
    //----------------------------------------------------------------------//

     /// <summary>
     /// Adds a cardData and amount to the UnknownDeck, used to load in new cards and setup
     /// </summary>
     /// <param name="cardData"></param>
     /// <param name="amount"></param>
    public void AddCardDataToUnknownDeck(CardData cardData, int amount){
        (bool exists, GameObject foundCard) = CardExistsInList(UnknownDeck, cardData);
        if (exists){
            foundCard.GetComponent<CardPrefab>().AddAmount(1);
            if(DebugMode){Debug.Log($"{DebugID} Added 1 to {foundCard.GetComponent<CardPrefab>().Data.CardName}, new: {foundCard.GetComponent<CardPrefab>().Data.Amount}");}
        }
        GameObject card = AddCardToUI(cardData, amount, UnknownDeckObject.transform);

        UnknownDeck.Add(card);
        SetAllProbabilities(UnknownDeck);

        if(DebugMode){Debug.Log($"{DebugID} Added {card.GetComponent<CardPrefab>().Data.CardName} to UnknownDeck");}
    }
    public void AddCardToOpenDeck(GameObject card){
        CardPrefab cardPrefab = card.GetComponent<CardPrefab>();

        // Add card to OpenDeck
        (bool exists, GameObject foundCard) = CardExistsInList(OpenDeck, cardPrefab.Data);
        if (exists){
            foundCard.GetComponent<CardPrefab>().AddAmount(1);
            if(DebugMode){Debug.Log($"{DebugID} Added 1 to {foundCard.GetComponent<CardPrefab>().Data.CardName}, new: {foundCard.GetComponent<CardPrefab>().Data.Amount}");}
        }
        else{
            GameObject newCard = AddCardToUI(cardPrefab.Data, 1, OpenDeckObject.transform);
            newCard.GetComponent<CardPrefab>().ChangeButton(true);
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
            cardPrefab.SubtractAmount(1);
            if(DebugMode){Debug.Log($"{DebugID} Removed 1 from {card.GetComponent<CardPrefab>().Data.CardName}, new: {card.GetComponent<CardPrefab>().Data.Amount}");}
        }

        // Update probabilities
        SetAllProbabilities(UnknownDeck);
        SortListByProbability(UnknownDeck);
    }
    public (bool, GameObject) CardExistsInList(List<GameObject> list, CardData cardData){
        foreach (GameObject card in list){
            if (card.GetComponent<CardPrefab>().Data.CardName == cardData.CardName){ return (true, card); }
        }
        return (false, null);
    }
    public void AddCardToDeck(GameObject card, int amount, Transform newParent, List<GameObject> newList){
        var cardData = card.GetComponent<CardPrefab>().Data;
        (bool exists, GameObject foundCard) = CardExistsInList(newList, cardData);
        if (exists){
            foundCard.GetComponent<CardPrefab>().AddAmount(amount);
        }else{
            GameObject newCard = AddCardToUI(cardData, amount, newParent);
            newList.Add(newCard);
        }
    }
    public void RemoveCardFromDeck(GameObject card, int amount, List<GameObject> oldList){
        CardPrefab cardPrefab = card.GetComponent<CardPrefab>();
        (bool exists, _) = CardExistsInList(oldList, cardPrefab.Data);
        if (exists){
            if(cardPrefab.GetAmount() <= amount){ 
                oldList.Remove(card);
                Destroy(card);
            }
            else{ cardPrefab.SubtractAmount(amount); }
        }
        else{ if (DebugMode){ Debug.Log($"{DebugID} Could not remove card from list, does not exist"); } }
        
    }
    public GameObject AddCardToUI(CardData cardData, int amount, Transform newParent){
        GameObject card = Instantiate(CardPrefabObject, newParent);
        card.GetComponent<CardPrefab>().SetCardPrefab(cardData.Clone(), amount);
        return card;
    }

    public void RemoveCardFromOpenDeck(GameObject card){
        CardPrefab cardPrefab = card.GetComponent<CardPrefab>();
        if (cardPrefab.GetAmount() == 1){ 
            OpenDeck.Remove(card);
            Destroy(card);
        }
        else{ cardPrefab.SubtractAmount(1); }
    }



    //----------------------------------------------------------------------//
    //-------Functions related to calculating probabilities of cards--------//
    //----------------------------------------------------------------------//

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
        }
        return size;
    }
    public void SortListByProbability(List<GameObject> list){
        list.Sort((a, b) => b.GetComponent<CardPrefab>().Data.Probability.CompareTo(a.GetComponent<CardPrefab>().Data.Probability));
        for (int i = 0; i < list.Count; i++){
            list[i].transform.SetSiblingIndex(i);
        }
    }



    //----------------------------------------------------------------------//
    //-----------------------Functions used by buttons----------------------//
    //----------------------------------------------------------------------//

    /// <summary>
    /// Called by the button AddInfectionCard. Opens the PopUp gameObject.
    /// </summary>
    public void OpenPopUp(){
        PopUpObject.SetActive(true);
    }
    /// <summary>
    /// Called by the Pandemic button. Puts all cards from the OpenDeck into the PreviousKnowndeck
    /// </summary>
    public void MoveAllCardsToPreviousknownDeck(){
        for (int i = OpenDeck.Count - 1; i >= 0; i--){
            GameObject card = OpenDeck[i];
            CardPrefab cardPrefab = card.GetComponent<CardPrefab>();
            int amount = cardPrefab.GetAmount();
            Debug.Log($"{DebugID} Moving {cardPrefab.Data.CardName} x{amount} to PreviousKnownDeck");

            AddCardToDeck(card, amount, PreviousKnownDeckObject.transform, PreviousKnownDeck);
            RemoveCardFromDeck(card, amount, OpenDeck);
        }

        SetAllProbabilities(PreviousKnownDeck);
        SortListByProbability(PreviousKnownDeck);
    }

     /// <summary>
    /// Will move all cards to UnknownDeck
    /// </summary>
    public void ResetAllCards(){
        // TODO move all cards to UnknownDeck
        for(int i = 0; i < OpenDeck.Count; i++){
            CardPrefab cardPrefab = OpenDeck[i].GetComponent<CardPrefab>();
            (bool exists, GameObject foundCard) = CardExistsInList(OpenDeck, cardPrefab.Data);
            if (exists){
                foundCard.GetComponent<CardPrefab>().AddAmount(cardPrefab.GetAmount());
                OpenDeck.Remove(OpenDeck[i]);
                Destroy(cardPrefab.gameObject);
            }
            else{
                AddCardToUI(cardPrefab.Data, cardPrefab.GetAmount(), UnknownDeckObject.transform);
                UnknownDeck.Add(OpenDeck[i]);
                OpenDeck.Remove(OpenDeck[i]);
            }
            
        }
    }



    //----------------------------------------------------------------------//
    //-----------Functions for starting and stopping application------------//
    //----------------------------------------------------------------------//
    public void SaveToFile(){
        // TODO: save all lists
        CardDataSaver.SaveToFile(UnknownDeck);
    }
    public void LoadFromFile(){
        (bool exists, List<CardData> loadedCards) = CardDataSaver.LoadFromFile();
        if (exists){
            foreach (CardData card in loadedCards){
                AddCardDataToUnknownDeck(card, card.Amount);
            }
            if (DebugMode){ Debug.Log($"{DebugID} Loaded cards from memory"); }
        }
        else{
            CardData w = new() { CardName = "Washington", Colour = CardData.CardColour.Blue };
            AddCardDataToUnknownDeck(w, 3);
            w = new() { CardName = "New York",      Colour = CardData.CardColour.Blue };
            AddCardDataToUnknownDeck(w, 3);
            w = new() { CardName = "Jacksonville",  Colour = CardData.CardColour.Yellow };
            AddCardDataToUnknownDeck(w, 3);
            w = new() { CardName = "London",        Colour = CardData.CardColour.Blue };
            AddCardDataToUnknownDeck(w, 3);
            w = new() { CardName = "Lagos",         Colour = CardData.CardColour.Yellow };
            AddCardDataToUnknownDeck(w, 3);
            w = new() { CardName = "Sao Paolo",     Colour = CardData.CardColour.Yellow };
            AddCardDataToUnknownDeck(w, 3);
            w = new() { CardName = "Istanbul",      Colour = CardData.CardColour.Black };
            AddCardDataToUnknownDeck(w, 3);
            w = new() { CardName = "Tripoli",       Colour = CardData.CardColour.Black };
            AddCardDataToUnknownDeck(w, 3);
            w = new() { CardName = "Cairo",         Colour = CardData.CardColour.Black };
            AddCardDataToUnknownDeck(w, 3);
            if (DebugMode){ Debug.Log($"{DebugID} File not found on device, creating new file"); }
            SaveToFile();
        }
    }
    public void QuitGame(){
        SaveToFile();
        Application.Quit();
    }
}
