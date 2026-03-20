using UnityEngine;

public class CardData
{
    public enum CardColour {Blue, Black, Yellow};

    public string CardName;
    public CardColour Colour;
    public int Amount;
    public float Probability;


    /// <summary>
    /// Used to get a unity Color matched with CardColour for UI elements.
    /// </summary>
    /// <param name="colour"></param>
    /// <returns></returns>
    public Color GetCardColour(){
        return this.Colour switch{
            CardData.CardColour.Blue => new Color(0.284f, 0.742f, 0.893f),
            CardData.CardColour.Black => new Color(0.284f, 0.284f, 0.284f),
            CardData.CardColour.Yellow => new Color(0.953f, 0.973f, 0.333f),
            _ => new Color(64f,64f,64f)
        };
    }
}
