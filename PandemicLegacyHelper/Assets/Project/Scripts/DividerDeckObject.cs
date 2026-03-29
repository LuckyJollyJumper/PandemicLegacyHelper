using UnityEngine;

/// <summary>
/// Ugly fix to remove the fillers when they are not needed anymore
/// </summary>
public class KnownDeckObject : DeckObject
{
    public override void RemoveCardData(CardData card, int amount){
        base.RemoveCardData(card, amount);
        if(Deck.Count == 0){
            Destroy(this.gameObject);
        }
    }
}
