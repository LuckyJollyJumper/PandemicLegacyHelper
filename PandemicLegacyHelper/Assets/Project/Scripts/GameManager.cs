using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

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
    // [SerializeField] private GameObject DeckObject; // Prefab object that holds a part of the previousKnownDeck
    [SerializeField] private GameObject DividerDeckObject; // Prefab object that holds a part of the previousKnownDeck
    
   
    [SerializeField] private DeckObject UnknownDeckObject;
    [SerializeField] private DeckObject PreviousKnownDeckObject;
    [SerializeField] private List<DeckObject> PreviousKnownDeckObjects;
    [SerializeField] private DeckObject OpenDeckObject;
    [Header("References")]
    [SerializeField] private GameObject PopUpObject;
    [SerializeField] private GameObject SettingsPopUpObject;
    [SerializeField] private GameObject WarningPopUpObject;
    [SerializeField] private GameObject PlayerDeckPopUpObject;
    [SerializeField] private GameObject PlayerDeckButton;
    [SerializeField] private Month MonthDropDownObject;

    [Header("Debug")]
    [SerializeField] private bool DebugMode;
    private string DebugID = "[GameManager]";
    
    void Start(){
        PopUpObject.SetActive(false);
        SettingsPopUpObject.SetActive(false);
        WarningPopUpObject.SetActive(false);
        PlayerDeckPopUpObject.SetActive(false);

        LoadFromFile();
    }



    //----------------------------------------------------------------------//
    //----------Functions for adding and removing cards from decks----------//
    //----------------------------------------------------------------------//

    /// <summary>
    /// Called from the PopUp and when loading in from file
    /// </summary>
    public void AddCardDataToUnknownDeck(CardData newCard, int amount){
        UnknownDeckObject.AddCardData(newCard, amount);
        SaveToFile();
    }
    /// <summary>
    /// Called from the CardDataPrefab button
    /// </summary>
    public void AddCardDataToOpenDeck(CardData card, DeckObject deck){
        deck.RemoveCardData(card, 1);
        OpenDeckObject.AddCardData(card, 1);
        SaveToFile();
    }

    /// <summary>
     /// Called from the CardDataPrefab button
     /// </summary>
    public void RemoveCardFromOpenDeck(CardData card){
        OpenDeckObject.RemoveCardData(card, 1);
        SaveToFile();
    }



    //----------------------------------------------------------------------//
    //-----------------------Functions used by buttons----------------------//
    //----------------------------------------------------------------------//

    public void OpenClosePopUp(){ PopUpObject.SetActive(!SettingsPopUpObject.activeSelf); }
    public void OpenCloseSettings(){ SettingsPopUpObject.SetActive(!SettingsPopUpObject.activeSelf); }
    public void OpenCloseWarning(){ WarningPopUpObject.SetActive(!WarningPopUpObject.activeSelf); }
    public void OpenClosePlayerDeck(){ 
        PlayerDeckPopUpObject.SetActive(!PlayerDeckPopUpObject.activeSelf); 
        if (PlayerDeckPopUpObject.activeSelf){
            PlayerDeckButton.GetComponent<Image>().transform.rotation = Quaternion.Euler(0, 0, 180);
        }else{
            PlayerDeckButton.GetComponent<Image>().transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    /// <summary>
    /// Called by the Pandemic button. Puts all cards from the OpenDeck into the PreviousKnowndeck
    /// </summary>
    public void MoveAllCardsToPreviousknownDeck(){
        List<CardData> l = OpenDeckObject.EmptyDeck();
        if (l.Count == 0){ return; }
        CreateDividerDeckObject(l);
        PlayerDeckPopUpObject.GetComponentInChildren<PlayerDeck>().DrawPandemicCard();
        SaveToFile();
        if (DebugMode){ Debug.Log($"{DebugID} Pressed Pandemic button; Moved all cards in Open Deck to PreviousKnownDeck"); }
    }
    private void CreateDividerDeckObject(List<CardData> cardList){
        GameObject newDeckObject = Instantiate(DividerDeckObject, PreviousKnownDeckObject.gameObject.transform);
        newDeckObject.transform.SetSiblingIndex(0); // Make sure the new deck is always at the start of the list to keep the order of the decks correct
        PreviousKnownDeckObjects.Add(newDeckObject.GetComponent<DeckObject>());
        newDeckObject.GetComponent<DeckObject>().AddCardDataList(cardList);
    }

    /// <summary>
    /// Will move all cards to UnknownDeck to reset to start a new month. Called from the Settings resetMonth button
    /// </summary>
    public void ResetAllCards(){
        if (OpenDeckObject == null){
            Debug.LogError($"{DebugID} OpenDeckObject is null in ResetAllCards");
            return;
        }
        if (UnknownDeckObject == null){
            Debug.LogError($"{DebugID} UnknownDeckObject is null in ResetAllCards");
            return;
        }
        if (MonthDropDownObject == null){
            Debug.LogError($"{DebugID} MonthDropDownObject is null in ResetAllCards");
            return;
        }
        if (PreviousKnownDeckObjects == null){
            PreviousKnownDeckObjects = new List<DeckObject>();
        }

        List<CardData> newList = OpenDeckObject.EmptyDeck();
        for (int i = 0; i < PreviousKnownDeckObjects.Count; i++){
            if (PreviousKnownDeckObjects[i] != null){
                newList.AddRange(PreviousKnownDeckObjects[i].EmptyDeck());
            }
            else if (DebugMode){
                Debug.LogWarning($"{DebugID} Skipping null PreviousKnownDeckObjects[{i}] in ResetAllCards");
            }
        }
        PreviousKnownDeckObjects = new();
        UnknownDeckObject.AddCardDataList(newList);

        SaveToFile(); 
        MonthDropDownObject.NextMonth();

        OpenCloseSettings();
        if (DebugMode){ Debug.Log($"{DebugID} Pressed Reset month button; Moved all cards to the UnknownDeck"); }
    }

    /// <summary>
    /// Will default back to the games initial state with only the beginning infection cards
    /// </summary>
    public void ResetApplication(){
        ResetAllCards(); // Move all cards to UnknownDeck
        OpenCloseWarning();
        UnknownDeckObject.EmptyDeck(); // Remove all cards from UnknownDeck
        SaveInitalCardData();
        if (DebugMode){ Debug.Log($"{DebugID} Pressed Reset button; Reset the application to starting infection deck"); }
    }

    /// <summary>
    /// Called by button and converts sring to integer
    /// </summary>
    public void SetPlayerCards(string cards){
        cards = cards?.Trim() ?? string.Empty;
        if (int.TryParse(cards, out int result)){
            PlayerDeck playerDeck = PlayerDeckPopUpObject.GetComponentInChildren<PlayerDeck>(true);
            if (playerDeck != null){
                playerDeck.SetPlayerDeckSize(result);
                if (DebugMode){ Debug.Log($"{DebugID} Set player deck size to {result}"); }
            }
            else{ Debug.LogError($"{DebugID} Could not find PlayerDeck component inside PlayerDeckPopUpObject"); }
        }
        else{
            Debug.LogError($"Could not parse '{cards}' to an integer");
        }
        OpenClosePlayerDeck();
    }


    //----------------------------------------------------------------------//
    //-----------Functions for starting and stopping application------------//
    //----------------------------------------------------------------------//
    public void SaveToFile(){
        CardDataSaver.SaveToFile(UnknownDeckObject, PreviousKnownDeckObjects, OpenDeckObject, (int)MonthDropDownObject.CurrentMonth);
    }
    public void LoadFromFile(){
        (bool exists, List<CardData> unknownDeck, List<List<CardData>> knownDeck, List<CardData> openDeck, int monthIndex) = CardDataSaver.LoadFromFile();
        if (exists){
            MonthDropDownObject.SetMonth((Month.Months)monthIndex);
            UnknownDeckObject.AddCardDataList(unknownDeck);
            OpenDeckObject.AddCardDataList(openDeck);
            foreach (List<CardData> cards in knownDeck){
                if (cards.Count > 0){ // Ensure that we do not load in empty dividers
                     CreateDividerDeckObject(cards);
                }
            }
            if (DebugMode){ Debug.Log($"{DebugID} Found file on device, loaded cards from memory"); }
        }
        else{
            SaveInitalCardData();
            if (DebugMode){ Debug.Log($"{DebugID} File not found on device, created new file"); }
        }
    }
    private void SaveInitalCardData(){
        CardData w = new() { CardName = "Washington", Amount = 3, Colour = CardData.CardColour.Blue };
        AddCardDataToUnknownDeck(w, w.Amount);
        w = new() { CardName = "New York",      Amount = 3,     Colour = CardData.CardColour.Blue };
        AddCardDataToUnknownDeck(w, w.Amount);
        w = new() { CardName = "Jacksonville",  Amount = 3,     Colour = CardData.CardColour.Yellow };
        AddCardDataToUnknownDeck(w, w.Amount);
        w = new() { CardName = "London",        Amount = 3,     Colour = CardData.CardColour.Blue };
        AddCardDataToUnknownDeck(w, w.Amount);
        w = new() { CardName = "Lagos",         Amount = 3,     Colour = CardData.CardColour.Yellow };
        AddCardDataToUnknownDeck(w, w.Amount);
        w = new() { CardName = "Sao Paolo",     Amount = 3,     Colour = CardData.CardColour.Yellow };
        AddCardDataToUnknownDeck(w, w.Amount);
        w = new() { CardName = "Istanbul",      Amount = 3,     Colour = CardData.CardColour.Black };
        AddCardDataToUnknownDeck(w, w.Amount);
        w = new() { CardName = "Tripoli",       Amount = 3,     Colour = CardData.CardColour.Black };
        AddCardDataToUnknownDeck(w, w.Amount);
        w = new() { CardName = "Cairo",         Amount = 3,     Colour = CardData.CardColour.Black };
        AddCardDataToUnknownDeck(w, w.Amount);
        SaveToFile();
    }
    public void QuitGame(){
        SaveToFile();
        Application.Quit();
    }
}
