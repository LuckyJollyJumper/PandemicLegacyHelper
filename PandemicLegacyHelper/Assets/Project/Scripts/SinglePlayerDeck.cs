using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Used to control a single "deck" object that visually represents a part of the draw pile in 
/// the player deck. Only used for visuals and called from inside of the PlayerDeck class.
/// </summary>
public class SinglePlayerDeck : MonoBehaviour
{
    [HideInInspector] private int DeckSize;
    [HideInInspector] private int DrawnCards = 0;
    [Header("References")]
    [SerializeField] private GameObject BeforePandemicCardObject;
    [SerializeField] private GameObject PandemicCardObject;
    [SerializeField] private GameObject AfterPandemicCardObject;
    [SerializeField] private GameObject CurrentDrawPileObject;

    void Start(){
        BeforePandemicCardObject.SetActive(true);
        PandemicCardObject.SetActive(false);
        AfterPandemicCardObject.SetActive(false);
    }

    public void SetDeckSize(int size){
        DeckSize = size;
        BeforePandemicCardObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = DeckSize.ToString();
    }
    public int GetDeckSize(){ return DeckSize; }

    /// <summary>
    /// Reveals the pandemic card visual box and an extra box to split the deck into two parts and set the text
    /// accoring to when it was drawn.
    /// </summary>
    public void DrawPandemicCard(){
        PandemicCardObject.SetActive(true);
        AfterPandemicCardObject.SetActive(true);
        BeforePandemicCardObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = (DrawnCards + 2).ToString();
        AfterPandemicCardObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = (DeckSize - DrawnCards - 2).ToString();
    }

    /// <summary>
    /// Used to set the deck object as active by revealing a bar under the boxes and changing the colour
    /// to indicate it has been drawn and stays this colour until reset of the game.
    /// </summary>
    /// <param name="active"></param>
    public void SetActiveCurrentDrawPile(bool active){
        if (active){
            CurrentDrawPileObject.GetComponent<Image>().color = Color.red;
            AfterPandemicCardObject.GetComponent<Image>().color = new Color(0.56f, 0.56f, 0.56f); //#90908F
            BeforePandemicCardObject.GetComponent<Image>().color = new Color(0.56f, 0.56f, 0.56f);
        }
        else{ CurrentDrawPileObject.GetComponent<Image>().color = new Color(1, 1, 1, 0); }
    }
}
