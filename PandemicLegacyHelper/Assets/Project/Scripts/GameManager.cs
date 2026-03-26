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
    [SerializeField] private GameObject FillerObject;
    [SerializeField] public List<GameObject> UnknownDeck; //The part of the drawpile we have not reached yet
    [SerializeField] public List<List<GameObject>> PreviousKnownDeck; // The part of the drawpile we know
    [SerializeField] public List<GameObject> OpenDeck; // the cards that ar efaced up
    [Header("References")]
    [SerializeField] private GameObject UnknownDeckObject;
    [SerializeField] private GameObject PreviousKnownDeckObject;
    [SerializeField] private GameObject OpenDeckObject;
    [SerializeField] private GameObject PopUpObject;
    [SerializeField] private GameObject SettingsObject;
    [Header("Debug")]
    [SerializeField] private bool DebugMode;
    private string DebugID = "[GameManager]";
    
    void Start(){
        PopUpObject.SetActive(false);
        SettingsObject.SetActive(false);
        PreviousKnownDeck = new();
        UnknownDeck = new();
        OpenDeck = new();

        LoadFromFile();
    }

    //----------------------------------------------------------------------//
    //----------Functions for adding and removing cards from decks----------//
    //----------------------------------------------------------------------//
    public void AddCardToOpenDeck(GameObject card){
        CardPrefab cardPrefab = card.GetComponent<CardPrefab>();

        // Add card to OpenDeck
        (bool exists, GameObject foundCard) = CardExistsInList(OpenDeck, cardPrefab.Data);
        if (exists){
            foundCard.GetComponent<CardPrefab>().AddAmount(1);
            if(DebugMode){Debug.Log($"{DebugID} Added 1 to {foundCard.GetComponent<CardPrefab>().Data.CardName}, new: {foundCard.GetComponent<CardPrefab>().Data.Amount}");}
        }
        else{
            GameObject newCard = AddCardDataToUI(cardPrefab.Data, 1, OpenDeckObject.transform);
            newCard.GetComponent<CardPrefab>().ChangeButton(true);
            OpenDeck.Add(newCard);
            if(DebugMode){Debug.Log($"{DebugID} Added new card {newCard.GetComponent<CardPrefab>().Data.CardName}");}
        }

        // Edit card from source deck
        if (cardPrefab.Data.Amount <= 1){
            Debug.Log($"{cardPrefab.Data.Amount }");
            Destroy(card);
            
            // Try to remove from UnknownDeck first
            if (UnknownDeck.Contains(card)){
                UnknownDeck.Remove(card);
                if(DebugMode){Debug.Log($"{DebugID} Removed {card.GetComponent<CardPrefab>().Data.CardName} from UnknownDeck");}
            }
            // If not in UnknownDeck, try PreviousKnownDeck
            else {
                RemoveCardFromPreviousKnownDeckAndReorder(card, 1);
                if(DebugMode){Debug.Log($"{DebugID} Removed {card.GetComponent<CardPrefab>().Data.CardName} from PreviousKnownDeck");}
            }
        }
        else{
            cardPrefab.SubtractAmount(1);
            
            // Update probabilities of source deck
            if (UnknownDeck.Contains(card)){
                SetAllProbabilities(UnknownDeck);
                SortListByProbability(UnknownDeck);
            }
            else {
                RemoveCardFromPreviousKnownDeckAndReorder(card, 0); // 0 amount since we only subtracted
            }
            
            if(DebugMode){Debug.Log($"{DebugID} Removed 1 from {card.GetComponent<CardPrefab>().Data.CardName}, new: {card.GetComponent<CardPrefab>().Data.Amount}");}
        }

        // Update probabilities
        SetAllProbabilities(OpenDeck);
        SortListByProbability(OpenDeck);
    }
    public void AddCardToDeck(GameObject card, int amount, Transform newParent, List<GameObject> newDeck){
        var cardData = card.GetComponent<CardPrefab>().Data;
        AddCardDataToDeck(cardData, amount, newParent, newDeck);
    }
    /// <summary>
    /// Used to load in new cards using only CardData. Used to load in new cards that have not been instantiated yet.
    /// </summary>
    public void AddCardDataToDeck(CardData cardData, int amount, Transform newParent, List<GameObject> newDeck){
        (bool exists, GameObject foundCard) = CardExistsInList(newDeck, cardData);
        if (exists){
            foundCard.GetComponent<CardPrefab>().AddAmount(amount);
        }else{
            GameObject newCard = AddCardDataToUI(cardData, amount, newParent);
            newDeck.Add(newCard);
        }
        SetAllProbabilities(newDeck);
        SortListByProbability(newDeck);
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
    /// <summary>
    /// Used by the PopUp or other methods that want to add a card without knowing of the Lists data
    /// </summary>
    public void AddCardDataToUnknownDeck(CardData data, int amount){
        AddCardDataToDeck(data, amount, UnknownDeckObject.transform, UnknownDeck);
    }

    public (bool, GameObject) CardExistsInList(List<GameObject> list, CardData cardData){
        foreach (GameObject card in list){
            if (card.GetComponent<CardPrefab>().Data.CardName == cardData.CardName){ return (true, card); }
        }
        return (false, null);
    }
    private List<GameObject> FindCardInPreviousKnownDeck(CardData cardData){
        for(int i = 0; i < PreviousKnownDeck.Count; i++){
            (bool exists, _) = CardExistsInList(PreviousKnownDeck[i], cardData);
            if (exists){
                return PreviousKnownDeck[i];
            }
        }
        return null;
    }
    private void RemoveCardFromPreviousKnownDeckAndReorder(GameObject card, int amount){
        List<GameObject> sublist = FindCardInPreviousKnownDeck(card.GetComponent<CardPrefab>().Data);
        if (sublist != null){
            RemoveCardFromDeck(card, amount, sublist);
            SetAllProbabilities(sublist);
            SortListByProbability(sublist);
            if(DebugMode){ Debug.Log($"{DebugID} Re-sorted PreviousKnownDeck sublist after card removal"); }
        }
    }
    public GameObject AddCardDataToUI(CardData cardData, int amount, Transform newParent){
        GameObject card = Instantiate(CardPrefabObject, newParent);
        card.GetComponent<CardPrefab>().SetCardPrefab(cardData.Clone(), amount);
        return card;
    }

     /// <summary>
     /// Used to remove a single card from the OpenDeck. Called from the CardDataPrefab button
     /// </summary>
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
    private void SetAllProbabilities(List<List<GameObject>> list){
        for(int i = 0; i < list.Count; i++){
            SetAllProbabilities(list[i]);
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
    /// Called by the settings button.
    /// </summary>
    public void OpenCloseSettings(){
        if (SettingsObject.activeSelf){ SettingsObject.SetActive(false); }
        else{ SettingsObject.SetActive(true); }
    }
    /// <summary>
    /// Called by the Pandemic button. Puts all cards from the OpenDeck into the PreviousKnowndeck
    /// </summary>
    public void MoveAllCardsToPreviousknownDeck(){
        List<GameObject> topList = new();
        if(OpenDeck.Count == 0){ return; }

        for (int i = OpenDeck.Count - 1; i >= 0; i--){
            GameObject card = OpenDeck[i];
            CardPrefab cardPrefab = card.GetComponent<CardPrefab>();
            int amount = cardPrefab.GetAmount();
            Debug.Log($"{DebugID} Moving {cardPrefab.Data.CardName} x{amount} to PreviousKnownDeck");

            AddCardToDeck(card, amount, PreviousKnownDeckObject.transform, topList);

            RemoveCardFromDeck(card, amount, OpenDeck);
        }

        // Move all new cards to the top (indices 0 onwards)
        for (int i = 0; i < topList.Count; i++){
            topList[i].transform.SetSiblingIndex(i);
        }

        // Create filler instance after the new cards
        GameObject fillerInstance = Instantiate(FillerObject, PreviousKnownDeckObject.transform);
        fillerInstance.transform.SetSiblingIndex(topList.Count);

        SetAllProbabilities(topList);
        SortListByProbability(topList);
        PreviousKnownDeck.Add(topList);
        Debug.Log($"{PreviousKnownDeck[0]}");
        if (DebugMode){ Debug.Log($"{DebugID} Pressed Pandemic button; Moved all cards in Open Deck to PreviousKnownDeck"); }
    }

    /// <summary>
    /// Will move all cards to UnknownDeck to reset to start a new month. Called from the Settings resetMonth button
    /// </summary>
    public void ResetAllCards(){
        // TODO move all cards to UnknownDeck
        MoveAllCardsToUnknownDeck(OpenDeck);
        for (int i = PreviousKnownDeck.Count-1; i >= 0; i--){
            MoveAllCardsToUnknownDeck(PreviousKnownDeck[i]);
        }
        if (DebugMode){ Debug.Log($"{DebugID} Pressed Reset month button; Moved all cards to the UnknownDeck"); }
        OpenCloseSettings();
    }
    private void MoveAllCardsToUnknownDeck(List<GameObject> list){
        for(int i = list.Count - 1; i >= 0; i--){
            GameObject card = list[i];
            CardPrefab cardPrefab = card.GetComponent<CardPrefab>();
            int amount = cardPrefab.GetAmount();

            AddCardToDeck(card, amount, UnknownDeckObject.transform, UnknownDeck);
            RemoveCardFromDeck(card, amount, list);
        }

        SetAllProbabilities(UnknownDeck);
        SortListByProbability(UnknownDeck);
    }

    /// <summary>
    /// Will default back to the games initial state with only the beginning infection cards
    /// </summary>
    public void ResetApplication(){
        ResetAllCards();
        for(int i = 0; i < UnknownDeck.Count; i++){
            Destroy(UnknownDeck[i]);
        }
        SaveInitalCardData();
        if (DebugMode){ Debug.Log($"{DebugID} Pressed Reset button; Reset the application to starting infection deck"); }
    }



    //----------------------------------------------------------------------//
    //-----------Functions for starting and stopping application------------//
    //----------------------------------------------------------------------//
    public void SaveToFile(){
        // TODO: save all lists
        List<GameObject> tempDeck = OpenDeck;
        for (int i = PreviousKnownDeck.Count; i >= 0; i--){
           tempDeck.AddRange(PreviousKnownDeck[i]); 
        }
        CardDataSaver.SaveToFile(UnknownDeck);
    }
    public void LoadFromFile(){
        (bool exists, List<CardData> loadedCards) = CardDataSaver.LoadFromFile();
        if (exists){
            foreach (CardData card in loadedCards){
                AddCardDataToDeck(card, card.Amount, UnknownDeckObject.transform, UnknownDeck);
            }
            if (DebugMode){ Debug.Log($"{DebugID} Loaded cards from memory"); }
        }
        else{
            SaveInitalCardData();
            if (DebugMode){ Debug.Log($"{DebugID} File not found on device, created new file"); }
        }
    }
    private void SaveInitalCardData(){
        UnknownDeck = new();
        CardData w = new() { CardName = "Washington", Amount = 3, Colour = CardData.CardColour.Blue };
        AddCardDataToDeck(w, w.Amount, UnknownDeckObject.transform, UnknownDeck);
        w = new() { CardName = "New York",      Amount = 3,     Colour = CardData.CardColour.Blue };
        AddCardDataToDeck(w, 3, UnknownDeckObject.transform, UnknownDeck);
        w = new() { CardName = "Jacksonville",  Amount = 3,     Colour = CardData.CardColour.Yellow };
        AddCardDataToDeck(w, 3, UnknownDeckObject.transform, UnknownDeck);
        w = new() { CardName = "London",        Amount = 3,     Colour = CardData.CardColour.Blue };
        AddCardDataToDeck(w, 3, UnknownDeckObject.transform, UnknownDeck);
        w = new() { CardName = "Lagos",         Amount = 3,     Colour = CardData.CardColour.Yellow };
        AddCardDataToDeck(w, 3, UnknownDeckObject.transform, UnknownDeck);
        w = new() { CardName = "Sao Paolo",     Amount = 3,     Colour = CardData.CardColour.Yellow };
        AddCardDataToDeck(w, 3, UnknownDeckObject.transform, UnknownDeck);
        w = new() { CardName = "Istanbul",      Amount = 3,     Colour = CardData.CardColour.Black };
        AddCardDataToDeck(w, 3, UnknownDeckObject.transform, UnknownDeck);
        w = new() { CardName = "Tripoli",       Amount = 3,     Colour = CardData.CardColour.Black };
        AddCardDataToDeck(w, 3, UnknownDeckObject.transform, UnknownDeck);
        w = new() { CardName = "Cairo",         Amount = 3,     Colour = CardData.CardColour.Black };
        AddCardDataToDeck(w, 3, UnknownDeckObject.transform, UnknownDeck);
        SaveToFile();
    }
    public void QuitGame(){
        SaveToFile();
        Application.Quit();
    }
}
