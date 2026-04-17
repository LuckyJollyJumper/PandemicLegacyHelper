using UnityEngine;
using UnityEngine.UI;

public class SinglePlayerDeck : MonoBehaviour
{
    [HideInInspector] private int DeckSize;
    [HideInInspector] private int DrawnCards = 0;
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

    public void DrawPandemicCard(){
        PandemicCardObject.SetActive(true);
        AfterPandemicCardObject.SetActive(true);
        BeforePandemicCardObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = (DrawnCards + 2).ToString();
        AfterPandemicCardObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = (DeckSize - DrawnCards - 2).ToString();
    }

    public void SetActiveCurrentDrawPile(bool active){
        if (active){
            CurrentDrawPileObject.GetComponent<Image>().color = Color.red;
            AfterPandemicCardObject.GetComponent<Image>().color = Color.blue;
            BeforePandemicCardObject.GetComponent<Image>().color = Color.blue;
        }
        else{ CurrentDrawPileObject.GetComponent<Image>().color = new Color(1, 1, 1, 0); }
    }

    public int GetDeckSize(){
        return DeckSize;
    }
}
