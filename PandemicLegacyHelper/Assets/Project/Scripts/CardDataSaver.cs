using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class CardDataList{
    public List<CardData> Cards;
}

[System.Serializable]
public class SaveData {
    public CardDataList UnknownDeck;
    public List<CardDataList> KnownDeck;
    public CardDataList OpenDeck;
    public int Month;
}

public static class CardDataSaver{
    public static string FilePath = Path.Combine(Application.persistentDataPath, "cards.json");
    
    public static void SaveToFile(DeckObject UnknownDeck, List<DeckObject> KnownDeck, DeckObject OpenDeck, int monthIndex){
        SaveData saveData = new SaveData {
            UnknownDeck = new CardDataList { Cards = ConvertGameObjectsToCardData(UnknownDeck.Deck) },
            KnownDeck = KnownDeck.Select(deckObj => new CardDataList { Cards = ConvertGameObjectsToCardData(deckObj.Deck) }).ToList(),
            OpenDeck = new CardDataList { Cards = ConvertGameObjectsToCardData(OpenDeck.Deck) },
            Month = monthIndex
        };
        
        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(FilePath, json);
        Debug.Log($"[CardDataSaver] Saved to {FilePath}");
    }

    public static (bool, List<CardData>, List<List<CardData>>, List<CardData>, int) LoadFromFile(){
        if (!File.Exists(FilePath)){
            Debug.LogWarning("File does not exist: " + FilePath);
            return (false, new List<CardData>(), new List<List<CardData>>(), new List<CardData>(), 0);
        }

        string json = File.ReadAllText(FilePath);
        SaveData saveData = JsonUtility.FromJson<SaveData>(json) ?? new SaveData();

        List<CardData> unknownDeck = saveData.UnknownDeck?.Cards ?? new List<CardData>();
        List<List<CardData>> knownDeck = saveData.KnownDeck?.Select(cdl => cdl.Cards ?? new List<CardData>()).ToList() ?? new List<List<CardData>>();
        List<CardData> openDeck = saveData.OpenDeck?.Cards ?? new List<CardData>();
        int monthIndex = saveData.Month;

        Debug.Log($"[CardDataSaver] Loaded from {FilePath}");

        return (true, unknownDeck, knownDeck, openDeck, monthIndex);
    }

    public static List<CardData> ConvertGameObjectsToCardData(List<GameObject> gameObjects){
        List<CardData> cardDataList = new List<CardData>();
        foreach (GameObject obj in gameObjects){
            CardPrefab cardPrefab = obj.GetComponent<CardPrefab>();
            if (cardPrefab != null && cardPrefab.GetData() != null){
                cardDataList.Add(cardPrefab.GetData());
            }
        }
        return cardDataList;
    }
}