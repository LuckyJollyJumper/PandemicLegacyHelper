using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class CardDataList{
    public List<CardData> Cards;
}

public static class CardDataSaver{
    public static string FilePath = Path.Combine(Application.persistentDataPath, "cards.json");
    public static void SaveToFile(List<GameObject> c){
        List<CardData> cards = ConvertGameObjectsToCardData(c);
        CardDataList cardDataList = new() { Cards = cards };
        string json = JsonUtility.ToJson(cardDataList);
        File.WriteAllText(FilePath, json);
        Debug.Log($"[CardDataSaver] Saved to {FilePath}");
    }

    public static (bool, List<CardData>) LoadFromFile(){
        if (!File.Exists(FilePath)){
            Debug.LogWarning("File does not exist: " + FilePath);
            return (false, new List<CardData>());
        }

        string json = File.ReadAllText(FilePath);
        CardDataList cardDataList = JsonUtility.FromJson<CardDataList>(json);

        Debug.Log($"[CardDataSaver] Loaded from {FilePath}");

        return (true, cardDataList.Cards.Select(sc => new CardData{
            CardName = sc.CardName,
            Colour = sc.Colour,
            Amount = sc.Amount,
            Probability = sc.Probability
        }).ToList());
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