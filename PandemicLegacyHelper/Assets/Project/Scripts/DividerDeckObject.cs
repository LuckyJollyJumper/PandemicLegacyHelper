using UnityEngine;

/// <summary>
/// Respresent a subDeck used in KnownDeckObject and is a child of DeckObject. Needed to be able to delete the SubDeck when the 
/// deck is empty.
/// </summary>
public class DividerDeckObject : DeckObject
{
    public override void RemoveCardData(CardData card, int amount){
        base.RemoveCardData(card, amount);
        if(Deck.Count == 0){
            Destroy(this.gameObject);
        }
    }
}
