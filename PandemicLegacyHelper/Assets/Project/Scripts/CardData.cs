using UnityEngine;

/// <summary>
/// Class used to store the data of a single card
/// </summary>
[System.Serializable]
public class CardData
{
    public enum CardColour {Blue, Black, Yellow, Green};

    public string CardName;
    public CardColour Colour;
    public int Amount;
    public float Probability;

    /// <summary>
    /// Used to create a copy of the CardData, so that we can have multiple instances of the same card with different probabilities and amounts.
    /// </summary>
    public CardData Clone(){
        return new CardData {
            CardName = this.CardName,
            Colour = this.Colour,
            Amount = this.Amount,
            Probability = this.Probability
        };
    }

    /// <summary>
    /// Used to get a unity Color matched with CardColour for UI elements.
    /// </summary>
    /// <param name="colour"></param>
    /// <returns></returns>
    public Color GetCardColour(){
        return this.Colour switch{
            CardData.CardColour.Blue => new Color(0.284f, 0.742f, 0.893f), //#48BDE4
            CardData.CardColour.Black => new Color(0.284f, 0.284f, 0.284f), //#484848
            CardData.CardColour.Yellow => new Color(0.953f, 0.973f, 0.333f), //#F3F855
            CardData.CardColour.Green => new Color(0.24f, 0.56f, 0.302f), //#3D8F4D
            _ => new Color(64f,64f,64f)
        };
    }
}
