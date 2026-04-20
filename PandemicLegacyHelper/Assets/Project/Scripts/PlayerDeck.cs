using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerDeck : MonoBehaviour
{
    private int PlayerDeckSize = 40;
    private int PandemicDeckSize; // Size of the part of the playerDeck that holds a single pandemic card
    private int NumberOfPandemicCards = 10; // Number of pandemic cards in the playerDeck
    private List<SinglePlayerDeck> SingleDecks = new List<SinglePlayerDeck>(); // List to hold the sizes of each pandemic deck
    [Header("References")]
    [SerializeField] private GameObject SingleDeckObject;
    [SerializeField] private TMPro.TextMeshProUGUI TextObject;
    [SerializeField]private int CurrentDrawnCards = 0;
    private bool PandemicCardDrawn = false; // only used per SinglePlayerDeck
    [Header("Debug")]
    [SerializeField] private bool DebugMode = false;
    [SerializeField] private string DebugID = "[PlayerDeck]";

    public void Start(){
        // All placeholders
        PlayerDeckSize = 80;
        NumberOfPandemicCards = 10;

        SetPlayerDeckSize(PlayerDeckSize);
    }

    public void SetPlayerDeckSize(int size){
        PlayerDeckSize = size;
        SetPandemicDeckSize();
        CalculateProbability();
    }
    /// <summary>
    /// Sets the size of each pandemic deck based on the total player deck size and the number of pandemic cards.
    /// Only used as a setup
    /// </summary>
    public void SetPandemicDeckSize(){
        for (int i = SingleDecks.Count - 1; i >= 0; i--){
            if (SingleDecks[i] != null){
                Destroy(SingleDecks[i].gameObject);
            }
        }
        SingleDecks.Clear();

        PandemicDeckSize = PlayerDeckSize / NumberOfPandemicCards;
        int quotient = PlayerDeckSize / NumberOfPandemicCards;
        int remainder = PlayerDeckSize % NumberOfPandemicCards;
        if (DebugMode) {
            Debug.Log($"{DebugID} Player Deck Size: {PlayerDeckSize}, Pandemic Deck Size: {PandemicDeckSize}, Quotient: {quotient}, Remainder: {remainder}");
        }

        for (int i = 0; i < NumberOfPandemicCards; i++){
            GameObject singleDeck = Instantiate(SingleDeckObject, this.transform);
            SinglePlayerDeck singleDeckComponent = singleDeck.GetComponent<SinglePlayerDeck>();
            if (singleDeckComponent == null){
                Debug.LogError($"{DebugID} Missing SinglePlayerDeck component on instantiated SingleDeckObject");
                continue;
            }

            // Distribute the remaining cards among the first few pandemic decks
            if (remainder != 0) {
                singleDeckComponent.SetDeckSize(PandemicDeckSize + 1);
                remainder -= 1;
            } else {
                singleDeckComponent.SetDeckSize(PandemicDeckSize);
            }
            SingleDecks.Add(singleDeckComponent);
        }

        if (SingleDecks.Count > 0){
            SingleDecks[0].SetActiveCurrentDrawPile(true);
        }
    }


    public void DrawCards(){
        if (CurrentDrawnCards >= PlayerDeckSize){ return; }
        CurrentDrawnCards += 2;
        if(GotIntoNewPandemicDeck()){ 
            PandemicCardDrawn = false; 
            (int i ,_) = GetCurrentPandemicDeckIndex(CurrentDrawnCards);
            SingleDecks[i].SetActiveCurrentDrawPile(true);
            if (i-1 >= 0){
                SingleDecks[i-1].SetActiveCurrentDrawPile(false);
            }
        }
        CalculateProbability();
    }
    public void DrawPandemicCard(){
        PandemicCardDrawn = true;
        if (CurrentDrawnCards < PlayerDeckSize){
            Debug.Log($"Pandemic card drawn");
            (int deckIndex, int cardIndex) = GetCurrentPandemicDeckIndex(CurrentDrawnCards);
            SingleDecks[deckIndex].DrawPandemicCard();
            DrawCards();
        }
    }

    public void CalculateProbability(){
        if (SingleDecks.Count == 0){
            TextObject.text = "No player deck configured";
            return;
        }

        (int deckIndex, int cardIndex) = GetCurrentPandemicDeckIndex(CurrentDrawnCards);
        if (deckIndex < 0 || deckIndex >= SingleDecks.Count || SingleDecks[deckIndex] == null){
            TextObject.text = "No player deck configured";
            return;
        }

        int deckSize = SingleDecks[deckIndex].GetDeckSize();
        if (deckSize - cardIndex <= 0){
            TextObject.text = "No cards remaining";
            return;
        }

        if (PandemicCardDrawn){
            TextObject.text = $"Safe for {deckSize - cardIndex} cards";
        }
        else{
            TextObject.text = $"Probability of Pandemic Card in next draw: {MathF.Round(2f / (float)(deckSize - cardIndex) * 100, 1)}%";
        }
    }

    /// <summary>
    /// Returns (singleDeckIndex, cardIndex), where cardIndex is the index of the card in the current singleDeck that the game
    /// is in. 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public (int, int) GetCurrentPandemicDeckIndex(int index){
        if (SingleDecks.Count == 0){
            return (-1, -1);
        }

        foreach (SinglePlayerDeck singleDeck in SingleDecks){
            if (singleDeck == null){
                continue;
            }

            int c = singleDeck.GetDeckSize();
            index -= c;
            if (index < 0){
                return (SingleDecks.IndexOf(singleDeck), index + c);
            }
        }
        return (-1, -1); // Return (-1, -1) if index is out of bounds
    }

    /// <summary>
    /// Returns whether we just moved into a new pandemic deck with the previous draw of 2 cards
    /// </summary>
    public bool GotIntoNewPandemicDeck(){
        int previousIndexArg = Mathf.Max(CurrentDrawnCards - 2, 0);
        (int i ,_) = GetCurrentPandemicDeckIndex(CurrentDrawnCards);
        (int pi, _) = GetCurrentPandemicDeckIndex(previousIndexArg);
        Debug.Log($"Current Index: {i}, Previous Index: {pi}");
        return i != pi;
    }
}
