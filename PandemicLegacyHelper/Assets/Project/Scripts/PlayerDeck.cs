using UnityEngine;

public class PlayerDeck : MonoBehaviour
{
    private int PlayerDeckSize;
    private int PandemicDeckSize; // Size of the part of the playerDeck that holds a single pandemic card
    private int NumberOfPandemicCards; // Number of pandemic cards in the playerDeck
    private List<int> PandemicDeckSizes;
    [SerializeField] private GameObject SingleDeckObject;

    public void SetPandemicDeckSize(){
        int quotient = PlayerDeckSize / PandemicDeckSize;
        int remainder = PlayerDeckSize % PandemicDeckSize;

        for (int i = 0; i < quotient; i++){
            PandemicDeckSizes[i] = PandemicDeckSize;
            // Distribute the remaining cards among the first few pandemic decks
            if (remainder != 0) { PandemicDeckSizes[i] += 1; remainder -= 1; }
            GameObject singleDeck = Instantiate(SingleDeckObject, this.transform);
        }
    }
}
