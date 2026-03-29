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
    // [SerializeField] private GameObject DeckObject; // Prefab object that holds a part of the previousKnownDeck
    [SerializeField] private GameObject DividerDeckObject; // Prefab object that holds a part of the previousKnownDeck
    
    [Header("References")]
    [SerializeField] private DeckObject UnknownDeckObject;
    [SerializeField] private DeckObject PreviousKnownDeckObject;
    [SerializeField] private List<DeckObject> PreviousKnownDeckObjects;
    [SerializeField] private DeckObject OpenDeckObject;
    [SerializeField] private GameObject PopUpObject;
    [SerializeField] private GameObject SettingsObject;
    [Header("Debug")]
    [SerializeField] private bool DebugMode;
    private string DebugID = "[GameManager]";
    
    void Start(){
        PopUpObject.SetActive(false);
        SettingsObject.SetActive(false);

        LoadFromFile();
    }

    //----------------------------------------------------------------------//
    //----------Functions for adding and removing cards from decks----------//
    //----------------------------------------------------------------------//

    /// <summary>
    /// Used to add a new card to the UnknownDeck. Called from the PopUp and when loading in from file
    /// </summary>
    public void AddCardDataToUnknownDeck(CardData newCard, int amount){
        UnknownDeckObject.AddCardData(newCard, amount);
    }

    public void AddCardDataListToPreviousKnownDeck(List<CardData> cards){
        
    }

    public void AddCardDataToOpenDeck(CardData card, DeckObject deck){
        deck.RemoveCardData(card, 1);
        OpenDeckObject.AddCardData(card, 1);
    }

    /// <summary>
     /// Used to remove a single card from the OpenDeck. Called from the CardDataPrefab button
     /// </summary>
    public void RemoveCardFromOpenDeck(CardData card){
        OpenDeckObject.RemoveCardData(card, 1);
    }

    //----------------------------------------------------------------------//
    //-----------------------Functions used by buttons----------------------//
    //----------------------------------------------------------------------//

    public void OpenClosePopUp(){ PopUpObject.SetActive(!SettingsObject.activeSelf); }
    public void OpenCloseSettings(){ SettingsObject.SetActive(!SettingsObject.activeSelf); }

    /// <summary>
    /// Called by the Pandemic button. Puts all cards from the OpenDeck into the PreviousKnowndeck
    /// </summary>
    public void MoveAllCardsToPreviousknownDeck(){
        List<CardData> l = OpenDeckObject.EmptyDeck();
        GameObject newDeckObject = Instantiate(DeckObject, PreviousKnownDeckObject.gameObject.transform);
        PreviousKnownDeckObjects.Add(newDeckObject.GetComponent<DeckObject>());
        newDeckObject.GetComponent<DeckObject>().AddCardDataList(l);
        if (DebugMode){ Debug.Log($"{DebugID} Pressed Pandemic button; Moved all cards in Open Deck to PreviousKnownDeck"); }
    }

    /// <summary>
    /// Will move all cards to UnknownDeck to reset to start a new month. Called from the Settings resetMonth button
    /// </summary>
    public void ResetAllCards(){
        List<CardData> newList = OpenDeckObject.EmptyDeck();
        for (int i = 0; i < PreviousKnownDeckObjects.Count; i++){
            newList.AddRange(PreviousKnownDeckObjects[i].EmptyDeck());
        }
        UnknownDeckObject.AddCardDataList(newList);
        
        OpenCloseSettings();
        if (DebugMode){ Debug.Log($"{DebugID} Pressed Reset month button; Moved all cards to the UnknownDeck"); }
    }
    private void MoveAllCardsToUnknownDeck(List<GameObject> list){
        
    }

    /// <summary>
    /// Will default back to the games initial state with only the beginning infection cards
    /// </summary>
    public void ResetApplication(){
        ResetAllCards(); // Move all cards to UnknownDeck
        UnknownDeckObject.EmptyDeck(); // Remove all cards from UnknownDeck
        SaveInitalCardData();
        if (DebugMode){ Debug.Log($"{DebugID} Pressed Reset button; Reset the application to starting infection deck"); }
    }



    //----------------------------------------------------------------------//
    //-----------Functions for starting and stopping application------------//
    //----------------------------------------------------------------------//
    public void SaveToFile(){
        // TODO: save all lists
        CardDataSaver.SaveToFile(UnknownDeckObject.Deck);
    }
    public void LoadFromFile(){
        (bool exists, List<CardData> loadedCards) = CardDataSaver.LoadFromFile();
        if (exists){
            foreach (CardData card in loadedCards){
                UnknownDeckObject.AddCardData(card, card.Amount);
            }
            if (DebugMode){ Debug.Log($"{DebugID} Loaded cards from memory"); }
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
