using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class CardDataList{
    public List<CardData> cards;
}

public static class CardDataSaver{
    public static void SaveToFile(List<CardData> cards){
        var serializableCards = cards.Select(c => new CardData{
            CardName = c.CardName,
            Colour = c.Colour,
            Amount = c.Amount,
            Probability = c.Probability,
        }).ToList();

        CardDataList cardDataList = new CardDataList { cards = serializableCards };
        string json = JsonUtility.ToJson(cardDataList);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "cards.json"), json);
    }

    public static List<CardData> LoadFromFile(){
        string FilePath = Path.Combine(Application.persistentDataPath, "cards.json");
        if (!File.Exists(FilePath)){
            Debug.LogWarning("File does not exist: " + FilePath);
            return new List<CardData>();
        }

        string json = File.ReadAllText(FilePath);
        CardDataList cardDataList = JsonUtility.FromJson<CardDataList>(json);

        return cardDataList.cards.Select(sc => new CardData{
            CardName = sc.CardName,
            Colour = sc.Colour,
            Amount = sc.Amount,
            Probability = sc.Probability
        }).ToList();
    }
}